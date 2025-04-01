using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URL_ShortenerAPI.Data;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Extentions;
using URL_ShortenerAPI.Services.UrlShortServices;
using URL_ShortenerAPI.Data.DTOs;
using URL_ShortenerAPI.Data.Results;

namespace URL_ShortenerAPI.Controllers;

[Route("api/short-urls")]
public class ShortUrlsController : DbModelController
{
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IShortUrlService shortUrlService;
    public ShortUrlsController(IMapper mapper, IHttpContextAccessor httpContextAccessor, IShortUrlService shortUrlService)
        : base(mapper)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.shortUrlService = shortUrlService;
    }

    [HttpPost("shorten")]
    [Authorize]
    public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest shortenUrlRequest)
    {
        try
        {
            string userId = httpContextAccessor.GetUserId()!;
            var shortCode = await shortUrlService.ShortenUrlAsync(shortenUrlRequest.OriginalUrl, userId);
            return Ok(new { ShortCode = shortCode });
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("resolve/{shortCode}")]
    public async Task<IActionResult> ResolveUrl(string shortCode)
    {
        try
        {
            var originalUrl = await shortUrlService.ResolveUrlAsync(shortCode);

            if (originalUrl == null)
                return NotFound();

            await shortUrlService.TrackUrlClicksAsync(shortCode);
            return Redirect(originalUrl);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<ShortUrlDTO>>> GetShortUrlsAsync(
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortOrder = null,
        [FromQuery] string? filterColumn = null,
        [FromQuery] string? filterQuery = null)
    {
        if (pageIndex < 0 || pageSize <= 0)
            return InvalidPageIndexOrPageSize();

        try
        {
            var serviceResult = await shortUrlService.GetShortUrlsAsync();

            if (!serviceResult.Success)
                return BadRequest(serviceResult.ErrorMessage);

            if (serviceResult.Model is not IQueryable<ShortUrl> shortUrls)
                return EntryNotFound("Short urls");

            var shortUrlDTOs = shortUrls.AsEnumerable().Select(u => mapper.Map<ShortUrlDTO>(u));
            return await ApiResult<ShortUrlDTO>.CreateAsync(
                shortUrlDTOs.AsQueryable(),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery
            );
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetShortUrlByIdAsync))]
    [Authorize]
    public async Task<ActionResult<ShortUrlDTO>> GetShortUrlByIdAsync(long id)
    {
        if (id < 1)
            return InvalidShortUrlID();

        var serviceResult = await shortUrlService.GetShortUrlByIdAsync(id);
        return HandleShortUrlDTOServiceResult(serviceResult);
    }

    [HttpGet("by-shortcode/{shortCode}")]
    [Authorize]
    public async Task<ActionResult<ShortUrlDTO>> GetShortUrlByNameAsync(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return ShortCodeIsNullOrEmpty();

        var serviceResult = await shortUrlService.GetShortUrlByShortCodeAsync(shortCode);
        return HandleShortUrlDTOServiceResult(serviceResult);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteShortUrlAsync(long id)
    {
        if (id < 1)
            return InvalidShortUrlID();

        var isUserAllowedDeleteShortUrl = await IsUserAllowedDeleteShortUrlAsync(id);
        if (!isUserAllowedDeleteShortUrl)
            return UserNotHavePermissionDeleteItem();

        var serviceResult = await shortUrlService.DeleteShortUrlAsync(id);
        return HandleServiceResult(serviceResult);
    }

    [HttpDelete("by-shortcode/{shortCode}")]
    [Authorize]
    public async Task<IActionResult> DeleteShortUrlAsync(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return ShortCodeIsNullOrEmpty();

        var isUserAllowedDeleteShortUrl = await IsUserAllowedDeleteShortUrlAsync(shortCode);
        if (!isUserAllowedDeleteShortUrl)
            return UserNotHavePermissionDeleteItem();

        var serviceResult = await shortUrlService.DeleteShortUrlByShortCodeAsync(shortCode);
        return HandleServiceResult(serviceResult);
    }

    [HttpGet("is-unique")]
    [Authorize]
    public async Task<ActionResult<bool>> IsOriginalUrlUniqueAsync([FromQuery]string originalUrl)
    {
        if (string.IsNullOrEmpty(originalUrl))
            return OriginalUrlIsNullOrEmpty();

        var isOriginalUrlUnique = await shortUrlService.IsOriginalUrlUniqueAsync(originalUrl);
        return isOriginalUrlUnique;
    }

    async Task<bool> IsUserAllowedDeleteShortUrlAsync(long id)
    {
        var serviceResult = await shortUrlService.GetShortUrlByIdAsync(id);
        return IsUserAllowedDeleteShortUrl(serviceResult);
    }

    async Task<bool> IsUserAllowedDeleteShortUrlAsync(string shortCode)
    {
        var serviceResult = await shortUrlService.GetShortUrlByShortCodeAsync(shortCode);
        return IsUserAllowedDeleteShortUrl(serviceResult);
    }

    bool IsUserAllowedDeleteShortUrl(ServiceResult<ShortUrl> serviceResult)
    {
        if (!serviceResult.Success || serviceResult.Model is not ShortUrl shortUrl)
            return false;

        bool isAdmin = IsAdmin();
        if (isAdmin)
            return true;

        var userId = httpContextAccessor.GetUserId()!;
        bool isItCurrentUser = shortUrl.UserId == userId;
        return isItCurrentUser;
    }

    bool IsAdmin()
        => User.IsInRole(Roles.AdminRole);

    ActionResult InvalidShortUrlID()
       => InvalidEntryID(nameof(ShortUrl));
    ActionResult ShortCodeIsNullOrEmpty()
       => BadRequest($"Short code is null or empty.");
    ActionResult OriginalUrlIsNullOrEmpty()
       => BadRequest($"Original Url is null or empty.");
    ActionResult UserNotHavePermissionDeleteItem()
       => Unauthorized("User does not have permission to delete this item.");
    ActionResult ShortUrlIsNull()
        => EntryIsNull(nameof(ShortUrl));

    ActionResult<ShortUrlDTO> HandleShortUrlDTOServiceResult(ServiceResult<ShortUrl> serviceResult)
        => HandleDTOServiceResult<ShortUrl, ShortUrlDTO>(serviceResult, "Short url not found.");
}
