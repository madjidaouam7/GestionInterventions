using GestionInterventions.Domain.Entities;

namespace GestionInterventions.Application.DTOs;

public class ClientDto
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;

    public string Adresse { get; set; } = string.Empty;
    public ClientDto() { }
    public ClientDto(Client client)
    {
        Id = client.Id;
        Nom = client.Nom;
        Email = client.Email;
        Telephone = client.Telephone;
        Adresse = client.Adresse;
    }
}


public class UpdateClientDto
{
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
}
