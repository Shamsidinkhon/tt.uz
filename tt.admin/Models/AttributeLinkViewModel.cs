using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Entities;

namespace tt.admin.Models
{
    public class AttributeLinkViewModel
    {
        public int Type { get; set; }
        public List<CoreAttribute> Attributes { get; set; }
        public List<Category> Categories { get; set; }
    }
}
