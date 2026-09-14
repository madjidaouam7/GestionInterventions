using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Queries.GetClientById;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetClientByIdQueryHandler(IClientRepository clientRepository, ICurrentUserService currentUserService)
    {
        _clientRepository = clientRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ClientDto?> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (client == null)
        {
            throw new NotFoundException($"Aucun client trouvé avec l'id {request.Id}.");
        }

        // Un Client ne peut consulter que son propre profil
        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != client.Id)
        {
            throw new ForbiddenAccessException("Vous ne pouvez consulter que votre propre profil.");
        }

        return new ClientDto(client);

    }
}