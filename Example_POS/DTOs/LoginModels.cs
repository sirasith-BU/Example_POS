using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Example_POS.DTOs
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        //[DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(10, MinimumLength =10, ErrorMessage = "Password must be exactly 10 characters.")]
        //[DisplayName("Password")]
        public string Password { get; set; }
    }

}
