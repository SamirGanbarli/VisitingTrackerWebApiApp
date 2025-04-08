using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class Visits
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int StoreId { get; set; }
    [Required]
    public DateTime VisitDate { get; set; }
    [Required]
    public string Status { get; set; } = string.Empty;
}
}