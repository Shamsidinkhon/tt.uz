using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tt.admin.Models
{
    public class CategoryPostModel
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int TypeId { get; set; }
    }
}
