using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tt.uz.Helpers;
namespace tt.uz.Entities
{
    public class LegalEntity
    {
        public LegalEntity()
        {
            CreatedDate = DateHelper.GetDate();
            UpdatedDate = DateHelper.GetDate();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public int Region { get; set; }
        public int District { get; set; }
        public string Phone { get; set; }
        public int Logo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [NotMapped]
        public Image LogoImage { get; set; }
    }
}