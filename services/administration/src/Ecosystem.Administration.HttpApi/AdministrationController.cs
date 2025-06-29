using Ecosystem.Administration.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.Administration;

public abstract class AdministrationController : AbpControllerBase
{
    protected AdministrationController()
    {
        LocalizationResource = typeof(AdministrationResource);
    }
}
