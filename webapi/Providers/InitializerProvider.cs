using URL_ShortenerAPI.Data;
using URL_ShortenerAPI.Data.Auth;
using URL_ShortenerAPI.Initializers;
using URL_ShortenerAPI.Repositories;

namespace URL_ShortenerAPI.Providers;

public static class InitializerProvider
{
    public static async Task InitializeDataAsync(this WebApplication app, ConfigurationManager config)
    {
        using IServiceScope serviceScope = app.Services.CreateScope();

        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

        var roleRepository = serviceProvider.GetRequiredService<RoleRepository>();
        var userRepository = serviceProvider.GetRequiredService<UserRepository>();

        var isAnyRoles = await roleRepository.AnyRolesAsync();
        if (!isAnyRoles)
            await InitializeRolesAsync(roleRepository);

        var isAnyUsers = await userRepository.AnyUsersAsync();
        if (!isAnyUsers)
            await InitializeUsersAsync(userRepository, config);
    }

    static async Task InitializeRolesAsync(RoleRepository roleRepository)
        => await RolesInitializer.InitializeAsync(roleRepository, Roles.AdminRole, Roles.UserRole);

    static async Task InitializeUsersAsync(UserRepository userRepository, ConfigurationManager config)
    {
        string adminName = config.GetValue<string>("Admin:Name")!;
        string adminPassword = config.GetValue<string>("Admin:Password")!;

        var adminModel = new LoginModel() { Name = adminName, Password = adminPassword };
        var roles = new[] { Roles.AdminRole, Roles.UserRole };

        await UsersInitializer.InitializeAsync(userRepository, adminModel, roles);
    }
}
