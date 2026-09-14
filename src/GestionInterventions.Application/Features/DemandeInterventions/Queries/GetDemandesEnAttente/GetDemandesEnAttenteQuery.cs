using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.DemandeInterventions.Queries.GetDemandesEnAttente;

public record GetDemandesEnAttenteQuery : IRequest<List<DemandeInterventionDto>>;