using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using tt.uz.Helpers;

namespace tt.uz.Entities
{
    public class VerificationCode
    {
        public const string EMAIL = "email";
        public const string PHONE = "phone";
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
        public int Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public VerificationCode()
        {
            this.Code = new Random().Next(10000, 99999);
            this.CreatedDate = DateHelper.GetDate();
        }

    }
}
