using GestionInterventions.Application.DTOs;
using MediatR;

namespace GestionInterventions.Application.Features.Techniciens.Queries.GetTechniciens;

public record GetTechniciensQuery : IRequest<List<TechnicienDto>>;