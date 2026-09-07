using HolyTac.Menu.Domain;
using MediatR;

namespace HolyTac.Menu.Application.Queries;

public record GetMenuQuery(MenuCategory? Category = null) : IRequest<IReadOnlyList<MenuItemDto>>;

public class GetMenuQueryHandler(IMenuItemRepository repository)
    : IRequestHandler<GetMenuQuery, IReadOnlyList<MenuItemDto>>
{
    public async Task<IReadOnlyList<MenuItemDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
    {
        var items = request.Category is null
            ? await repository.GetAllAsync(cancellationToken)
            : await repository.GetByCategoryAsync(request.Category.Value, cancellationToken);

        return items.Select(MenuItemDto.FromDomain).ToList();
    }
}
