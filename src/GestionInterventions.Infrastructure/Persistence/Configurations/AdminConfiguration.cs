using GestionInterventions.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GestionInterventions.Infrastructure.Configuration;

public class AdminConfiguration : IAdminConfiguration
{
    public string Email { get; }

    public AdminConfiguration(IConfiguration configuration)
    {
        Email = configuration["SeedAdmin:Email"]
            ?? throw new InvalidOperationException(
                "L'email du Responsable n'est pas configuré.");
    }
}