using Microsoft.EntityFrameworkCore;

namespace URL_ShortenerAPI.Data.Models;

[Index(nameof(ShortCode), IsUnique = true)]
[Index(nameof(OriginalUrl), IsUnique = true)]
public class ShortUrl : IDbModel
{
    public long Id { get; set; }
    public string OriginalUrl { get; set; } = null!;
    public string ShortCode { get; set; } = null!;
    public int ClickCount { get; set; }
    public DateTime CreatedDate { get; set; }

    public string UserId { get; set; } = null!;
}
