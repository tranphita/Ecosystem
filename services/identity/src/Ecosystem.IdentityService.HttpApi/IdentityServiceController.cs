using Ecosystem.IdentityService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.IdentityService;

public abstract class IdentityServiceController : AbpControllerBase
{
    protected IdentityServiceController()
    {
        LocalizationResource = typeof(IdentityServiceResource);
    }
}
