using GestionInterventions.Application.Common.Interfaces;
using System.Security.Claims;

namespace GestionInterventions.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public int? ClientId =>
        int.TryParse(User?.FindFirst("ClientId")?.Value, out var id) ? id : null;

    public int? TechnicienId =>
        int.TryParse(User?.FindFirst("TechnicienId")?.Value, out var id) ? id : null;
}
