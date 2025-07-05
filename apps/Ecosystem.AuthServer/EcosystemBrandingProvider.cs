using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Ecosystem;

[Dependency(ReplaceServices = true)]
public class EcosystemBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Ecosystem";
}
