using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Dtos
{
    public class LegalEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public int Region { get; set; }
        public int District { get; set; }
        public string Phone { get; set; }
        public string Logo { get; set; }
    }
}
