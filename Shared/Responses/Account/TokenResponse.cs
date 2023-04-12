namespace Shared.Responses.Account;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string UserImageUrl { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }
}