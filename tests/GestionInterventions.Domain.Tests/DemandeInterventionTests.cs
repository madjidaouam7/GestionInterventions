using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Tests;

public class DemandeInterventionTests
{
    [Fact]
    public void Accepter_QuandDemandeEstEnAttente_DevientAcceptee()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // Act
        demande.Accepter();

        // Assert
        Assert.Equal(StatutDemande.Acceptee, demande.Statut);
    }

    [Fact]
    public void Accepter_QuandDemandeEstAcceptee_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        demande.Accepter();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        demande.Accepter();
    });
    }

    [Fact]
    public void Refuser_QuandDemandeEstEnAttente_DevientRefusee()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        // Act
        demande.Refuser();

        // Assert
        Assert.Equal(StatutDemande.Refusee, demande.Statut);
    }

    [Fact]
    public void Refuser_QuandDemandeEstAcceptee_LeverUneException()
    {
        // Arrange
        var demande = new DemandeIntervention(
            "Ordinateur chauffe",
            1,
            PrioriteDemande.Normale
        );

        demande.Accepter();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        demande.Refuser();
    });
    }


    [Fact]
    public void DescriptionVide_QuandCreationDemande_LeverUneException()
    {
        // Arrange
        string description = "";
        int equipementId = 1;
        PrioriteDemande priorite = PrioriteDemande.Normale;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DemandeIntervention(description, equipementId, priorite)
        );
        // Assert
        Assert.Equal("La description du probleme est obligatoire.", exception.Message);
    }

    [Fact]
    public void EquipementIdVide_QuandCreationDemande_LeverUneException()
    {
        // Arrange
        string description = "Ordinateur chauffe";
        int equipementId = 0;
        PrioriteDemande priorite = PrioriteDemande.Normale;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DemandeIntervention(description, equipementId, priorite)
        );
        // Assert
        Assert.Equal("L'Id de l'equipement est obligatoire.", exception.Message);
    }

    [Fact]
    public void PrioriteInvalide_QuandCreationDemande_LeverUneException()
    {
        // Arrange
        string description = "Ordinateur chauffe";
        int equipementId = 1;
        PrioriteDemande priorite = (PrioriteDemande)999;

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new DemandeIntervention(description, equipementId, priorite)
        );
        // Assert
        Assert.Equal("Le niveau de priorité est invalide (normale ou urgente).", exception.Message);
    }

    [Fact]
    public void Demande_QuandDonneesValide_EstCreeCorrectement()
    {
        // Arrange
        string description = "Ordinateur chauffe";
        int equipementId = 1;
        PrioriteDemande priorite = PrioriteDemande.Normale;

        // Act
        var demande = new DemandeIntervention(
            description,
            equipementId,
            priorite
        );

        // Assert
        Assert.Equal(description,demande.Description);
        Assert.Equal(equipementId,demande.EquipementId);
        Assert.Equal(priorite,demande.Priorite);
        Assert.Equal(StatutDemande.EnAttente, demande.Statut);
    }
}