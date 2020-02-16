using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class Contact
    {
        public Contact()
        {
            CreatedDate = DateHelper.GetDate();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
