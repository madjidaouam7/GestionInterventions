using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.AccepterDemande;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.RefuserDemande;
using GestionInterventions.Application.Features.DemandeInterventions.Commands.CreateDemandeIntervention;
using GestionInterventions.Application.Features.DemandeInterventions.Queries.GetDemandesEnAttente;
using GestionInterventions.Application.Features.DemandeInterventions.Queries.GetMesDemandes;
using GestionInterventions.Application.Features.DemandeInterventions.Queries.GetAllDemandes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionInterventions.Domain.Exceptions;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Application.Common.Models;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DemandeInterventionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public DemandeInterventionsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }



    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<int>> CreateDemandeIntervention(
        [FromBody] CreateDemandeInterventionCommand command,
        CancellationToken cancellationToken)
    {
        var demandeInterventionId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(CreateDemandeIntervention), new { id = demandeInterventionId }, demandeInterventionId);
    }



    [HttpGet("en-attente")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<List<DemandeInterventionDto>>> GetDemandesEnAttente(CancellationToken cancellationToken)
    {
        var query = new GetDemandesEnAttenteQuery();


        var demandeInterventions = await _mediator.Send(query, cancellationToken);

        return Ok(demandeInterventions);
    }


    [HttpGet("mes-demandes")]
    [Authorize(Roles = "Client")]
    public async Task<ActionResult<PagedResult<DemandeInterventionDto>>> GetMesDemandes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var clientId = _currentUserService.ClientId ?? throw new ForbiddenAccessException("Identifiant client introuvable dans le token.");

        var query = new GetMesDemandesQuery(clientId, page, pageSize);

        var mesDemandes = await _mediator.Send(query, cancellationToken);

        return Ok(mesDemandes);
    }



    [HttpPut("{id}/accepter")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> AccepterDemande(int id, CancellationToken cancellationToken)
    {
        var command = new AccepterDemandeCommand(id);

        var demandeId = await _mediator.Send(command, cancellationToken);

        return Ok(demandeId);
    }



    [HttpPut("{id}/refuser")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> RefuserDemande(int id, CancellationToken cancellationToken)
    {
        var command = new RefuserDemandeCommand(id);

        var demandeId = await _mediator.Send(command, cancellationToken);

        return Ok(demandeId);
    }



    [HttpGet]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<PagedResult<DemandeInterventionDto>>> GetAllDemandes(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
    {
        var query = new GetAllDemandesQuery(page, pageSize);

        var demandes = await _mediator.Send(query, cancellationToken);

        return Ok(demandes);
    }
}
