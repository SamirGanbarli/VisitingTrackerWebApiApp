using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Photos
    {
    [Key]
    public int Id { get; set; }
    [Required]
    public int VisitId { get; set; }
    [Required]
    public int ProductId { get; set; } 
    [Required]
    public string Base64Image { get; set; }  = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
}