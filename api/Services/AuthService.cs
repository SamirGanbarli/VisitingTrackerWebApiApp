using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Middleware;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace api.Services
{
public class AuthService
{
    private readonly AppDbContext _context; // The database context used to interact with the users' data in the database.
    private readonly JwtSettings _jwtSettings; // Settings related to JWT (JSON Web Token), such as key, issuer, audience, etc.

    // Constructor with dependency injection for AppDbContext (database) and JwtSettings (JWT configuration).
    public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
    {
        _context = context;
        _jwtSettings = jwtOptions.Value; // Retrieve the JwtSettings from the provided IOptions.
    }

    // The LoginAsync method handles user login.
    // It checks the provided username and password against the database and returns a JWT token if credentials are valid.
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Find the user from the database by the provided username.
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        // If the user doesn't exist or the password doesn't match the stored hash, throw an UnauthorizedAccessException.
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        // Generate a JWT token for the authenticated user.
        var token = GenerateJwtToken(user);

        // Return the token as part of the LoginResponse.
        return new LoginResponse { Token = token };
    }

    // The GenerateJwtToken method generates a JWT token for the given user.
    // It uses user data, like username and role, to include claims in the token.
    private string GenerateJwtToken(Users user)
    {
        // Define the symmetric security key for signing the token (from the JWT settings).
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        // Define the signing credentials using the security key and HMAC SHA256 algorithm.
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the claims for the JWT (e.g., user ID, role, username).
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject claim (user ID).
            new Claim(ClaimTypes.Role, user.Role), // Role claim.
            new Claim("username", user.Username) // Custom claim for username.
        };

        // Create the JWT token with the claims, expiration time, and signing credentials.
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer, // The issuer of the token.
            audience: _jwtSettings.Audience, // The audience of the token.
            claims: claims, // The claims embedded in the token.
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes), // Set the expiration time for the token.
            signingCredentials: credentials // Signing credentials.
        );

        // Return the generated JWT as a string.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

}