using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestionInterventions.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Client", "Technicien", "Responsable" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var responsableEmail = configuration["SeedAdmin:Email"]
            ?? throw new InvalidOperationException("SeedAdmin:Email manquant dans la configuration.");
        var responsablePassword = configuration["SeedAdmin:Password"]
            ?? throw new InvalidOperationException("SeedAdmin:Password manquant dans la configuration.");

        var existingResponsable = await userManager.FindByEmailAsync(responsableEmail);

        if (existingResponsable is null)
        {
            var responsable = new ApplicationUser
            {
                UserName = responsableEmail,
                Email = responsableEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(responsable, responsablePassword);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(responsable, "Responsable");
        }
    }
}