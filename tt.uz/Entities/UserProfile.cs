using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class UserProfile
    {
        public UserProfile()
        {
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FacebookId { get; set; }
        public string Longtitude { get; set; }
        public string Latitude { get; set; }
        public int RegionId { get; set; }
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
