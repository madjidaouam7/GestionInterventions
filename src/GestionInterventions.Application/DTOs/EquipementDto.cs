using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;

namespace GestionInterventions.Application.DTOs;

public class EquipementDto
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string NumeroSerie { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Localisation { get; set; } = string.Empty;

    public int ClientId { get; set; }
    
    public StatutEquipement Statut { get; set; }

    public EquipementDto(){}
    public EquipementDto(Equipement equipement)
    {
        Id = equipement.Id;
        Nom = equipement.Nom;
        NumeroSerie = equipement.NumeroSerie;
        Description = equipement.Description;
        Localisation = equipement.Localisation;
        ClientId = equipement.ClientId;
        Statut = equipement.Statut;
    }
}