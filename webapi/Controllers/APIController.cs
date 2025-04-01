using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using URL_ShortenerAPI.Exceptions;
using URL_ShortenerAPI.Data.Results;

namespace URL_ShortenerAPI.Controllers;

[ApiController]
[Route($"api/[controller]")]
public abstract class APIController : Controller
{
    protected ActionResult InvalidPageIndexOrPageSize()
        => BadRequest("Invalid page index or page size.");
    protected ActionResult InvalidEntryID(string entryName)
        => BadRequest($"Invalid {entryName} ID.");
    protected ActionResult EntryIsNull(string entryName)
        => BadRequest($"{entryName} entry is null.");
    protected ActionResult EntryNotFound(string entryName)
        => NotFound($"{entryName} not found.");

    protected ActionResult HandleException(Exception ex)
    {
        switch (ex)
        {
            case SecurityTokenException or UnauthorizedAccessException:
                return Unauthorized(ex.Message); // 401 Unauthorized

            case ArgumentException:
                return BadRequest(ex.Message); // 400 Bad Request

            case NotFoundException:
                return NotFound(ex.Message); // 404 Not Found

            default:
                // For any unexpected exception, return a 500 Internal Server Error
                return StatusCode(500, $"Internal server error: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    protected IActionResult HandleServiceResult(ServiceResult serviceResult)
    {
        if (serviceResult.Success)
            return Ok();

        return BadRequest(serviceResult.ErrorMessage);
    }

    protected ActionResult<T> HandleServiceResult<T>(ServiceResult<T> serviceResult, string? notFoundMessage = null)
        where T : class
    {
        if (!serviceResult.Success)
            return BadRequest(serviceResult.ErrorMessage);

        if (serviceResult.Model is T model)
            return model;

        return NotFound(notFoundMessage);
    }
}