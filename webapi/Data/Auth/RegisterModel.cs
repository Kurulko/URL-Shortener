using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using URL_ShortenerAPI.Data.Models;

namespace URL_ShortenerAPI.Data.Auth;

public class RegisterModel : AuthModel
{
    [Required(ErrorMessage = "Please, repeat password!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least {1} characters long.")]
    [Compare("Password", ErrorMessage = "Passwords don't match.")]
    [JsonPropertyName("passwordconfirm")]
    public string PasswordConfirm { get; set; } = null!;
}
