using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Entities;

public class Client
{
    public int Id { get; private set; }

    public string Nom { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Telephone { get; private set; } = string.Empty;

    public string Adresse { get; private set; } = string.Empty;
    public string IdentityUserId { get; private set; } = string.Empty;

    // Propriété de navigation
    //public ICollection<Equipement> Equipements { get; set; } = new List<Equipement>();

    private Client() { } // Pour EF Core

    public Client(string nom, string email, string telephone, string adresse, string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(nom))
            throw new DomainException("Le nom du client est obligatoire.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("L'email du client est invalide.");

        if (string.IsNullOrWhiteSpace(telephone))
            throw new DomainException("Le téléphone du client est obligatoire.");

        if (string.IsNullOrWhiteSpace(adresse))
            throw new DomainException("L'adresse du client est obligatoire.");

        if (string.IsNullOrWhiteSpace(identityUserId))
        throw new DomainException("Le client doit être rattaché à un compte utilisateur.");

        Nom = nom;
        Email = email;
        Telephone = telephone;
        Adresse = adresse;
        IdentityUserId = identityUserId;
    }

    public void ModifierCoordonnees(string email, string telephone, string adresse)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("L'email du client est invalide.");

        if (string.IsNullOrWhiteSpace(telephone))
            throw new DomainException("Le téléphone du client est obligatoire.");

        if (string.IsNullOrWhiteSpace(adresse))
            throw new DomainException("L'adresse du client est obligatoire.");

        Email = email;
        Telephone = telephone;
        Adresse = adresse;
    }
}
