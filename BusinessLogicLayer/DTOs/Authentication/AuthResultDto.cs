namespace BusinessLogicLayer.DTOs.Authentication
{
    public class AuthResultDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
    }
}
