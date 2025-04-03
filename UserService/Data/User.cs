using System;

namespace UserService.Data;

public class User
{
    public int Id { get; set; }
    public string KeycloakId { get; set; } // Keycloak'tan alınan ID
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
