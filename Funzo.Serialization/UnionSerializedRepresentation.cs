namespace Funzo.Serialization;
internal class UnionSerializedRepresentation
{
    public string Tag => Value.GetType().Name;

    public object Value { get; set; }
}
