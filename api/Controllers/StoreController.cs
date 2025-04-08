using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{


[Microsoft.AspNetCore.Mvc.Route("api/stores")] // Defines the base route for all actions in this controller. The URL for the actions will be prefixed with "api/stores" (e.g., /api/stores, /api/stores/{id}, etc.)
[ApiController] // Marks this class as an API controller, which automatically applies model validation, request data binding, and other features for Web API controllers.
public class StoresController : ControllerBase
{
    private readonly StoreService _storeService; // The StoreService instance used to handle business logic for stores.

    // Constructor with dependency injection for StoreService.
    public StoresController(StoreService storeService)
    {
        _storeService = storeService;
    }

    // GET method to list all stores with pagination
    [HttpGet] // Specifies that this action will handle GET requests to /api/stores.
    public async Task<IActionResult> GetStores([FromQuery] int page = 1, [FromQuery] int pageSize = 10) // The 'page' and 'pageSize' parameters are passed via query string.
    {
        var stores = await _storeService.GetStoresAsync(page, pageSize); // Call the GetStoresAsync method in StoreService to fetch a list of stores with pagination.
        return Ok(stores); // Return an HTTP 200 response with the list of stores.
    }

    // POST method to create a new store (Admin only)
    [HttpPost] // Specifies that this action will handle POST requests to /api/stores.
    [Authorize(Roles = "Admin")] // Ensures that only users with the "Admin" role can access this action.
    public async Task<IActionResult> CreateStore([FromBody] StoreRequestDto storeDTO) // The store data is expected in the body of the POST request as a StoreRequestDto.
    {
        var store = await _storeService.CreateStoreAsync(storeDTO); // Call the CreateStoreAsync method in StoreService to create a new store.
        return CreatedAtAction(nameof(GetStores), new { id = store.Id }, store); // Return an HTTP 201 Created response with the location of the newly created store.
    }

    // PUT method to update an existing store (Admin only)
    [HttpPut("{storeId}")] // Specifies that this action will handle PUT requests to /api/stores/{storeId}.
    [Authorize(Roles = "Admin")] // Ensures that only users with the "Admin" role can access this action.
    public async Task<IActionResult> UpdateStore(int storeId, [FromBody] StoreRequestDto storeDTO) // The store ID is passed as a route parameter and the updated store data is in the body.
    {
        var store = await _storeService.UpdateStoreAsync(storeId, storeDTO); // Call the UpdateStoreAsync method in StoreService to update the store with the provided storeId.
        return Ok(store); // Return an HTTP 200 response with the updated store details.
    }

    // DELETE method to delete a store (Admin only)
    [HttpDelete("{storeId}")] // Specifies that this action will handle DELETE requests to /api/stores/{storeId}.
    [Authorize(Roles = "Admin")] // Ensures that only users with the "Admin" role can access this action.
    public async Task<IActionResult> DeleteStore(int storeId) // The storeId is passed as a route parameter.
    {
        await _storeService.DeleteStoreAsync(storeId); // Call the DeleteStoreAsync method in StoreService to delete the store with the provided storeId.
        return NoContent(); // Return an HTTP 204 No Content response to indicate the deletion was successful.
    }
}

}