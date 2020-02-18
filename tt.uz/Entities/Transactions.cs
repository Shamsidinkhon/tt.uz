using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class Transactions
    {
        public Transactions()
        {
            CreateTime = DateHelper.GetDate();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string PaycomTransactionId { get; set; }
        [Required]
        public string PaycomTime { get; set; }
        [Required]
        public DateTime PaycomTimeDatetime { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        public DateTime PerformTime { get; set; }
        public DateTime CancelTime { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int State { get; set; }
        public int Reason { get; set; }
        public string Receivers { get; set; }
        [Required]
        public int UserId { get; set; }

    }
}
