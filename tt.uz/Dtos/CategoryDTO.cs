﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.uz.Dtos
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public int? Sort { get; set; }
        public int Status { get; set; }
        public string Icon { get; set; }
        public IEnumerable<CategoryDTO> Children { get; set; }
    }
}
