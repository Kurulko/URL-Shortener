using Microsoft.AspNetCore.Identity;

namespace URL_ShortenerAPI.Extentions;

public static class IdentityResultExtentions
{
    public static IdentityResult Failed(Exception ex)
        => IdentityResult.Failed(
                new IdentityError() { Description = ex.Message }
            );

    public static IdentityResult Failed(string message)
        => IdentityResult.Failed(
                new IdentityError() { Description = message }
            );
}
