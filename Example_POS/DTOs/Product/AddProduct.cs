namespace Example_POS.DTOs.Product
{
    public class AddProduct
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
