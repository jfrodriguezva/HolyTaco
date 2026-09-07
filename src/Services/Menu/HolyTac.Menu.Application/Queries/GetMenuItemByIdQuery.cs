using MediatR;

namespace HolyTac.Menu.Application.Queries;

public record GetMenuItemByIdQuery(Guid Id) : IRequest<MenuItemDto?>;

public class GetMenuItemByIdQueryHandler(IMenuItemRepository repository)
    : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto?>
{
    public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        return item is null ? null : MenuItemDto.FromDomain(item);
    }
}
