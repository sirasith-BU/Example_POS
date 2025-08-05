using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Example_POS.DTOs.User
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        //[DisplayName("Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Password must be exactly 6 characters.")]
        //[DisplayName("Password")]
        public required string Password { get; set; }
    }
}
