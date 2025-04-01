using Microsoft.AspNetCore.Identity;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.Results;

namespace URL_ShortenerAPI.Services.UrlShortServices;

public interface IShortUrlService
{
    Task<string> ShortenUrlAsync(string originalUrl, string userId);
    Task<string?> ResolveUrlAsync(string shortCode);
    Task<bool> TrackUrlClicksAsync(string shortCode);

    Task<ServiceResult<ShortUrl>> GetShortUrlByIdAsync(long id);
    Task<ServiceResult<ShortUrl>> GetShortUrlByShortCodeAsync(string shortCode);
    Task<ServiceResult<IQueryable<ShortUrl>>> GetShortUrlsAsync();

    Task<ServiceResult> DeleteShortUrlAsync(long id);
    Task<ServiceResult> DeleteShortUrlByShortCodeAsync(string shortCode);

    Task<bool> IsOriginalUrlUniqueAsync(string originalUrl);
}
