using System;

namespace UserService.Dtos;

public class UserRegistrationDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Bu alan, Keycloak için kullanılacak
}
