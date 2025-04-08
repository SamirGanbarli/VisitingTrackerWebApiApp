using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
public class UserService
{
    private readonly AppDbContext _context; // The database context used to interact with the User data in the database.

    // Constructor that injects the AppDbContext into the service.
    public UserService(AppDbContext context)
    {
        _context = context; // Assign the provided context to the class-level _context variable.
    }

    // GetLoggedInUserAsync method fetches the logged-in user's details based on the ClaimsPrincipal.
    public async Task<Users> GetLoggedInUserAsync(ClaimsPrincipal principal)
    {
        // Find the user ID from the claims in the provided ClaimsPrincipal (which represents the logged-in user).
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // If the user ID is not found in the claims, throw an exception.
        if (userIdClaim == null)
            throw new Exception("User not found in claims");

        // Parse the user ID from the claim value (which is expected to be a string) to an integer.
        var userId = int.Parse(userIdClaim);

        // Fetch the user from the database using the parsed user ID.
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        // If the user is not found in the database, throw an exception.
        if (user == null)
            throw new Exception("User not found");

        // Return the user object fetched from the database.
        return user;
    }
}

}