using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URL_ShortenerAPI.Data.Models;

namespace URL_ShortenerAPI.Data.Context;

public class URLShortenerContext : IdentityDbContext<User>
{
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();

    public URLShortenerContext(DbContextOptions options) : base(options)
        => Database.EnsureCreated();
}
