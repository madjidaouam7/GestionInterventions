# GestionInterventions

🇫🇷 [French](README.md) | 🇬🇧 **English**

A full-stack web application for managing technical maintenance interventions, developed with **.NET 8** and based on **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles.

> Individual training and portfolio project, deployed on Render for demonstration purposes.

## 📋 Overview

**GestionInterventions** is a technical maintenance management application that covers the main stages of a company's maintenance lifecycle.

Clients can manage their equipment and submit intervention requests. The manager reviews requests, schedules interventions, and assigns technicians. Technicians carry out interventions and submit their reports.

The project was designed to put structured software architecture, separation of concerns, and development practices close to those used in professional environments into practice.

## 🌐 Live Demo

The application is deployed on **Render**.

- **Web Application:** [Access the demo](https://siyana-web-demo.onrender.com/)

> **Note:** This version is hosted on Render's free tier for demonstration purposes. Local storage used for SQLite and uploaded files may be reset after certain restarts or redeployments. Service availability may also vary. This version is therefore not intended for production use with important data.

## 👥 Roles and Features

### 👤 Client

- Account registration and authentication.
- Manage their equipment.
- Submit intervention requests.
- Track the status of their requests.
- View equipment history.
- View interventions and their associated reports.
- Update personal information.
- Upload and view equipment photos, depending on the features available in the interface.

### 🧑‍🔧 Technician

- View assigned interventions.
- Start and carry out an intervention.
- Enter a structured intervention report.
- Submit completed interventions for validation.
- View information related to equipment and requests.

### 🧑‍💼 Manager

- View pending requests.
- Accept or reject requests.
- Schedule interventions.
- Assign technicians.
- Reschedule planned interventions.
- Validate completed interventions.
- Request corrections to an intervention.
- Manage technicians.
- View clients and interventions.
- View equipment history.
- Receive notifications when new requests are submitted, using SignalR.

Business rules are primarily centralized in the **Domain** layer, including status transitions for requests, equipment, and interventions, as well as business model invariants.

## 🏗️ Architecture

The project follows **Clean Architecture**, with clear separation of responsibilities between the business, application, technical, and presentation layers.

```text
GestionInterventions/
├── GestionInterventions.Domain
├── GestionInterventions.Application
├── GestionInterventions.Infrastructure
├── GestionInterventions.Api
└── GestionInterventions.Web
```

### Domain

Contains the core business logic:

- Entities and enumerations.
- Business exceptions.
- Value Objects.
- Business rules and invariants.

This layer does not depend on Entity Framework Core, ASP.NET Core, or the user interface.

### Application

Contains the application's use cases:

- Commands and Queries.
- MediatR Handlers.
- DTOs.
- Repository interfaces.
- Validation and orchestration of application operations.

### Infrastructure

Contains technical implementations:

- Entity Framework Core and SQLite.
- Repository implementations.
- Data access.
- ASP.NET Core Identity.
- Authentication and persistence-related functionality.

### API

Exposes business functionality through a REST API:

- Client, technician, and equipment management.
- Intervention requests and interventions.
- Authentication and authorization.
- Global error handling.
- SignalR Hub for real-time notifications.

### Web

User interface developed with **Blazor Web App using Interactive Server rendering**. It consumes the REST API to access application features and uses SignalR for real-time notifications.

## ⚙️ Main Technical Decisions

### CQRS and MediatR

Application operations are organized around Commands and Queries, each handled by a dedicated Handler.

```text
Command / Query
       ↓
     Handler
       ↓
Repository / Service
       ↓
     Domain
```

This organization helps separate read operations from write operations and keeps controllers lightweight.

### Repository and Unit of Work

The Application layer depends on abstractions defined by the project rather than directly on Entity Framework Core. Technical implementations are located in the Infrastructure layer.

### Authentication and Authorization

The application uses:

- ASP.NET Core Identity.
- JWT Bearer Authentication.
- Role-based authorization for Client, Technician, and Manager.
- Resource access checks based on application rules.

Keys and passwords required for local execution must be configured outside the repository, for example, using **.NET User Secrets**.

### Domain-Driven Design

The business model relies on entities that encapsulate their own behavior, invariants, business exceptions, and the `CompteRendu` Value Object.

Examples of business methods:

```csharp
Equipement.SignalerPanne();
Intervention.Terminer(compteRendu);
```

The goal is to enforce business rules within the domain rather than modifying states arbitrarily from controllers.

### Global Error Handling

A global middleware converts business exceptions into appropriate HTTP responses, helping keep controllers lightweight and consistent.

### Real-Time Notifications with SignalR

SignalR sends notifications about new intervention requests to the manager without requiring a manual page refresh.

### Photo Management

The application allows photos to be associated with equipment. Files are uploaded and served by the API, while the Web interface uses the configured API address to display them.

## 🛠️ Technologies

| Area | Technology |
|---|---|
| Language | C# |
| Framework | .NET 8 / ASP.NET Core |
| API | ASP.NET Core Web API |
| Web Interface | Blazor Web App — Interactive Server |
| ORM | Entity Framework Core |
| Database | SQLite |
| Architecture | Clean Architecture |
| Business Design | Domain-Driven Design |
| Application Pattern | CQRS |
| Mediator | MediatR |
| Authentication | ASP.NET Core Identity and JWT |
| Notifications | SignalR |
| Containerization | Docker and Docker Compose |
| User Interface | Bootstrap |
| Version Control | Git and GitHub |
| Demo Hosting | Render |

## 📸 Screenshots

### Dashboard

![Dashboard](screenshots/accueil.png)

### Equipment Management

![Equipment Management](screenshots/equipements.png)

### Intervention Request Management

![Intervention Request Management](screenshots/demandes.png)

### Intervention Management

![Intervention Management](screenshots/interventions.png)

The screenshots show the application's main interfaces. Make sure the corresponding files exist in the repository's `screenshots/` directory.

## 📁 Repository Structure

```text
GestionInterventions/
├── src/
│   ├── GestionInterventions.Domain/
│   ├── GestionInterventions.Application/
│   ├── GestionInterventions.Infrastructure/
│   ├── GestionInterventions.Api/
│   └── GestionInterventions.Web/
├── screenshots/
├── GestionInterventions.sln
├── Dockerfile
├── Dockerfile.web
├── docker-compose.yml
├── .dockerignore
├── .gitignore
├── README.md
└── README_EN.md
```

## 🚀 Installation and Local Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) if you want to run the application with Docker.
- The `dotnet-ef` tool to manage database migrations.

Install the Entity Framework Core tool if needed:

```bash
dotnet tool install --global dotnet-ef
```

### 1. Clone the Repository

```bash
git clone https://github.com/madjidaouam7/GestionInterventions.git
cd GestionInterventions
```

### 2. Restore and Build the Project

```bash
dotnet restore
dotnet build
```

### 3. Configure API Secrets

Initialize .NET User Secrets for the API project:

```bash
dotnet user-secrets init --project src/GestionInterventions.Api
```

Configure the password used by the Manager account creation mechanism:

```bash
dotnet user-secrets set "SeedAdmin:Password" "YOUR_PASSWORD" --project src/GestionInterventions.Api
```

Configure the JWT signing key with a sufficiently long and random value:

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_RANDOM_JWT_KEY" --project src/GestionInterventions.Api
```

Use your own values. Never publish actual secret values in the Git repository.

### 4. Configure the API URL

The Web application must know the API address. By default, the local project configuration uses:

```text
http://localhost:5084/
```

If the API listens on a different address or port, configure `Api:BaseUrl` in the Web application's settings to match the actual API address.

### 5. Create or Update the Database

```bash
dotnet ef database update --project src/GestionInterventions.Infrastructure --startup-project src/GestionInterventions.Api
```

Make sure the configured connection string points to the intended local database.

### 6. Run the API

In a first terminal:

```bash
dotnet run --project src/GestionInterventions.Api
```

### 7. Run the Web Application

In a second terminal:

```bash
dotnet run --project src/GestionInterventions.Web
```

Then open the local address displayed in the Web application's terminal.

> **Manager account:** The account is created by the application's initialization mechanism if the configuration and seeding process are properly set up. Its password must be configured locally. Client accounts can be created through public registration, while Technician accounts are created by the Manager.

## 🐳 Running with Docker

The repository contains Docker files to build the API and Web application, as well as a Docker Compose configuration.

If Docker Desktop is installed and the required local configuration is in place, run the following command from the repository root:

```bash
docker compose up --build
```

The services and their settings depend on the `docker-compose.yml` configuration. Review this file before starting the application to check the configured ports, environment variables, volumes, and network.

Secrets and local data files must not be added to the Git repository.

## 🧪 Tests and Continuous Integration

The project includes unit tests that can be executed with:

```bash
dotnet test
```

Continuous integration is handled by **GitHub Actions**. On every push to the `main` branch or Pull Request targeting `main`, the pipeline automatically performs:

- NuGet dependency restoration
- .NET 8 solution build
- Test execution

[![.NET CI](https://github.com/madjidaouam7/GestionInterventions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/madjidaouam7/GestionInterventions/actions/workflows/dotnet.yml)

## 📌 Project Status

Individual training and portfolio project developed to put the following concepts into practice:

- Separation of concerns through Clean Architecture.
- Domain-driven design.
- CQRS and MediatR.
- Dependency injection and dependency inversion.
- Data access using Entity Framework Core and SQLite.
- Authentication and authorization.
- REST API design.
- Blazor user interface development.
- Real-time notifications with SignalR.
- Containerization and deployment of an online demo.

## 📄 License

This project was developed for training, portfolio, and technical demonstration purposes. No specific open-source license has been declared at this time.