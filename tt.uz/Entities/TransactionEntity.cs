using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class TransactionEntity
    {
        public const int TIMEOUT = 43200000;
        public const int STATE_CREATED = 1;
        public const int STATE_COMPLETED = 2;
        public const int STATE_CANCELLED = -1;
        public const int STATE_CANCELLED_AFTER_COMPLETE = -2;

        public const int REASON_RECEIVERS_NOT_FOUND = 1;
        public const int REASON_PROCESSING_EXECUTION_FAILED = 2;
        public const int REASON_EXECUTION_FAILED = 3;
        public const int REASON_CANCELLED_BY_TIMEOUT = 4;
        public const int REASON_FUND_RETURNED = 5;
        public const int REASON_UNKNOWN = 10;

        public TransactionEntity()
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
        public bool isExpired()
        {
            return State == STATE_CREATED && Math.Abs(CreateTime.TimeOfDay.TotalMilliseconds * 1000 - DateHelper.GetDate().TimeOfDay.TotalMilliseconds * 1000) > TIMEOUT;
        }

    }

}
