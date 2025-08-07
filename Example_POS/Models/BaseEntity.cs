using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class BaseEntity
    {
        public int? CreateBy { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        public int? UpdateBy { get; set; }
        [Required]
        public DateTime UpdateTime { get; set; }
        public int? IsDelete { get; set; }
    }
}
