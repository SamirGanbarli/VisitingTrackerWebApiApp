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
    public class VisitService
    {
        private readonly AppDbContext _context;

        public VisitService(AppDbContext context)
        {
            _context = context;
        }

        // Get visits for the logged-in user (or all visits if Admin)
        public async Task<List<DTOs.VisitResponseDto>> GetVisitsAsync(Users user, int page, int pageSize)
        {
            var query = _context.Visits
                .AsQueryable();

            if (user.Role != "Admin")
            {
                query = query.Where(v => v.UserId == user.Id);
            }

            var visits = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new DTOs.VisitResponseDto
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    StoreId = v.StoreId,
                    VisitDate = v.VisitDate,
                    Status = v.Status
                })
                .ToListAsync();

            return visits;
        }

        // Create a new visit
        public async Task<DTOs.VisitResponseDto> CreateVisitAsync(VisitRequestDto visitRequest, int userId)
        {
            var visit = new api.Models.Visits
            {
                UserId = userId,
                StoreId = visitRequest.StoreId,
                VisitDate = visitRequest.VisitDate,
                Status = visitRequest.Status
            };

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();

            return new DTOs.VisitResponseDto
            {
                Id = visit.Id,
                UserId = visit.UserId,
                StoreId = visit.StoreId,
                VisitDate = visit.VisitDate,
                Status = visit.Status
            };
        }

        // Mark a visit as completed
        public async Task<VisitResponseDto> CompleteVisitAsync(int visitId)
        {
            var visit = await _context.Visits
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null)
                throw new Exception("Visit not found.");

            visit.Status = "Completed";
            await _context.SaveChangesAsync();

            return new VisitResponseDto
            {
                Id = visit.Id,
                UserId = visit.UserId,
                StoreId = visit.StoreId,
                VisitDate = visit.VisitDate,
                Status = visit.Status
            };
        }

        public async Task<Photos> UploadPhotoAsync(int visitId, PhotoRequestDto photoUploadDTO)
        {
            // Check if the visit exists
            var visit = await _context.Visits.FindAsync(visitId);
            if (visit == null)
            {
                throw new KeyNotFoundException("Visit not found.");
            }

            // Create a new photo entity
            var photo = new Photos
            {
                VisitId = visitId,
                ProductId = photoUploadDTO.ProductId,
                Base64Image = photoUploadDTO.Base64Image,
                UploadedAt = DateTime.UtcNow
            };

            // Save the photo
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return photo;
        }
    }
}