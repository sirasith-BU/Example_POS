namespace Example_POS.DTOs.Token
{
    public class TokenResponseDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required DateTime? RefreshTokenExpiredDate { get; set; }
    }
}
