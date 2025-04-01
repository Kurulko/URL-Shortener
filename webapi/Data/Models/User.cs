using Microsoft.AspNetCore.Identity;

namespace URL_ShortenerAPI.Data.Models;

public class User : IdentityUser
{
    public DateTime Registered { get; set; }

    public IEnumerable<ShortUrl>? ShortUrls { get; set; }
}
