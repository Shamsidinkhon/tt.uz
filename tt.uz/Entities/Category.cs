using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Entities
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int? ParentId { get; set; }
        [Required]
        public string Slug { get; set; }
        [Required]
        public string Name { get; set; }
        public string Image { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        [Required]
        public int? Sort { get; set; }
        [DefaultValue(1)]
        public int Status { get; set; }
        public string WebIcon { get; set; }
        public string MobileIcon { get; set; }
        public int AttributeType { get; set; }
        [NotMapped]
        public List<Category> Children { get; set; }
        [NotMapped]
        public int CountNews { get; set; }

    }
}
