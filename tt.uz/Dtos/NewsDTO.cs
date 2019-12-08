using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Dtos
{
    public class NewsDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Price Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Location Location { get; set; }
        [Required]
        public ContactDetail ContactDetail { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string ImageIds { get; set; }
        public List<NewsAttribute> NewsAttribute { get; set; }
    }
}
