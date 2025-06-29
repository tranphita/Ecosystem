using Ecosystem.Administration.Localization;
using Volo.Abp.Application.Services;

namespace Ecosystem.Administration;

public abstract class AdministrationAppService : ApplicationService
{
    protected AdministrationAppService()
    {
        LocalizationResource = typeof(AdministrationResource);
        ObjectMapperContext = typeof(AdministrationApplicationModule);
    }
}
