using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.Interventions.Commands.CreateIntervention;
using GestionInterventions.Application.Features.Interventions.Commands.ModifierPlanification;
using GestionInterventions.Application.Features.Interventions.Commands.CommencerIntervention;
using GestionInterventions.Application.Features.Interventions.Commands.TerminerIntervention;
using GestionInterventions.Application.Features.Interventions.Commands.ValiderIntervention;
using GestionInterventions.Application.Features.Interventions.Commands.DemanderCorrectionIntervention;
using GestionInterventions.Application.Features.Interventions.Queries.GetMesInterventions;
using GestionInterventions.Application.Features.Interventions.Queries.GetInterventionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionInterventions.Application.Common.Interfaces;
using GestionInterventions.Domain.Exceptions;
using GestionInterventions.Application.Features.Interventions.Queries.GetAllInterventions;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterventionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public InterventionsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }



    [HttpPost]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> CreateIntervention(
        [FromBody] CreateInterventionCommand command,
        CancellationToken cancellationToken)
    {
        var interventionId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(CreateIntervention), new { id = interventionId }, interventionId);
    }



    [HttpPut("{interventionId}")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<InterventionDto>> ModifierPlanification(
    int interventionId,
    [FromBody] UpdateInterventionDto updateDto,
    CancellationToken cancellationToken)
    {
        var command = new ModifierPlanificationCommand(interventionId, updateDto.DatePrevue);

        var intervention = await _mediator.Send(command, cancellationToken);

        return Ok(intervention);
    }


    [HttpGet("mes-interventions")]
    [Authorize(Roles = "Technicien")]
    public async Task<ActionResult<List<InterventionDto>>> GetMesInterventions(CancellationToken cancellationToken)
    {
        var technicienId = _currentUserService.TechnicienId ?? throw new ForbiddenAccessException("Identifiant technicien introuvable dans le token.");

        var query = new GetMesInterventionsQuery(technicienId);

        var mesInterventions = await _mediator.Send(query, cancellationToken);

        return Ok(mesInterventions);
    }


    [HttpPut("{interventionId}/commencer")]
    [Authorize(Roles = "Technicien")]
    public async Task<ActionResult<int>> CommencerIntervention(int interventionId, CancellationToken cancellationToken)
    {
        var command = new CommencerInterventionCommand(interventionId);

        var intervention = await _mediator.Send(command, cancellationToken);

        return Ok(intervention);
    }


    [HttpPut("{interventionId}/terminer")]
    [Authorize(Roles = "Technicien")]
    public async Task<ActionResult<int>> TerminerIntervention(int interventionId, [FromBody] TerminerInterventionDto dto, CancellationToken cancellationToken)
    {
        var command = new TerminerInterventionCommand(interventionId, dto.OperationsEffectuees, dto.Observations, dto.Resultat, dto.Recommandations);

        var intervention = await _mediator.Send(command, cancellationToken);

        return Ok(intervention);
    }



    [HttpPut("{interventionId}/valider")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> ValiderIntervention(int interventionId, CancellationToken cancellationToken)
    {
        var command = new ValiderInterventionCommand(interventionId);

        var intervention = await _mediator.Send(command, cancellationToken);

        return Ok(intervention);
    }


    [HttpPut("{interventionId}/demander-correction")]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<int>> DemanderCorrectionIntervention(int interventionId, CancellationToken cancellationToken)
    {
        var command = new DemanderCorrectionInterventionCommand(interventionId);

        var intervention = await _mediator.Send(command, cancellationToken);

        return Ok(intervention);
    }


    [HttpGet("{interventionId}")]
    [Authorize(Roles = "Responsable,Client,Technicien")]
    public async Task<ActionResult<InterventionDto>> GetInterventionById(int interventionId, CancellationToken cancellationToken)
    {
        var query = new GetInterventionByIdQuery(interventionId);

        var intervention = await _mediator.Send(query, cancellationToken);

        return Ok(intervention);
    }


    [HttpGet]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<List<InterventionDto>>> GetAllInterventions(CancellationToken cancellationToken)
    {
        var query = new GetAllInterventionsQuery();
        var interventions = await _mediator.Send(query, cancellationToken);
        return Ok(interventions);
    }
}
