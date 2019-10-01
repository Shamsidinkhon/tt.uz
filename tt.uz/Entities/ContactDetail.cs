using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class ContactDetail
    {
        public ContactDetail() {
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
            IsIndividual = true;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactDetailId { get; set; }
        public string Name { get; set; }
        public bool IsIndividual { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
