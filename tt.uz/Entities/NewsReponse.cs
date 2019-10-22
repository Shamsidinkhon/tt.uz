using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class NewsReponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public Price Price { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public ContactDetail ContactDetail { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
