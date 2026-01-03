namespace Funzo.Example;

public interface Item
{
    public string Id { get; }
}

public interface IDataAccess<T>
    where T : Item
{
    Task<Result<T, TErr>> GetItem<TErr>(string id, CancellationToken cancellationToken)
        where TErr : class, IUnion<ItemNotFoundError>;

    Task<Result<TErr>> InsertItem<TErr>(T item, CancellationToken cancellationToken)
        where TErr : class, IUnion<ItemAlreadyExists>;
}


public record ItemNotFoundError(string Id);
public record ItemAlreadyExists(string Id);