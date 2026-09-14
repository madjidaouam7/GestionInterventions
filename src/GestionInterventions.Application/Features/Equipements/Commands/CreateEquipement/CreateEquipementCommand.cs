using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Commands.CreateEquipement;

public record CreateEquipementCommand(
    string Nom,
    string NumeroSerie,
    string Description,
    string Localisation,
    int ClientId
) : IRequest<int>;