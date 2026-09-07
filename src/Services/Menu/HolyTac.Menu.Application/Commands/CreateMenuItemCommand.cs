using HolyTac.Menu.Domain;
using MediatR;

namespace HolyTac.Menu.Application.Commands;

public record CreateMenuItemCommand(
    string Name,
    string Description,
    decimal Price,
    MenuCategory Category,
    bool IsSpicy,
    string? ImageUrl) : IRequest<Guid>;

public class CreateMenuItemCommandHandler(IMenuItemRepository repository)
    : IRequestHandler<CreateMenuItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = MenuItem.Create(
            request.Name,
            request.Description,
            new Money(request.Price),
            request.Category,
            request.IsSpicy,
            request.ImageUrl);

        await repository.AddAsync(item, cancellationToken);
        return item.Id;
    }
}
