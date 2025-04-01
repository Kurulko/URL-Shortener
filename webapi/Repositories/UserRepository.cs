using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URL_ShortenerAPI.Data.Context;
using URL_ShortenerAPI.Data.Models;
using URL_ShortenerAPI.Exceptions;
using URL_ShortenerAPI.Extentions;

namespace URL_ShortenerAPI.Repositories;

public class UserRepository
{
    readonly UserManager<User> userManager;
    public UserRepository(UserManager<User> userManager)
        => this.userManager = userManager;

    IQueryable<User> Users => userManager.Users;

    static IdentityResult UserNotFoundByIDResult(string userId)
        => IdentityResultExtentions.Failed($"User (ID '{userId}') not found.");
    static IdentityResult UserNotFoundByNameResult(string name)
        => IdentityResultExtentions.Failed($"User (Name '{name}') not found.");

    static IdentityResult UserIDIsNullOrEmptyResult => IdentityResultExtentions.Failed("User ID cannot not be null or empty.");

    #region CRUD

    public virtual async Task<User> AddUserAsync(User user)
    {
        User? existingUser = await GetUserByIdAsync(user.Id);

        if (existingUser is null)
        {
            if (await UserExistsByUsernameAsync(user.UserName!))
                throw new DbUpdateException("User name must be unique.");

            await userManager.CreateAsync(user);
            return user;
        }

        return existingUser;
    }

    public virtual async Task<IdentityResult> CreateUserAsync(User user, string password)
        => await userManager.CreateAsync(user, password);

    public virtual async Task<IdentityResult> UpdateUserAsync(User user)
    {
        if (string.IsNullOrEmpty(user.Id))
            return UserIDIsNullOrEmptyResult;

        User? existingUser = await GetUserByIdAsync(user.Id);

        if (existingUser is not null)
        {
            if (existingUser.UserName != user.UserName)
                await userManager.SetUserNameAsync(existingUser, user.UserName);

            existingUser.Registered = user.Registered;
            return await userManager.UpdateAsync(existingUser);
        }

        return UserNotFoundByIDResult(user.Id);
    }

    public virtual async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return UserIDIsNullOrEmptyResult;

        User? user = await GetUserByIdAsync(userId);

        if (user is not null)
            return await userManager.DeleteAsync(user);

        return UserNotFoundByIDResult(userId);
    }

    public virtual async Task<User?> GetUserByUsernameAsync(string userName)
        => await userManager.FindByNameAsync(userName);
    public virtual async Task<User?> GetUserByEmailAsync(string email)
        => await userManager.FindByEmailAsync(email);

    public virtual async Task<IQueryable<User>> GetUsersAsync()
        => await Task.FromResult(Users);

    public virtual async Task<User?> GetUserByIdAsync(string userId)
        => await userManager.FindByIdAsync(userId);

    public virtual async Task<bool> AnyUsersAsync()
        => await Users.AnyAsync();

    public virtual async Task<bool> UserExistsAsync(string userId)
        => await Users.AnyAsync(u => u.Id == userId);

    public virtual async Task<bool> UserExistsByUsernameAsync(string userName)
        => await Users.AnyAsync(r => r.UserName!.ToLower() == userName.ToLower());

    #endregion


    #region Roles

    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        User? user = await GetUserByIdAsync(userId);

        if (user is null)
            throw NotFoundException.NotFoundExceptionByID(nameof(User), userId);

        return await userManager.GetRolesAsync(user);
    }

    public virtual async Task<IdentityResult> AddRolesToUserAsync(string userId, string[] roles)
    {
        User? user = await GetUserByIdAsync(userId);

        if (user is null)
            return UserNotFoundByIDResult(userId);

        return await userManager.AddToRolesAsync(user, roles);
    }

    public virtual async Task<IdentityResult> DeleteRoleFromUserAsync(string userId, string roleName)
    {
        User? user = await GetUserByIdAsync(userId);

        if (user is null)
            return UserNotFoundByIDResult(userId);

        return await userManager.RemoveFromRoleAsync(user, roleName);
    }

    #endregion
}
