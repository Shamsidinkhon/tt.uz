using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class CoreAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        [NotMapped]
        public string Label { get { return Title; } }
        public int Type { get; set; }
        public string Unit { get; set; }
        public bool Required { get; set; }
        [NotMapped]
        public List<AttributeOption> AttributeOptions { get; set; }
    }
}
