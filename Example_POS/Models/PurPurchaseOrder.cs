using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class PurPurchaseOrder : BaseEntity
    {
        [Key]
        public int PurchaseOrderId { get; set; }
        [Required]
        public string PurchaseOrderNo { get; set; }
        [Required]
        public decimal ToTalPrice { get; set; }
        [Required]
        public decimal ToTalPriceFc { get; set; }
        public virtual ICollection<PurPurchaseOrderItem> PurPurchaseOrderItem { get; set; }

    }
}
