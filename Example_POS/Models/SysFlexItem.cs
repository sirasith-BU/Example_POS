using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class SysFlexItem : BaseEntity
    {
        [Key]
        public int FlexItemId { get; set; }
        [Required]
        public int FlexId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FlexItemCode { get; set; }

        [MaxLength(200)]
        public string FlexItemName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public SysFlex SysFlex { get; set; }
    }
}
