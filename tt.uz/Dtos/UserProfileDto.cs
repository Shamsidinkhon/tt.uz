using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Dtos
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public int RegionId { get; set; }
        public int DistrictId { get; set; }
        public int ImageId { get; set; }
    }
}
