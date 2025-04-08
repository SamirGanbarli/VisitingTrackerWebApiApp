using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
public class StoreService
{
    private readonly AppDbContext _context; // The database context used to interact with the Store data in the database.

    // Constructor that injects the AppDbContext into the service.
    public StoreService(AppDbContext context)
    {
        _context = context; // Assign the provided context to the class-level _context variable.
    }

    // GetStoresAsync method handles fetching a list of stores from the database with pagination support.
    public async Task<List<StoreResponseDTO>> GetStoresAsync(int page = 1, int pageSize = 10)
    {
        // Query the stores, apply pagination using Skip and Take, and map the result to StoreResponseDTO.
        var stores = await _context.Stores
            .Skip((page - 1) * pageSize) // Skip the stores based on the page number and page size.
            .Take(pageSize) // Take the specified number of stores for the current page.
            .Select(store => new StoreResponseDTO
            {
                Id = store.Id, // Return the store ID.
                Name = store.Name, // Return the store name.
                Location = store.Location, // Return the store location.
                CreatedAt = store.CreatedAt // Return the store creation date.
            })
            .ToListAsync(); // Execute the query and return the results as a list of StoreResponseDTO.

        return stores; // Return the list of stores.
    }

    // CreateStoreAsync method handles the logic for creating a new store.
    public async Task<StoreResponseDTO> CreateStoreAsync(StoreRequestDto storeDTO)
    {
        // Create a new store object from the data in the request DTO (StoreRequestDto).
        var store = new Stores
        {
            Name = storeDTO.Name, // Map the store name from the request.
            Location = storeDTO.Location, // Map the store location from the request.
            CreatedAt = DateTime.UtcNow // Set the store creation date to the current UTC time.
        };

        // Add the new store to the Stores DbSet in the context.
        _context.Stores.Add(store);

        // Save the changes to the database asynchronously.
        await _context.SaveChangesAsync();

        // Return a StoreResponseDTO containing the created store's details.
        return new StoreResponseDTO
        {
            Id = store.Id, // Return the generated store ID.
            Name = store.Name, // Return the store's name.
            Location = store.Location, // Return the store's location.
            CreatedAt = store.CreatedAt // Return the store's creation date.
        };
    }

    // UpdateStoreAsync method handles updating an existing store.
    public async Task<StoreResponseDTO> UpdateStoreAsync(int storeId, StoreRequestDto storeDTO)
    {
        // Find the store by its ID. If not found, throw a KeyNotFoundException.
        var store = await _context.Stores.FindAsync(storeId);
        if (store == null) throw new KeyNotFoundException("Store not found");

        // Update the store's properties with the new values from the request.
        store.Name = storeDTO.Name;
        store.Location = storeDTO.Location;

        // Update the store in the context.
        _context.Stores.Update(store);

        // Save the changes to the database asynchronously.
        await _context.SaveChangesAsync();

        // Return a StoreResponseDTO containing the updated store's details.
        return new StoreResponseDTO
        {
            Id = store.Id, // Return the store ID.
            Name = store.Name, // Return the store's updated name.
            Location = store.Location, // Return the store's updated location.
            CreatedAt = store.CreatedAt // Return the store's creation date (unchanged).
        };
    }

    // DeleteStoreAsync method handles deleting a store.
    public async Task DeleteStoreAsync(int storeId)
    {
        // Find the store by its ID. If not found, throw a KeyNotFoundException.
        var store = await _context.Stores.FindAsync(storeId);
        if (store == null) throw new KeyNotFoundException("Store not found");

        // Remove the store from the context.
        _context.Stores.Remove(store);

        // Save the changes to the database asynchronously.
        await _context.SaveChangesAsync();
    }
}

}