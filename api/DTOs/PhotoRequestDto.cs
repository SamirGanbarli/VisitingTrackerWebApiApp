using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs
{
    public class PhotoRequestDto
    {
        public int ProductId { get; set; }
        public string Base64Image { get; set; } = string.Empty;
    }
}