using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;


public class HistoriqueEquipementDto
{
    public int EquipementId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public StatutEquipement Statut { get; set; }

    public List<HistoriqueDemandeDto> Demandes { get; set; } = new();
    public HistoriqueEquipementDto(){}
    public HistoriqueEquipementDto(Equipement equipement)
    {
        EquipementId = equipement.Id;
        Nom = equipement.Nom;
        NumeroSerie = equipement.NumeroSerie;
        Statut = equipement.Statut;

        foreach(var demande in equipement.Demandes)
        {
            var demandeDto = new HistoriqueDemandeDto(demande);
            Demandes.Add(demandeDto);
        }
    }
}
