using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Queries.GetClients;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, List<ClientDto>>
{
    private readonly IClientRepository _clientRepository;

    public GetClientsQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {   

        var clients = await _clientRepository.GetAllAsync(cancellationToken);

        return clients.Select(client => new ClientDto(client))
                      .ToList();
    
    }
}