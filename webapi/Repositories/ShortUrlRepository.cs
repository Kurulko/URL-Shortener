using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using URL_ShortenerAPI.Data.Context;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Exceptions;

namespace URL_ShortenerAPI.Repositories;

public class ShortUrlRepository : DbModelRepository<ShortUrl>
{
    public ShortUrlRepository(URLShortenerContext db) : base(db)
    {

    }

    public virtual async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
    {
        var models = await GetAllAsync();
        return await models.SingleOrDefaultAsync(m => m.ShortCode == shortCode);
    }
    public virtual async Task RemoveByShortCodeAsync(string shortCode)
    {
        var model = await GetByShortCodeAsync(shortCode) ?? throw new NotFoundException(nameof(ShortUrl), $"{nameof(ShortUrl)} (Short code '{shortCode}') not found."); ;

        dbSet.Remove(model);
        await SaveChangesAsync();
    }
}