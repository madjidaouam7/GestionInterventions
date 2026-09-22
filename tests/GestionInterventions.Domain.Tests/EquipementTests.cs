using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Tests;

public class EquipementTests
{
    [Fact]
    public void SignalerPanne_QuandEquipementEstFonctionnel_DevientEnPanne()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act
        equipement.SignalerPanne();

        // Assert
        Assert.Equal(StatutEquipement.EnPanne, equipement.Statut);
    }

    [Fact]
    public void SignalerPanne_QuandEquipementEstEnMaintenance_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();
        equipement.DemarrerMaintenance();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        equipement.SignalerPanne();
    });
    }






    [Fact]
    public void DemarrerMaintenance_QuandEquipementEstEnPanne_DevientEnMaintenance()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();

        // Act
        equipement.DemarrerMaintenance();

        // Assert
        Assert.Equal(StatutEquipement.EnMaintenance, equipement.Statut);
    }

    [Fact]
    public void DemarrerMaintenance_QuandEquipementEstFonctionnel_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        equipement.DemarrerMaintenance();
    });
    }








    [Fact]
    public void RemettreEnService_QuandEquipementEstEnMaintenance_DevientFonctionnel()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();
        equipement.DemarrerMaintenance();

        // Act
        equipement.RemettreEnService();

        // Assert
        Assert.Equal(StatutEquipement.Fonctionnel, equipement.Statut);
    }

    [Fact]
    public void RemettreEnService_EquipementFonctionnel_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        equipement.RemettreEnService();
    });
    }





    [Fact]
    public void SignalerEchecMaintenance_QuandEquipementEstEnMaintenance_DevientEnPanne()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();
        equipement.DemarrerMaintenance();

        // Act
        equipement.SignalerEchecMaintenance();

        // Assert
        Assert.Equal(StatutEquipement.EnPanne, equipement.Statut);
    }

    [Fact]
    public void SignalerEchecMaintenance_EquipementFonctionnel_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        equipement.SignalerEchecMaintenance();
    });
    }





    [Fact]
    public void AnnulerSignalementPanne_QuandEquipementEstEnPanne_DevientFonctionnel()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        equipement.SignalerPanne();

        // Act
        equipement.AnnulerSignalementPanne();
        // Assert
        Assert.Equal(StatutEquipement.Fonctionnel, equipement.Statut);
    }

    [Fact]
    public void AnnulerSignalementPanne_EquipementFonctionnel_LeverUneException()
    {
        // Arrange
        var equipement = new Equipement(
            "Ordinateur",
            "SN12345",
            "Ordinateur de bureau",
            "Bureau 1",
            1
        );

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        equipement.AnnulerSignalementPanne();
    });
    }








    [Fact]
    public void NomVide_QuandCreationEquipement_LeverUneException()
    {
        // Arrange
        string nom = "";
        string numeroSerie = "SN12345";
        string description = "Ordinateur de bureau";
        string localisation = "Bureau 1";
        int clientId = 1;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Equipement(nom, numeroSerie, description, localisation, clientId)
        );
        // Assert
        Assert.Equal("Le nom de l'equipement est obligatoire.", exception.Message);
    }

    [Fact]
    public void NumeroSerieVide_QuandCreationEquipement_LeverUneException()
    {
        // Arrange
        string nom = "Ordinateur";
        string numeroSerie = "";
        string description = "Ordinateur de bureau";
        string localisation = "Bureau 1";
        int clientId = 1;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Equipement(nom, numeroSerie, description, localisation, clientId)
        );
        // Assert
        Assert.Equal("Le numero de serie de l'equipement est invalide.", exception.Message);
    }

    [Fact]
    public void DescriptionVide_QuandCreationEquipement_LeverUneException()
    {
        // Arrange
        string nom = "Ordinateur";
        string numeroSerie = "SN12345";
        string description = "";
        string localisation = "Bureau 1";
        int clientId = 1;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Equipement(nom, numeroSerie, description, localisation, clientId)
        );
        // Assert
        Assert.Equal("La description de l'equipement est obligatoire.", exception.Message);
    }

    [Fact]
    public void LocalisationVide_QuandCreationEquipement_LeverUneException()
    {
        // Arrange
        string nom = "Ordinateur";
        string numeroSerie = "SN12345";
        string description = "Ordinateur de bureau";
        string localisation = "";
        int clientId = 1;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Equipement(nom, numeroSerie, description, localisation, clientId)
        );
        // Assert
        Assert.Equal("La localisation de l'equipement est obligatoire.", exception.Message);
    }

    [Fact]
    public void ClientIdVide_QuandCreationEquipement_LeverUneException()
    {
        // Arrange
        string nom = "Ordinateur";
        string numeroSerie = "SN12345";
        string description = "Ordinateur de bureau";
        string localisation = "Bureau 1";
        int clientId = 0;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Equipement(nom, numeroSerie, description, localisation, clientId)
        );
        // Assert
        Assert.Equal("L'Id du client est obligatoire.", exception.Message);
    }




    [Fact]
    public void Equipement_QuandDonneesValide_EstCorrectementCree()
    {
        // Arrange
        string nom = "Ordinateur";
        string numeroSerie = "SN12345";
        string description = "Ordinateur de bureau";
        string localisation = "Bureau 1²";
        int clientId = 1;

        // Act
        var equipement = new Equipement(
            nom,
            numeroSerie,
            description,
            localisation,
            clientId
        );
        // Assert
        Assert.Equal(nom, equipement.Nom);
        Assert.Equal(numeroSerie, equipement.NumeroSerie);
        Assert.Equal(description, equipement.Description);
        Assert.Equal(localisation, equipement.Localisation);
        Assert.Equal(clientId, equipement.ClientId);
        Assert.Equal(StatutEquipement.Fonctionnel, equipement.Statut);
    }
}