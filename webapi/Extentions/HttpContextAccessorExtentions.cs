using System.Security.Claims;

namespace URL_ShortenerAPI.Extentions;

public static class HttpContextAccessorExtentions
{
    public static string? GetUserId(this IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}