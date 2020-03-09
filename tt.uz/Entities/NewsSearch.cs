using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class NewsSearch
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int OwnerId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }

        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public PriceSearchClass Price {get; set;}
        public LocationInnerClass Location {get; set;}
        public List<AttributeInnerClass> Attributes { get; set; }

        public class PriceSearchClass
        {
            public decimal AmountFrom { get; set; }
            public decimal AmountTo { get; set; }
            public int Currency { get; set; }
            public bool Exchange { get; set; }
            public bool Free { get; set; }
            public bool Negotiable { get; set; }

        }
        public class LocationInnerClass
        {
            public int RegionId { get; set; }
            public int DistrictId { get; set; }
            public string Longtitude { get; set; }
            public string Latitude { get; set; }
        }
        public class AttributeInnerClass
        {
            public int AttributeId { get; set; }
            public int ValueFrom { get; set; }
            public int ValueTo { get; set; }
            public string Value { get; set; }
        }
    }


}
