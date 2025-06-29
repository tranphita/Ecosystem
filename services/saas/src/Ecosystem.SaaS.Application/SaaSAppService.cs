using Ecosystem.SaaS.Localization;
using Volo.Abp.Application.Services;

namespace Ecosystem.SaaS;

public abstract class SaaSAppService : ApplicationService
{
    protected SaaSAppService()
    {
        LocalizationResource = typeof(SaaSResource);
        ObjectMapperContext = typeof(SaaSApplicationModule);
    }
}
