using MediatR;

namespace GestionInterventions.Application.Features.Clients.Commands.RegisterClient;

public record RegisterClientCommand(
    string Email,
    string Password,
    string Nom,
    string Telephone,
    string Adresse
) : IRequest<int>;