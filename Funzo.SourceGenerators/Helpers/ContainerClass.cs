namespace Funzo.SourceGenerators.Helpers;

public sealed class ContainerClass(string name, bool isStatic)
{
    public string Name { get; } = name;
    public bool IsStatic { get; } = isStatic;
}
