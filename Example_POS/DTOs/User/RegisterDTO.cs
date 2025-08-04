using System.ComponentModel.DataAnnotations;

namespace Example_POS.DTOs.User
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Password must be exactly 10 characters.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Re-Password is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Re-Password must be exactly 10 characters.")]
        public string? RePassword { get; set; }
    }
}
