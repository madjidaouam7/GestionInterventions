using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Commands.AjouterPhotoEquipement;

public record AjouterPhotoEquipementCommand(
    int EquipementId,
    string NomFichier,
    string ContentType,
    Stream Contenu
) : IRequest<int>;