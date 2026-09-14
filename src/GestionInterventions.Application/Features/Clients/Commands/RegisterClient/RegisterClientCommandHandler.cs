using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Exceptions;
using MediatR;

namespace GestionInterventions.Application.Features.Clients.Commands.RegisterClient;

public class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, int>
{
    private readonly IIdentityService _identityService;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterClientCommandHandler(IIdentityService identityService, IClientRepository clientRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email, request.Password, "Client");

        if (!succeeded)
            throw new DomainException(string.Join(" ", errors));

        try
        {
            var client = new Client(request.Nom, request.Email, request.Telephone, request.Adresse, userId!);

            await _clientRepository.AddAsync(client, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return client.Id;
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
