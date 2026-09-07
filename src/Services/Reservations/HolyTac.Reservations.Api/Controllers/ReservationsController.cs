using HolyTac.Reservations.Application;
using HolyTac.Reservations.Application.Commands;
using HolyTac.Reservations.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HolyTac.Reservations.Api.Controllers;

public record CreateReservationRequest(
    string CustomerName,
    string Phone,
    string? Email,
    int PartySize,
    DateTime ReservationAtUtc,
    string? Notes);

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(ISender sender) : ControllerBase
{
    [HttpGet("availability")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetAvailability(
        [FromQuery] DateOnly date, [FromQuery] int partySize, CancellationToken cancellationToken)
    {
        var slots = await sender.Send(new GetAvailabilityQuery(date, partySize), cancellationToken);
        return Ok(slots.Select(s => s.ToString(@"hh\:mm")));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ReservationDto>>> GetByDate(
        [FromQuery] DateOnly date, CancellationToken cancellationToken)
    {
        var reservations = await sender.Send(new GetReservationsByDateQuery(date), cancellationToken);
        return Ok(reservations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReservationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await sender.Send(new GetReservationByIdQuery(id), cancellationToken);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await sender.Send(new CreateReservationCommand(
                request.CustomerName, request.Phone, request.Email, request.PartySize,
                request.ReservationAtUtc, request.Notes), cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new ConfirmReservationCommand(id), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new CancelReservationCommand(id), cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
