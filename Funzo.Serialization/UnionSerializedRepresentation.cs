namespace Funzo.Serialization;

internal class UnionSerializedRepresentation
{
    public string Tag => Value.GetType().Name;

    public required object Value { get; set; }
}
