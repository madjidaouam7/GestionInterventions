using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Commands.UpdateClient;

public record UpdateClientCommand(
    int Id,
    string Email,
    string Telephone,
    string Adresse
) : IRequest<ClientDto?>;