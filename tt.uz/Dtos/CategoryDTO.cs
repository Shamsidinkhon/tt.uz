using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public int Value { get { return Id; } }
        public int? ParentId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Label { get { return Name; } }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? Sort { get; set; }
        public int Status { get; set; }
        public string WebIcon { get; set; }
        public string MobileIcon { get; set; }
        public IEnumerable<CategoryDTO> Children { get; set; }
    }
}
