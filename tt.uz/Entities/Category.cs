using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Slug { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescrition { get; set; }
        public string MetaKeywords { get; set; }
        public int Sort { get; set; }
        public int Status { get; set; }
        public string Icon { get; set; }

    }
}
