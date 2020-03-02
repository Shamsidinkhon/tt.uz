using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class News
    {
        public const int NEW = 1;
        public const int ACTIVE = 2;
        public const int REJECTED = 3;
        public const int ARCHIVE = 4;
        public const int DELETED = 5;
        public News() {
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
            Status = NEW;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int PriceId { get; set; }
        public Price Price { get; set; }

        public string Description { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public int ContactDetailId { get; set; }
        public ContactDetail ContactDetail { get; set; }

        public int OwnerId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [NotMapped]
        public List<Image> Images { get; set; }
        [NotMapped]
        public bool Favourite { get; set; }
        [NotMapped]
        public bool VendorFavourite { get; set; }
        [NotMapped]
        public UserProfile OwnerDetails { get; set; }
        [NotMapped]
        public List<NewsAttribute> NewsAttribute { get; set; }
        [NotMapped]
        public List<Tariff> Tariffs { get; set; }
    }
}
