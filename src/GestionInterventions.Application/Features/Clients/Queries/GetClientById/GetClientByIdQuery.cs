using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Queries.GetClientById;

public record GetClientByIdQuery(int Id) : IRequest<ClientDto?>;
