using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponseDto>;
