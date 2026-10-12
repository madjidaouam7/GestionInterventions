using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.Equipements.Commands.AjouterPhotoEquipement;

public class AjouterPhotoEquipementCommandHandler : IRequestHandler<AjouterPhotoEquipementCommand, int>
{
    private readonly IEquipementRepository _equipementRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public AjouterPhotoEquipementCommandHandler(IEquipementRepository equipementRepository, IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
    {
        _equipementRepository = equipementRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(AjouterPhotoEquipementCommand request, CancellationToken cancellationToken)
    {
        var extensionsAutorisees = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        var extension = Path.GetExtension(request.NomFichier).ToLowerInvariant();

        if (!extensionsAutorisees.Contains(extension))
            throw new InvalidOperationException("Format de fichier non autorisé.");


        var contentTypesAutorises = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!contentTypesAutorises.Contains(request.ContentType))
            throw new InvalidOperationException("Type de fichier non autorisé.");


        const long tailleMaximale = 5 * 1024 * 1024;

        if (request.Contenu.Length > tailleMaximale)
            throw new InvalidOperationException("La taille du fichier ne doit pas dépasser 5 Mo.");


        var equipement = await _equipementRepository.GetByIdAsync(request.EquipementId, cancellationToken);

        if (equipement is null)
            throw new InvalidOperationException("L'équipement est introuvable.");

        var chemin = await _fileStorageService.SaveAsync(request.Contenu, request.NomFichier, request.ContentType, cancellationToken);

        var photo = new EquipementPhoto(request.EquipementId, request.NomFichier, chemin);

        equipement.Photos.Add(photo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return photo.Id;
    }
}