using Ecosystem.SaaS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.SaaS;

public abstract class SaaSController : AbpControllerBase
{
    protected SaaSController()
    {
        LocalizationResource = typeof(SaaSResource);
    }
}
