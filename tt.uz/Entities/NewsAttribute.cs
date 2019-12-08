using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class NewsAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NewsId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
        [NotMapped]
        public CoreAttribute AttributeInfo { get; set; }
    }
}
