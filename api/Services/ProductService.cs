using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
   public class ProductService
{
    private readonly AppDbContext _context; // The database context used to interact with the product data in the database.

    // Constructor that injects the AppDbContext into the service.
    public ProductService(AppDbContext context)
    {
        _context = context; // Assign the provided context to the class-level _context variable.
    }

    // AddProductAsync method handles the logic for adding a new product to the database.
    public async Task<ProductResponseDto> AddProductAsync(ProductRequestDto request)
    {
        // Create a new product object from the data in the request DTO (ProductRequestDto).
        var product = new Models.Products
        {
            Name = request.Name, // Map the product name from the request.
            Category = request.Category // Map the product category from the request.
        };

        // Add the new product to the Products DbSet in the context.
        _context.Products.Add(product);

        // Save the changes to the database asynchronously.
        await _context.SaveChangesAsync();

        // Return a ProductResponseDto containing the created product's details.
        return new ProductResponseDto
        {
            Id = product.Id, // Return the generated product ID.
            Name = product.Name, // Return the product's name.
            Category = product.Category // Return the product's category.
        };
    }

    // GetProductsAsync method handles fetching a list of products from the database.
    public async Task<List<ProductResponseDto>> GetProductsAsync(int page, int pageSize)
    {
        // Query the products, apply pagination using Skip and Take, and map the result to ProductResponseDto.
        return await _context.Products
            .Skip((page - 1) * pageSize) // Skip the products based on the page number and page size.
            .Take(pageSize) // Take the specified number of products for the current page.
            .Select(p => new ProductResponseDto
            {
                Id = p.Id, // Return the product ID.
                Name = p.Name, // Return the product name.
                Category = p.Category // Return the product category.
            }).ToListAsync(); // Execute the query and return the results as a list of ProductResponseDto.
    }
}
}