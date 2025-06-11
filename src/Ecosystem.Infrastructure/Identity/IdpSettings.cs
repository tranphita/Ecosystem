namespace Ecosystem.Infrastructure.Identity;
public class IdpSettings
{
    public const string SectionName = "IdpSettings";
    public string BaseUrl { get; init; } = null!;
}
