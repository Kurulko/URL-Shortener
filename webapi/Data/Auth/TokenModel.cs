namespace URL_ShortenerAPI.Data.Auth;

public class TokenModel
{
    public string TokenStr { get; set; } = null!;
    public int ExpirationDays { get; set; }
    public string[] Roles { get; set; } = null!;
}