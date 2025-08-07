using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    [Index(nameof(FlexCode), IsUnique = true)]
    public class SysFlex : BaseEntity
    {
        [Key]
        public int FlexId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FlexCode { get; set; }

        [MaxLength(200)]
        public string FlexName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<SysFlexItem> SysFlexItem { get; set; }
    }
}
