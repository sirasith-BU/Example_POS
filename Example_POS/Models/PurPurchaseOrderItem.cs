using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class PurPurchaseOrderItem : BaseEntity
    {
        [Key]
        public int PurchaseOrderItemId { get; set; }
        [Required]
        public int PurPurchaseOrderId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int ProductCode { get; set; }
        public int ProductName { get; set; }
        [Required]
        public decimal Qty { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        public PurPurchaseOrder PurPurchaseOrder { get; set; }
    }
}
