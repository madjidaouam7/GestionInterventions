using GestionInterventions.Application.Features.Techniciens.Commands.CreateTechnicien;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.Techniciens.Queries.GetTechniciens;
using Microsoft.AspNetCore.Authorization;
//using GestionInterventions.Application.Features.Clients.Queries.GetClientById;
//using GestionInterventions.Application.Features.Clients.Commands.UpdateClient;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TechniciensController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechniciensController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/techniciens
    [HttpPost]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> CreateTechnicien(
        [FromBody] CreateTechnicienCommand command,
        CancellationToken cancellationToken)
    {
        var technicienId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(CreateTechnicien), new { id = technicienId }, technicienId);
    }


    // GET: api/techniciens
    [HttpGet]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<List<TechnicienDto>>> GetTechniciens(CancellationToken cancellationToken)
    {
        var query = new GetTechniciensQuery();


        var techniciens = await _mediator.Send(query, cancellationToken);

        return Ok(techniciens);
    }
}