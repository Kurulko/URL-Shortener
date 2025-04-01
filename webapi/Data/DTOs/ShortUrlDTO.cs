using URL_ShortenerAPI.Data.Models;

namespace URL_ShortenerAPI.Data.DTOs;

public class ShortUrlDTO
{
    public long Id { get; set; }
    public string OriginalUrl { get; set; } = null!;
    public string ShortCode { get; set; } = null!;
    public int ClickCount { get; set; }
    public DateTime CreatedDate { get; set; }

    public bool IsCreatedByYou { get; set; }
}
