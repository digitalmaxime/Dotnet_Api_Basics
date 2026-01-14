namespace WebConfigurationDemo;

public record ComplexSetting
{
    public required string SubSetting1 { get; init; }
    public required string SubSetting2 { get; init; }
}