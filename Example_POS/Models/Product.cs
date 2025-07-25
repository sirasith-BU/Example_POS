using System.ComponentModel.DataAnnotations;
    
namespace Example_POS.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
