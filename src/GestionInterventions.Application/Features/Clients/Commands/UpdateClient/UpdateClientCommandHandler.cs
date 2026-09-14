using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Commands.UpdateClient;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientDto?>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateClientCommandHandler(IClientRepository clientRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ClientDto?> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {

        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);

        if (client is null)
            throw new NotFoundException($"Aucun client trouvé avec l'id {request.Id}.");

        // Un Client ne peut modifer que son propre profil
        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != client.Id)
        {
            throw new ForbiddenAccessException("Vous ne pouvez modifier que votre propre profil.");
        }

        client.ModifierCoordonnees(request.Email, request.Telephone, request.Adresse);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClientDto(client);
    }
}
