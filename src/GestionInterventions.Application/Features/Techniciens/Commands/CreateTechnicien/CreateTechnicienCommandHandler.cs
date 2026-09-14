using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Techniciens.Commands.CreateTechnicien;

public class CreateTechnicienCommandHandler : IRequestHandler<CreateTechnicienCommand, int>
{
    private readonly IIdentityService _identityService;
    private readonly ITechnicienRepository _technicienRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTechnicienCommandHandler(IIdentityService identityService, ITechnicienRepository technicienRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _technicienRepository = technicienRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateTechnicienCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email, request.Password, "Technicien");

        if (!succeeded)
            throw new DomainException(string.Join(" ", errors));

        try
        {
            var technicien = new Technicien(request.Nom, request.Email,request.Telephone, request.Adresse, userId!);

            await _technicienRepository.AddAsync(technicien, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return technicien.Id;
        }
        catch
        {
            // Rollback manuel : le compte Identity a déjà été créé, on le supprime
            // si la création du Client métier échoue, pour éviter un compte orphelin.
            await _identityService.DeleteUserAsync(userId!);
            throw;
        }
    }
}
