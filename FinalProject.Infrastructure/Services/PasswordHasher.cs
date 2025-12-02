using FinalProject.Application.Interfaces;

namespace FinalProject.Infrastructure.Services;

/// <summary>
/// Password hashing service using BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password using BCrypt
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies a password against a BCrypt hash or plain text (for development)
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        // Si el hash no empieza con $2, es texto plano (desarrollo)
        if (!hash.StartsWith("$2"))
        {
            return password == hash;
        }
        
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
