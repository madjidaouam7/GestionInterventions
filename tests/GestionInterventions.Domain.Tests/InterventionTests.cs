using GestionInterventions.Domain.Entities;
using GestionInterventions.Domain.Enums;
using GestionInterventions.Domain.Exceptions;

namespace GestionInterventions.Domain.Tests;

public class InterventionTests
{
    [Fact]
    public void ModifierDatePrevue_QuandInterventionEstPlanifiee_EtDateValide_ModifieDate()
    {
        // Arrange
        var dateInitiale = DateTime.UtcNow.AddDays(1);

        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: dateInitiale
        );

        var nouvelleDate = DateTime.UtcNow.AddDays(5);

        // Act
        intervention.ModifierDatePrevue(nouvelleDate);

        // Assert
        Assert.Equal(nouvelleDate, intervention.DatePrevue);
    }

    [Fact]
    public void ModifierDatePrevue_QuandInterventionEstEnCours_LeverUneException()
    {
        // Arrange
        var dateInitiale = DateTime.UtcNow.AddDays(1);
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: dateInitiale
        );

        intervention.Commencer();
        var nouvelleDate = DateTime.UtcNow.AddDays(5);

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        intervention.ModifierDatePrevue(nouvelleDate);
    });
    }


    [Fact]
    public void ModifierDatePrevue_QuandDateEstPassee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var nouvelleDate = DateTime.UtcNow.AddDays(-1);

        // Act et Assert
        Assert.Throws<DomainException>(() =>
        {
            intervention.ModifierDatePrevue(nouvelleDate);
        });
    }


    [Fact]
    public void Commencer_QuandInterventionEstPlanifiee_DevientEnCours()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        // Act
        intervention.Commencer();

        // Assert
        Assert.Equal(StatutIntervention.EnCours, intervention.Statut);
    }

    [Fact]
    public void Commencer_QuandInterventionEstEnCours_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        intervention.Commencer();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        intervention.Commencer();
    });
    }


    [Fact]
    public void Terminer_QuandInterventionEstEnCours_DevientTerminee()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        intervention.Commencer();

        var compteRendu = new CompteRendu(
        "operationsEffectuees",
        "observations",
        ResultatIntervention.Succes,
        "recommandations"
        );

        // Act
        intervention.Terminer(compteRendu);

        // Assert
        Assert.Equal(StatutIntervention.Terminee, intervention.Statut);
    }

    [Fact]
    public void Terminer_QuandInterventionEstTerminee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var compteRendu = new CompteRendu(
                "operationsEffectuees",
                "observations",
                ResultatIntervention.Succes,
                "recommandations"
                );

        intervention.Commencer();
        intervention.Terminer(compteRendu);

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        intervention.Terminer(compteRendu);
    });
    }


    [Fact]
    public void Valider_QuandInterventionEstTerminee_DevientCloturee()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var compteRendu = new CompteRendu(
                "operationsEffectuees",
                "observations",
                ResultatIntervention.Succes,
                "recommandations"
                );

        intervention.Commencer();
        intervention.Terminer(compteRendu);

        // Act
        intervention.Valider();

        // Assert
        Assert.Equal(StatutIntervention.Cloturee, intervention.Statut);
    }

    [Fact]
    public void Valider_QuandInterventionEstCloturee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var compteRendu = new CompteRendu(
                        "operationsEffectuees",
                        "observations",
                        ResultatIntervention.Succes,
                        "recommandations"
                        );

        intervention.Commencer();
        intervention.Terminer(compteRendu);
        intervention.Valider();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        intervention.Valider();
    });
    }


    [Fact]
    public void DemanderCorrection_QuandInterventionEstTerminees_DevientEnCours()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var compteRendu = new CompteRendu(
        "operationsEffectuees",
        "observations",
        ResultatIntervention.Succes,
        "recommandations"
        );

        intervention.Commencer();
        intervention.Terminer(compteRendu);

        // Act
        intervention.DemanderCorrection();

        // Assert
        Assert.Equal(StatutIntervention.EnCours, intervention.Statut);
    }

    [Fact]
    public void DemanderCorrection_QuandInterventionEstCloturee_LeverUneException()
    {
        // Arrange
        var intervention = new Intervention(
            demandeId: 1,
            technicienId: 1,
            datePrevue: DateTime.UtcNow.AddDays(1)
        );

        var compteRendu = new CompteRendu(
                "operationsEffectuees",
                "observations",
                ResultatIntervention.Succes,
                "recommandations"
                );

        intervention.Commencer();
        intervention.Terminer(compteRendu);
        intervention.Valider();

        // Act et Assert
        Assert.Throws<DomainException>(() =>
    {
        intervention.DemanderCorrection();
    });
    }


    [Fact]
    public void DemandeIdInvalide_QuandCreationIntervention_LeverUneException()
    {
        // Arrange
        int demandeId = 0;
        int technicienId = 1;
        DateTime datePrevue = DateTime.UtcNow.AddDays(1);

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Intervention(demandeId, technicienId, datePrevue)
        );
        // Assert
        Assert.Equal("L'Id de la demande est obligatoire.", exception.Message);
    }

    [Fact]
    public void TechnicienIdInvalide_QuandCreationIntervention_LeverUneException()
    {
        // Arrange
        int demandeId = 1;
        int technicienId = 0;
        DateTime datePrevue = DateTime.UtcNow.AddDays(1);

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Intervention(demandeId, technicienId, datePrevue)
        );
        // Assert
        Assert.Equal("L'Id du technicien est obligatoire.", exception.Message);
    }

    [Fact]
    public void DatePrevueInvalide_QuandCreationIntervention_LeverUneException()
    {
        // Arrange
        int demandeId = 1;
        int technicienId = 1;
        DateTime datePrevue = DateTime.UtcNow.AddDays(-1);

        // Act
        var exception = Assert.Throws<DomainException>(() =>
            new Intervention(demandeId, technicienId, datePrevue)
        );
        // Assert
        Assert.Equal("La date prévue de l'intervention doit être ultérieure à la date actuelle.", exception.Message);
    }

    [Fact]
    public void Intervention_QuandDonneesValide_EstCreeCorrectement()
    {
        // Arrange
        int demandeId = 1;
        int technicienId = 1;
        DateTime datePrevue = DateTime.UtcNow.AddDays(1);

        // Act
        var intervention = new Intervention(
            demandeId,
            technicienId,
            datePrevue
        );

        // Assert
        Assert.Equal(demandeId, intervention.DemandeId);
        Assert.Equal(technicienId, intervention.TechnicienId);
        Assert.Equal(datePrevue, intervention.DatePrevue);
        Assert.Equal(StatutIntervention.Planifiee, intervention.Statut);
    }
}