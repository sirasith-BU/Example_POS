using System.ComponentModel.DataAnnotations;

namespace Example_POS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? Salt { get; set; }

        public string? JwtToken { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public int CreateBy { get; set; }

        public DateTime CreateTime { get; set; }

        public int UpdateBy { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }
    }
}
