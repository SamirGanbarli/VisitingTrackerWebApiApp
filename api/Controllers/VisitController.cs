using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
[Route("api/visits")] // Specifies the base route for this controller. All routes within this controller will start with "api/visits" (e.g., /api/visits, /api/visits/{id}).
[ApiController] // Marks this class as an API controller, enabling model binding, automatic validation, and other Web API features.
public class VisitsController : ControllerBase
{
    private readonly VisitService _visitService; // The service that handles business logic for visits.
    private readonly UserService _userService; // The service to handle fetching user details.

    // Constructor with dependency injection for VisitService and UserService.
    public VisitsController(VisitService visitService, UserService userService)
    {
        _visitService = visitService;
        _userService = userService;
    }

    // GET method to list all visits for a logged-in user (with pagination).
    [HttpGet] // Specifies that this method will handle GET requests to /api/visits.
    [Authorize] // Ensures that only authenticated users can access this endpoint.
    public async Task<ActionResult<List<VisitResponseDto>>> GetVisits(int page = 1, int pageSize = 10) // Pagination parameters from query string.
    {
        var user = await _userService.GetLoggedInUserAsync(User); // Fetch the logged-in user details from the current context.

        // Fetch all visits for the logged-in user (using pagination).
        var visits = await _visitService.GetVisitsAsync(user, page, pageSize);
        return Ok(visits); // Return an HTTP 200 response with the list of visits.
    }

    // POST method to create a new visit for a logged-in user (Standard role only).
    [HttpPost] // Specifies that this method will handle POST requests to /api/visits.
    [Authorize(Roles = "Standard")] // Restricts access to users with the "Standard" role.
    public async Task<ActionResult<VisitResponseDto>> CreateVisit([FromBody] VisitRequestDto visit) // Visit data comes from the request body.
    {
        var user = await _userService.GetLoggedInUserAsync(User); // Get the logged-in user.

        if (visit.UserId != user.Id) // Ensure the user is trying to create a visit for themselves.
            return Unauthorized("You can only create visits for yourself."); // Return 401 Unauthorized if they try to create a visit for someone else.

        // Call service to create the visit and return the created visit.
        var createdVisit = await _visitService.CreateVisitAsync(visit, user.Id);
        return CreatedAtAction(nameof(GetVisits), new { id = createdVisit.Id }, createdVisit); // Return HTTP 201 Created with the location of the created visit.
    }

    // PUT method to mark a visit as completed (Standard role only).
    [HttpPut("{visitId}/complete")] // Specifies that this method handles PUT requests to /api/visits/{visitId}/complete.
    [Authorize(Roles = "Standard")] // Restricts access to users with the "Standard" role.
    public async Task<ActionResult<VisitResponseDto>> CompleteVisit(int visitId) // The visit ID is passed as a route parameter.
    {
        var user = await _userService.GetLoggedInUserAsync(User); // Get the logged-in user.

        var visit = await _visitService.CompleteVisitAsync(visitId); // Mark the visit as completed.

        // Check if the logged-in user owns the visit (they can only complete their own visits).
        if (visit.UserId != user.Id)
            return Unauthorized("You can only complete your own visits."); // Return 401 Unauthorized if they try to complete someone else's visit.

        return Ok(visit); // Return an HTTP 200 response with the updated visit (status marked as completed).
    }

    // POST method to upload a photo for a visit (Standard role only).
    [HttpPost("{visitId}/photos")] // Specifies that this method handles POST requests to /api/visits/{visitId}/photos.
    [Authorize(Roles = "Standard")] // Restricts access to users with the "Standard" role.
    public async Task<ActionResult> UploadPhoto(int visitId, [FromBody] PhotoRequestDto photoUploadDTO) // The visitId is a route parameter, and photo data is in the request body.
    {
        try
        {
            // Validate that Base64 photo data is provided in the request body.
            if (string.IsNullOrEmpty(photoUploadDTO.Base64Image))
            {
                return BadRequest("Base64 photo data is required."); // Return 400 Bad Request if no image data is provided.
            }

            // Call service to upload the photo for the specified visit.
            var photo = await _visitService.UploadPhotoAsync(visitId, photoUploadDTO);

            return Ok(new { Message = "Photo uploaded successfully", PhotoId = photo.Id }); // Return an HTTP 200 response with success message and the uploaded photo ID.
        }
        catch (KeyNotFoundException) // Catch exception if the visitId does not exist in the database.
        {
            return NotFound("Visit not found."); // Return 404 Not Found if the visit with the provided ID doesn't exist.
        }
    }
}

}