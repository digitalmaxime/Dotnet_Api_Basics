namespace OptionsPatternComplexeClass.Options;

public class SystemsOption
{
    public const string Name = "Systems";

    public Dictionary<string, Uri>? Dict { get; set; } = default;
}