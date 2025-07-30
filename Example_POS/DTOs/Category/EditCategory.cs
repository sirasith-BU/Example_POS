namespace Example_POS.DTOs.Category
{
    public class EditCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Delete { get; set; }
    }
}
