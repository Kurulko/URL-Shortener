using Microsoft.AspNetCore.Identity;
using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Repositories;

namespace URL_ShortenerAPI.Initializers;

public static class UsersInitializer
{
    public static async Task<User> InitializeAsync(UserRepository userRepository, LoginModel loginModel, params string[] rolesStr)
    {
        User? user = await userRepository.GetUserByUsernameAsync(loginModel.Name);

        if (user is null)
        {
            user = (User)loginModel;
            user.Registered = DateTime.Now;

            IdentityResult result = await userRepository.CreateUserAsync(user, loginModel.Password);

            if (result.Succeeded)
            {
                await userRepository.AddRolesToUserAsync(user.Id, rolesStr);
            }
        }

        return user;
    }
}
