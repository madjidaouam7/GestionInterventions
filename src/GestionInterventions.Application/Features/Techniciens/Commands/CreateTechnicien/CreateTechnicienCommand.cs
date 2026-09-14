using MediatR;

namespace GestionInterventions.Application.Features.Techniciens.Commands.CreateTechnicien;

public record CreateTechnicienCommand(
    string Email,
    string Password,
    string Nom,
    string Telephone,
    string Adresse
) : IRequest<int>;
