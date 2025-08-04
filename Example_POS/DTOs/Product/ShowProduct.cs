using System.ComponentModel.DataAnnotations;

namespace Example_POS.DTOs.Product
{
    public class ShowProduct
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime UpdateTime { get; set; }
        public int? IsDelete { get; set; }
    }
}
