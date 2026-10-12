namespace GestionInterventions.Application.DTOs;

public class EquipementPhotoDto
{
    public int Id { get; set; }

    public string NomFichier { get; set; } = string.Empty;

    public string CheminFichier { get; set; } = string.Empty;

    public DateTime DateAjout { get; set; }
}