using GestionInterventions.Application.Features.Clients.Commands.RegisterClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionInterventions.Application.DTOs;
using GestionInterventions.Application.Features.Clients.Queries.GetClientById;
using GestionInterventions.Application.Features.Clients.Queries.GetClients;
using GestionInterventions.Application.Features.Clients.Commands.UpdateClient;
using Microsoft.AspNetCore.Authorization;

namespace GestionInterventions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/clients
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<int>> RegisterClient(
        [FromBody] RegisterClientCommand command,
        CancellationToken cancellationToken)
    {
        var clientId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(RegisterClient), new { id = clientId }, clientId);
    }



    // GET: api/clients/5
    [HttpGet("{id}")]
    [Authorize(Roles = "Responsable,Client")]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var query = new GetClientByIdQuery(id);

        var client = await _mediator.Send(query);

        return Ok(client);
    }


    // GET: api/clients
    [HttpGet]
    [Authorize(Roles = "Responsable")]
    public async Task<ActionResult<List<ClientDto>>> GetClients(CancellationToken cancellationToken)
    {
        var query = new GetClientsQuery();


        var clients = await _mediator.Send(query, cancellationToken);

        return Ok(clients);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Responsable,Client")]
    public async Task<ActionResult<ClientDto>> UpdateClient(
    int id,
    [FromBody] UpdateClientDto updateDto,
    CancellationToken cancellationToken)
    {
        var command = new UpdateClientCommand(
            id,
            updateDto.Email,
            updateDto.Telephone,
            updateDto.Adresse);

        var client = await _mediator.Send(command, cancellationToken);

        return Ok(client);
    }
}