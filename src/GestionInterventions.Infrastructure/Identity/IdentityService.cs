using GestionInterventions.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace GestionInterventions.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, null, result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, role);

        return (true, user.Id, Array.Empty<string>());
    }



    public async Task DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
            await _userManager.DeleteAsync(user);
    }



    public async Task<(bool Succeeded, string? UserId, string? Email, string? Role)>
    LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return (false, null, null, null);

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordIsValid)
            return (false, null, null, null);

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault();

        return (
            true,
            user.Id,
            user.Email,
            role
        );
    }
}