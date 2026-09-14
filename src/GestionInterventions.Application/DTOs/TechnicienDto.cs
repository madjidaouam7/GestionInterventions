using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.DTOs;

public class TechnicienDto
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;

    public string Adresse { get; set; } = string.Empty;
    public TechnicienDto(){}
    public TechnicienDto(Technicien technicien)
    {
        Id = technicien.Id;
        Nom = technicien.Nom;
        Email = technicien.Email;
        Telephone = technicien.Telephone;
        Adresse = technicien.Adresse;
    }
}

