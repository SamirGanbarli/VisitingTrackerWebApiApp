using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class VisitDto
    {
        
    }
    public class VisitRequestDto
    {
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

     public class VisitResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StoreId { get; set; }
        public DateTime VisitDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}