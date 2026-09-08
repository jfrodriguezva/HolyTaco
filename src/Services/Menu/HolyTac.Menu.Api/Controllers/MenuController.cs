using HolyTac.Menu.Application;
using HolyTac.Menu.Application.Commands;
using HolyTac.Menu.Application.Queries;
using HolyTac.Menu.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HolyTac.Menu.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MenuItemDto>>> GetMenu(
        [FromQuery] MenuCategory? category, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMenuQuery(category), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MenuItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMenuItemByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateMenuItemCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var id = await sender.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
