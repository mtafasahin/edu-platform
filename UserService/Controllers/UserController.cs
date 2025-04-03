using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly string _keycloakTokenUrl = "http://keycloak:8080/realms/edu-platform/protocol/openid-connect/token";
    private readonly string _keycloakUserUrl = "http://keycloak:8080/admin/realms/edu-platform/users";
    private readonly string _keycloakClientId = "kong-client"; // Keycloak Client ID
    private readonly string _keycloakClientSecret = "ZTNCwoHbehHkXlXn6LlOYbEudYxYWpNA"; // Keycloak Client Secret

    public UserController(ApplicationDbContext dbContext, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto registration)
    {
        if (registration == null)
        {
            return BadRequest("Invalid registration data.");
        }

        // Keycloak'a kullanıcı kaydetme
        var keycloakResponse = await RegisterUserInKeycloak(registration);
        if (string.IsNullOrWhiteSpace(keycloakResponse))
        {
            return BadRequest("Error registering user in Keycloak.");
        }
        
        // UserService veritabanına kullanıcı ekleme
        var user = new User
        {
            KeycloakId = keycloakResponse,
            FirstName = registration.FirstName,
            LastName = registration.LastName,
            Email = registration.Email            
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "User registered successfully", userId = user.Id });
    }

    private async Task<string> RegisterUserInKeycloak(UserRegistrationDto registration)
    {
        // First, get access token from Keycloak
        var tokenRequestData = new Dictionary<string, string>
        {
            { "client_id", _keycloakClientId },
            { "client_secret", _keycloakClientSecret },
            { "grant_type", "client_credentials" }
        };

        var tokenRequestContent = new FormUrlEncodedContent(tokenRequestData);
        var tokenResponse = await _httpClient.PostAsync(_keycloakTokenUrl, tokenRequestContent);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        var tokenData = JsonSerializer.Deserialize<TokenResponse>(tokenJson);
        

        // Create user in Keycloak
        var userRequest = new
        {
            username = registration.Email,
            email = registration.Email,
            enabled = true,
            firstName = registration.FirstName,
            lastName = registration.LastName,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = registration.Password,
                    temporary = false
                }
            }
        };

        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

        var userResponse = await _httpClient.PostAsJsonAsync(
            _keycloakUserUrl, 
            userRequest);

        if (!userResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var createdUserLocation = userResponse.Headers.Location;
        var userId = createdUserLocation?.Segments.Last();
        return userId ?? string.Empty;
        
    }
}
