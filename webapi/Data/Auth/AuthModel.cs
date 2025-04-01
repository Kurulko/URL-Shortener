using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using URL_ShortenerAPI.Data.Models;

namespace URL_ShortenerAPI.Data.Auth;

public class AuthModel
{
    [Required(ErrorMessage = "Please, enter your name!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Please, enter your password!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "{0} must be at least {1} characters long.")]
    public string Password { get; set; } = null!;

    [JsonPropertyName("rememberme")]
    public bool RememberMe { get; set; }

    public static explicit operator User(AuthModel authModel)
        => new() { UserName = authModel.Name };
}
