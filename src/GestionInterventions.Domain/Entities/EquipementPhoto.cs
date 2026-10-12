namespace GestionInterventions.Domain.Entities;

public class EquipementPhoto
{
    public int Id { get; private set; }
    public string NomFichier { get; private set; } = string.Empty;
    public string CheminFichier { get; private set; } = string.Empty;
    public DateTime DateAjout { get; private set; }
    public int EquipementId { get; private set; }
    public Equipement Equipement { get; private set; } = null!;


    public EquipementPhoto(int equipementId, string nomFichier, string cheminFichier)
    {
        EquipementId = equipementId;
        NomFichier = nomFichier;
        CheminFichier = cheminFichier;
        DateAjout = DateTime.UtcNow;
    }

}