namespace Funzo.Example;

public class DataAccess : IDataAccess<Todo>
{
    private static readonly List<Todo> _todos = [];

    public async Task<Result<Todo, TErr>> GetItem<TErr>(string id, CancellationToken cancellationToken) where TErr : class, IUnion<ItemNotFoundError>
    {
        var item = _todos.SingleOrDefault(t => t.Id == id);

        if (item is null)
        {
            return Result<Todo, TErr>.Err(TErr.From<TErr>(new ItemNotFoundError(id)));
        }

        return item;
    }

    public async Task<Result<TErr>> InsertItem<TErr>(Todo item, CancellationToken cancellationToken) where TErr : class, IUnion<ItemAlreadyExists>
    {
        var dbItem = _todos.SingleOrDefault(t => t.Id == item.Id);

        if (dbItem is { })
        {
            return Result<TErr>.Err(TErr.From<TErr>(new ItemAlreadyExists(item.Id)));
        }

        return Result<TErr>.Ok();
    }
}
