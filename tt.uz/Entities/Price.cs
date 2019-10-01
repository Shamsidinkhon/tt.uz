using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class Price
    {
        public Price()
        {
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
            Exchange = false;
            Free = false;
            Negotiable = false;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriceId { get; set; }
        public decimal Amount { get; set; }
        public int Currency { get; set; }
        public bool Exchange { get; set; }
        public bool Free { get; set; }
        public bool Negotiable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
