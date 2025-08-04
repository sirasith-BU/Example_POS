using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class SuplierGroup
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Detail { get; set; }
        public string? Comment { get; set; }
        public int? Status { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime UpdateTime { get; set; }

    }
}
