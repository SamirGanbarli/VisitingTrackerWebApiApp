using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
[ApiController] // Specifies that this class is an API controller, which automatically applies model validation, binding, and other features. It also indicates that it's designed to handle HTTP requests.
[Route("api/products")] // Defines the base route for all actions within this controller. All actions will be prefixed with "api/products" (e.g., /api/products, /api/products/{id}, etc.)
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService; // The ProductService instance used to handle product-related logic.

    // Constructor with dependency injection for ProductService.
    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    // POST method for adding a new product (Admin only)
    [HttpPost] // This attribute defines the route for the AddProduct action. It will handle POST requests to /api/products.
    [Authorize(Roles = "Admin")] // Ensures that only users with the "Admin" role can access this action.
    public async Task<IActionResult> AddProduct(ProductRequestDto request) // The 'request' is expected to be sent in the body of the POST request.
    {
        var product = await _productService.AddProductAsync(request); // Call the AddProductAsync method in ProductService to handle adding the new product.
        return Ok(product); // Return an HTTP 200 response with the added product.
    }

    // GET method for listing products with pagination support
    [HttpGet] // This attribute defines the route for the GetProducts action. It will handle GET requests to /api/products.
    [Authorize] // Ensures that the user is authenticated before accessing this action. Any authenticated user can access this endpoint.
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10) // The 'page' and 'pageSize' parameters are passed via query string in the GET request.
    {
        var products = await _productService.GetProductsAsync(page, pageSize); // Call the GetProductsAsync method in ProductService to fetch the list of products with pagination.
        return Ok(products); // Return an HTTP 200 response with the list of products.
    }
}
}