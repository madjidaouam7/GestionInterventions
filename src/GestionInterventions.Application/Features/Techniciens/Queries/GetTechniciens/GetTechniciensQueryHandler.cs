using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Domain.Entities;
using MediatR;

namespace GestionInterventions.Application.Features.Techniciens.Queries.GetTechniciens;

public class GetTechniciensQueryHandler : IRequestHandler<GetTechniciensQuery, List<TechnicienDto>>
{
    private readonly ITechnicienRepository _technicienRepository;

    public GetTechniciensQueryHandler(ITechnicienRepository technicienRepository)
    {
        _technicienRepository = technicienRepository;
    }

    public async Task<List<TechnicienDto>> Handle(GetTechniciensQuery request, CancellationToken cancellationToken)
    {   

        var techniciens = await _technicienRepository.GetAllAsync(cancellationToken);

        return techniciens.Select(technicien => new TechnicienDto(technicien))
                          .ToList();
    
    }
}