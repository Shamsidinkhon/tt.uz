using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Dtos
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int CategoryId { get; set; }

        public Price Price { get; set; }

        public string Description { get; set; }

        public Location Location { get; set; }

        public ContactDetail ContactDetail { get; set; }

        public int Status { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
