using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class Region
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Lang { get; set; }
        public string Title { get; set; }
        public string Label
        {
            get
            {
                return Title;
            }
        }
        public int ParentId { get; set; }
        public int Value
        {
            get
            {
                return ParentId;
            }
        }
        public int SoatoId { get; set; }
        public int Depth { get; set; }
        [DefaultValue(1)]
        public int Available { get; set; }
        [NotMapped]
        public List<Region> Children { get; set; }
    }
}
