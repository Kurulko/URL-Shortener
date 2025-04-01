using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Data.Results;
using URL_ShortenerAPI.Exceptions;
using URL_ShortenerAPI.Repositories;
using URL_ShortenerAPI.Services.UrlShortServices;
using URL_ShortenerAPI.Services;
using System.Text;
using System.Security.Cryptography;

namespace URL_ShortenerAPI.Services.ShortUrlServices;

public class ShortUrlService : BaseService<ShortUrl>, IShortUrlService
{
    readonly ShortUrlRepository shortUrlRepository;
    public ShortUrlService(ShortUrlRepository shortUrlRepository)
    {
        this.shortUrlRepository = shortUrlRepository;
    }

    readonly InvalidIDException invalidShortUrlIDException = new InvalidIDException(nameof(ShortUrl));
    readonly ArgumentNullOrEmptyException shortCodeIsNullOrEmptyException = new ArgumentNullOrEmptyException("Short code");

    NotFoundException ShortUrlNotFoundByIDException(long id)
       => NotFoundException.NotFoundExceptionByID(nameof(ShortUrl), id);
    NotFoundException ShortUrlNotFoundByShortCodeException(string shortCode)
        => new NotFoundException(nameof(ShortUrl), $"{nameof(ShortUrl)} (Short code '{shortCode}') not found.");

    public async Task<string> ShortenUrlAsync(string originalUrl, string userId)
    {
        var shortCode = await GenerateUniqueShortCodeAsync(originalUrl);
        var shortUrl = new ShortUrl
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedDate = DateTime.Now,
            UserId = userId
        };

        await shortUrlRepository.AddAsync(shortUrl);
        return shortCode;
    }

    async Task<string> GenerateUniqueShortCodeAsync(string originalUrl)
    {
        string shortCode;

        const int shortCodeLength = 8;
        do
        {
            shortCode = Guid.NewGuid().ToString("N")[..shortCodeLength];
        }
        while (await shortUrlRepository.GetByShortCodeAsync(shortCode) is not null);

        return shortCode;
    }

    public async Task<string?> ResolveUrlAsync(string shortCode)
    {
        var shortUrl = await shortUrlRepository.GetByShortCodeAsync(shortCode);
        return shortUrl?.OriginalUrl;
    }

    public async Task<bool> TrackUrlClicksAsync(string shortCode)
    {
        var shortUrl = await shortUrlRepository.GetByShortCodeAsync(shortCode);

        if (shortUrl == null) 
            return false;

        shortUrl.ClickCount++;
        await shortUrlRepository.SaveChangesAsync();
        return true;
    }


    public async Task<ServiceResult<ShortUrl>> GetShortUrlByIdAsync(long id)
    {
        if (id < 1)
            return ServiceResult<ShortUrl>.Fail(invalidShortUrlIDException);

        try
        {
            var shortUrlById = await shortUrlRepository.GetByIdAsync(id);
            return ServiceResult<ShortUrl>.Ok(shortUrlById);
        }
        catch
        {
            return ServiceResult<ShortUrl>.Fail(FailedToActionStr("short url", "get"));
        }
    }

    public async Task<ServiceResult<ShortUrl>> GetShortUrlByShortCodeAsync(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return ServiceResult<ShortUrl>.Fail(shortCodeIsNullOrEmptyException);

        try
        {
            var shortUrlByShortCode = await shortUrlRepository.GetByShortCodeAsync(shortCode);
            return ServiceResult<ShortUrl>.Ok(shortUrlByShortCode);
        }
        catch
        {
            return ServiceResult<ShortUrl>.Fail(FailedToActionStr("short url by short code", "get"));
        }
    }

    public async Task<ServiceResult<IQueryable<ShortUrl>>> GetShortUrlsAsync()
    {
        try
        {
            var shortUrls = await shortUrlRepository.GetAllAsync();
            return ServiceResult<IQueryable<ShortUrl>>.Ok(shortUrls);
        }
        catch
        {
            return ServiceResult<IQueryable<ShortUrl>>.Fail(FailedToActionStr("short urls", "get"));
        }
    }

    public async Task<ServiceResult> DeleteShortUrlAsync(long id)
    {
        if (id < 1)
            return ServiceResult.Fail(invalidShortUrlIDException);

        ShortUrl? shortUrl = await shortUrlRepository.GetByIdAsync(id);

        if (shortUrl is null)
            return ServiceResult.Fail(ShortUrlNotFoundByIDException(id));

        try
        {
            await shortUrlRepository.RemoveAsync(id);
            return ServiceResult.Ok();
        }
        catch
        {
            return ServiceResult.Fail(FailedToActionStr("short url", "delete"));
        }
    }

    public async Task<ServiceResult> DeleteShortUrlByShortCodeAsync(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return ServiceResult.Fail(shortCodeIsNullOrEmptyException);

        ShortUrl? shortUrl = await shortUrlRepository.GetByShortCodeAsync(shortCode);

        if (shortUrl is null)
            return ServiceResult.Fail(ShortUrlNotFoundByShortCodeException(shortCode));

        try
        {
            await shortUrlRepository.RemoveByShortCodeAsync(shortCode);
            return ServiceResult.Ok();
        }
        catch
        {
            return ServiceResult.Fail(FailedToActionStr("short url", "delete"));
        }
    }

    public async Task<bool> IsOriginalUrlUniqueAsync(string originalUrl)
    {
        var isThereOriginalUrl = await shortUrlRepository.AnyAsync(s => s.OriginalUrl == originalUrl);
        return !isThereOriginalUrl;
    }
}
