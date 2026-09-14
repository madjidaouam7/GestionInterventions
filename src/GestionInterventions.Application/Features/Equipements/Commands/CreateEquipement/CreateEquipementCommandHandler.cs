using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Exceptions;
using GestionInterventions.Domain.Entities;
using MediatR;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Application.Features.Equipements.Commands.CreateEquipement;

public class CreateEquipementCommandHandler : IRequestHandler<CreateEquipementCommand, int>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    ICurrentUserService _currentUserService;

    public CreateEquipementCommandHandler(IEquipementRepository equipementRepository, IClientRepository clientRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _equipementRepository = equipementRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateEquipementCommand request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);

        if (client is null)
            throw new NotFoundException("Le client n'existe pas.");

        if (_currentUserService.Role == "Client" && _currentUserService.ClientId != request.ClientId)
        {
            throw new ForbiddenAccessException(
                "Vous ne pouvez créer un équipement que pour votre propre compte.");
        }

        var equipement = new Equipement(request.Nom, request.NumeroSerie, request.Description, request.Localisation, request.ClientId);

        await _equipementRepository.AddAsync(equipement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return equipement.Id;
    }
}
