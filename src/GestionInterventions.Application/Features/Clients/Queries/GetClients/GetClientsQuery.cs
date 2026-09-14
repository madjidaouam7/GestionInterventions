using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Queries.GetClients;

public record GetClientsQuery : IRequest<List<ClientDto>>;