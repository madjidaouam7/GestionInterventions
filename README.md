# GestionInterventions

🇫🇷 **Français** | 🇬🇧 [English](README_EN.md)

> Application web full-stack de gestion des interventions techniques, développée avec .NET et basée sur Clean Architecture, CQRS et les principes du Domain-Driven Design.

## 📋 Présentation

**GestionInterventions** est une application de gestion des interventions techniques permettant de suivre l'ensemble du cycle de maintenance d'une entreprise.

Le système permet aux clients de déclarer des problèmes sur leurs équipements, au responsable de traiter et planifier les demandes, et aux techniciens de réaliser les interventions et de renseigner leurs comptes rendus.

Le projet a été réalisé comme projet de formation afin de mettre en pratique une architecture logicielle structurée et des pratiques proches d'un environnement professionnel.

## 👥 Rôles et fonctionnalités

### Client

* Création de compte et authentification
* Gestion de ses équipements
* Déclaration de demandes d'intervention
* Suivi de l'état de ses demandes
* Consultation de l'historique de ses équipements
* Consultation des interventions et comptes rendus associés
* Mise à jour de ses informations personnelles

### Technicien

* Consultation des interventions qui lui sont assignées
* Démarrage d'une intervention
* Réalisation et clôture technique d'une intervention
* Saisie d'un compte rendu structuré
* Consultation des informations liées aux équipements et demandes

### Responsable

* Consultation des demandes en attente
* Acceptation ou refus des demandes
* Planification des interventions
* Affectation d'un technicien
* Modification d'une intervention planifiée
* Validation des interventions terminées
* Demande de correction d'une intervention
* Gestion des techniciens
* Consultation des clients et des interventions
* Consultation de l'historique complet des équipements

Les principales règles métier sont centralisées dans le **Domain**, notamment les transitions d'état des demandes, équipements et interventions ainsi que les règles d'accès liées à la propriété des ressources.

## 🏗️ Architecture

Le projet suit une approche **Clean Architecture** avec séparation claire des responsabilités.

GestionInterventions.Domain
        ↓
GestionInterventions.Application
        ↓
GestionInterventions.Infrastructure
        ↓
GestionInterventions.Api
        ↓
GestionInterventions.Web


### Domain

Contient le cœur métier de l'application :

* Entités
* Énumérations
* Exceptions métier
* Value Objects
* Règles et invariants métier

Le Domain ne dépend pas d'Entity Framework Core, d'ASP.NET Core ou de l'interface utilisateur.

### Application

Contient les cas d'utilisation de l'application :

* Commands
* Queries
* Handlers MediatR
* DTOs
* Interfaces des repositories
* Validation

### Infrastructure

Contient les implémentations techniques :

* Entity Framework Core
* SQLite
* Repositories
* Unit of Work
* ASP.NET Core Identity
* Génération et validation des JWT
* Persistence

### API

Expose les fonctionnalités via une API REST :

* Authentification
* Clients
* Techniciens
* Équipements
* Demandes d'intervention
* Interventions
* Gestion globale des exceptions

### Web

Interface utilisateur développée avec **Blazor Web App** et consommant l'API.

## ⚙️ Principaux choix techniques

### CQRS + MediatR

Chaque cas d'utilisation est représenté par une **Command** ou une **Query**, avec un Handler dédié.

Command / Query
       ↓
    Handler
       ↓
Repository / Service
       ↓
     Domain


### Repository + Unit of Work

La couche Application ne dépend pas directement d'Entity Framework Core.

Les interfaces des repositories sont définies dans Application et leurs implémentations se trouvent dans Infrastructure.

### Authentification et autorisation

L'application utilise :

* ASP.NET Core Identity
* JWT Bearer Authentication
* Autorisation basée sur les rôles
* Vérification de propriété des ressources

Les informations sensibles sont stockées localement avec **User Secrets** et ne sont pas versionnées dans Git.

### Domain-Driven Design

Le modèle métier utilise notamment :

* Entités avec comportement
* Invariants métier
* Exceptions métier
* Value Object `CompteRendu`
* Méthodes métier explicites

Par exemple :

Equipement.SignalerPanne()
Intervention.Terminer(compteRendu)

plutôt que de modifier directement les états depuis les contrôleurs.

### Gestion globale des erreurs

Un middleware global transforme les exceptions métier en réponses HTTP appropriées afin de conserver des contrôleurs légers et cohérents.

## 🛠️ Technologies

| Domaine             | Technologie                 |
| ------------------- | --------------------------- |
| Langage             | C#                          |
| Framework           | .NET 8 / ASP.NET Core       |
| API                 | ASP.NET Core Web API        |
| Frontend            | Blazor Web App              |
| ORM                 | Entity Framework Core       |
| Base de données     | SQLite                      |
| Architecture        | Clean Architecture          |
| Design              | Domain-Driven Design        |
| Pattern             | CQRS                        |
| Médiateur           | MediatR                     |
| Authentification    | ASP.NET Core Identity + JWT |
| UI                  | Bootstrap                   |
| Contrôle de version | Git / GitHub                |

## 📸 Captures d'écran

### Tableau de bord

![Tableau de bord](screenshots/accueil.png)

### Gestion des équipements

![Gestion des équipements](screenshots/equipements.png)

### Gestion des demandes

![Gestion des demandes](screenshots/demandes.png)

### Gestion des interventions

![Gestion des interventions](screenshots/interventions.png)

> Les captures d'écran présentent les principales interfaces de l'application.

## 📁 Structure du projet

GestionInterventions/
│
├── src/
│   ├── GestionInterventions.Domain/
│   ├── GestionInterventions.Application/
│   ├── GestionInterventions.Infrastructure/
│   ├── GestionInterventions.Api/
│   └── GestionInterventions.Web/
│
├── screenshots/
│   ├── accueil.png
│   ├── equipements.png
│   ├── demandes.png
│   └── interventions.png
│
├── GestionInterventions.sln
├── README.md
├── README_EN.md
└── .gitignore

## 🚀 Installation

### Prérequis

* .NET 8 SDK
* Git
* `dotnet-ef`

Installation de l'outil Entity Framework :

dotnet tool install --global dotnet-ef

### Cloner le projet

git clone https://github.com/madjidaouam7/GestionInterventions.git
cd GestionInterventions

### Restaurer et compiler

dotnet restore
dotnet build

### Configurer les secrets

Les secrets nécessaires à l'application doivent être configurés avec **User Secrets**.

Initialiser User Secrets :

dotnet user-secrets init --project src/GestionInterventions.Api

Configurer le mot de passe du compte Responsable :


dotnet user-secrets set "SeedAdmin:Password" "VOTRE_MOT_DE_PASSE" --project src/GestionInterventions.Api

Configurer la clé JWT :

dotnet user-secrets set "Jwt:Key" "VOTRE_CLE_JWT" --project src/GestionInterventions.Api

> Ne partagez jamais vos vraies valeurs de secrets dans le dépôt GitHub.

### Créer / mettre à jour la base de données

dotnet ef database update --project src/GestionInterventions.Infrastructure --startup-project src/GestionInterventions.Api

### Lancer l'API

Dans un terminal :

dotnet run --project src/GestionInterventions.Api

### Lancer l'application Web

Dans un deuxième terminal :

dotnet run --project src/GestionInterventions.Web

## 🔐 Compte Responsable

Le compte Responsable est créé automatiquement lors de l'initialisation de l'application.

Pour des raisons de sécurité, le mot de passe n'est pas stocké dans le dépôt. Il doit être configuré localement avec **User Secrets**.

Les comptes Client peuvent être créés via l'inscription publique et les comptes Technicien peuvent être créés par le Responsable.

## 📌 État du projet

Projet de formation réalisé individuellement afin de mettre en pratique la conception et le développement d'une application web structurée avec ASP.NET Core.

Le projet met particulièrement l'accent sur :

* la séparation des responsabilités ;
* la conception orientée domaine ;
* CQRS et MediatR ;
* l'injection de dépendances ;
* l'accès aux données avec Entity Framework Core ;
* l'authentification et l'autorisation ;
* la conception d'une API REST ;
* la création d'une interface utilisateur avec Blazor.

## 📄 Licence

Projet réalisé à des fins de formation, de portfolio et de démonstration technique.
