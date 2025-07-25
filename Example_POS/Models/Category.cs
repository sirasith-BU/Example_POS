using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public  string? Name { get; set; }
        public string? Description { get; set; }
        public int? CreateBy { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        public int? UpdateBy { get; set; }
        [Required]
        public DateTime UpdateTime { get; set; }
        public int? IsDelete { get; set; }
    }
}
