using Microsoft.AspNetCore.Identity;
using URL_ShortenerAPI.Repositories;

namespace URL_ShortenerAPI.Initializers;

public class RolesInitializer
{
    public static async Task InitializeAsync(RoleRepository roleRepository, params string[] rolesStr)
    {
        foreach (string roleStr in rolesStr)
        {
            IdentityRole? role = await roleRepository.GetRoleByNameAsync(roleStr);
            if (role is null)
                await roleRepository.AddRoleAsync(new IdentityRole(roleStr));
        }
    }
}