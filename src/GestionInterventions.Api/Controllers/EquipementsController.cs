using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.Equipements.Commands.CreateEquipement;
using GestionInterventions.Application.Features.Equipements.Queries.GetAllEquipements;
using GestionInterventions.Application.Features.Equipements.Queries.GetEquipementById;
using GestionInterventions.Application.Features.Equipements.Queries.GetEquipementsByClient;
using GestionInterventions.Application.Features.Equipements.Queries.GetHistoriqueEquipement;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EquipementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<int>> CreateEquipement(
        [FromBody] CreateEquipementCommand command,
        CancellationToken cancellationToken)
    {
        var equipementId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetEquipement), new { equipementId = equipementId }, equipementId);
    }



    [HttpGet("{equipementId}")]
    [Authorize(Roles = "Responsable,Client")]
    public async Task<ActionResult<EquipementDto>> GetEquipement(int equipementId, CancellationToken cancellationToken)
    {
        var query = new GetEquipementByIdQuery(equipementId);

        var equipement = await _mediator.Send(query, cancellationToken);

        return Ok(equipement);
    }


    [HttpGet("{clientId}/equipements")]
    [Authorize(Roles = "Responsable,Client")]
    public async Task<ActionResult<List<EquipementDto>>> GetEquipementsByClient(int clientId, CancellationToken cancellationToken)
    {
        var query = new GetEquipementsByClientQuery(clientId);


        var equipements = await _mediator.Send(query, cancellationToken);

        return Ok(equipements);
    }


    [HttpGet("{equipementId}/historique")]
    [Authorize(Roles = "Responsable,Client")]
    public async Task<ActionResult<HistoriqueEquipementDto>> GetHistoriqueEquipement(int equipementId, CancellationToken cancellationToken)
    {
        var query = new GetHistoriqueEquipementQuery(equipementId);

        var equipement = await _mediator.Send(query, cancellationToken);

        return Ok(equipement);
    }


    [HttpGet]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<List<EquipementDto>>> GetAllEquipements(CancellationToken cancellationToken)
    {
        var query = new GetAllEquipementsQuery();
        var equipements = await _mediator.Send(query, cancellationToken);
        return Ok(equipements);
    }
}