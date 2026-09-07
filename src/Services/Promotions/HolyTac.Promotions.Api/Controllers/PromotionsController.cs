using HolyTac.Promotions.Application;
using HolyTac.Promotions.Application.Commands;
using HolyTac.Promotions.Application.Queries;
using HolyTac.Promotions.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HolyTac.Promotions.Api.Controllers;

public record CreatePromotionRequest(
    string Title,
    string Description,
    string? ImageUrl,
    DiscountType DiscountType,
    decimal? DiscountValue,
    decimal? ComboPrice,
    List<Guid> MenuItemIds,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    bool IsFeatured);

[ApiController]
[Route("api/[controller]")]
public class PromotionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PromotionDto>>> Get(
        [FromQuery] bool onlyActive, CancellationToken cancellationToken)
    {
        return Ok(onlyActive
            ? await sender.Send(new GetActivePromotionsQuery(), cancellationToken)
            : await sender.Send(new GetAllPromotionsQuery(), cancellationToken));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IReadOnlyList<PromotionDto>>> GetFeatured(CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetFeaturedPromotionsQuery(), cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PromotionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await sender.Send(new GetPromotionByIdQuery(id), cancellationToken);
        return promotion is null ? NotFound() : Ok(promotion);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePromotionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await sender.Send(new CreatePromotionCommand(
                request.Title, request.Description, request.ImageUrl, request.DiscountType,
                request.DiscountValue, request.ComboPrice, request.MenuItemIds,
                request.StartsAtUtc, request.EndsAtUtc, request.IsFeatured), cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivatePromotionCommand(id), cancellationToken);
        return NoContent();
    }
}
