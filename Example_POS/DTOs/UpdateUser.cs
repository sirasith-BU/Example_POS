using System.ComponentModel.DataAnnotations;

namespace Example_POS.DTOs
{
    public class UpdateUser
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
    }
}
