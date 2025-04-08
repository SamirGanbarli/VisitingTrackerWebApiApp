using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    
[ApiController] // Specifies that this class is an API controller, which automatically applies model validation, binding, and other features. It also indicates that it's designed to handle HTTP requests.
[Route("api/auth")] // Defines the base route for all actions within this controller. All actions will be prefixed with "api/auth" (e.g., /api/auth/login, /api/auth/register, etc.)
public class AuthController : ControllerBase
{
    private readonly AuthService _authService; // The AuthService instance used to handle authentication logic.
    private readonly AppDbContext _context; // The database context to interact with the application's database.

    // Constructor with dependency injection for AuthService and AppDbContext.
    public AuthController(AuthService authService, AppDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    // POST method for logging in a user
    [HttpPost("login")] // This attribute defines the route for the login action. It will handle POST requests to /api/auth/login.
    public async Task<IActionResult> Login([FromBody] LoginRequest request) // The 'request' is expected to be sent in the body of the POST request.
    {
        try
        {
            var result = await _authService.LoginAsync(request); // Call the LoginAsync method in AuthService to handle login.
            return Ok(result); // Return an HTTP 200 response with the login result.
        }
        catch (UnauthorizedAccessException) // If login fails due to incorrect credentials, catch the exception.
        {
            return Unauthorized(new { message = "Invalid username or password." }); // Return an HTTP 401 Unauthorized response with an error message.
        }
    }

    // POST method for registering a new user
    [HttpPost("register")] // This attribute defines the route for the register action. It will handle POST requests to /api/auth/register.
    public async Task<IActionResult> Register([FromBody] RegisterRequest request) // The 'request' is expected to be sent in the body of the POST request.
    {
        // Check if the username already exists in the database.
        var existingUser = await _context.Users
            .SingleOrDefaultAsync(u => u.Username == request.Username);

        if (existingUser != null) // If the username exists, return a BadRequest response.
        {
            return BadRequest("Username already exists.");
        }

        // Hash the password before saving it to the database.
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create a new user instance.
        var user = new Users
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        // Add the new user to the Users table in the database.
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save the changes to the database.

        return Ok(new { Message = "User registered successfully." }); // Return a success response indicating successful registration.
    }

    // GET method to retrieve a list of users (Admin only)
    [HttpGet("users")] // This attribute defines the route for the GetUsers action. It will handle GET requests to /api/auth/users.
    public async Task<IActionResult> GetUsers()
    {
        // Ensure the user is an admin by checking the role in the user's claims.
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userRole != "Admin") // If the logged-in user is not an admin, return a 403 Forbidden response.
        {
            return Forbid();  // Return 403 Forbidden if not an Admin.
        }

        // Fetch all users from the Users table in the database.
        var users = await _context.Users.ToListAsync();

        return Ok(users); // Return the list of users with an HTTP 200 OK response.
    }    
}
}