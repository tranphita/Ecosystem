using Ecosystem.SmartBox.Localization;
using Volo.Abp.Application.Services;

namespace Ecosystem.SmartBox;

    public abstract class SmartBoxAppService : ApplicationService
    {
        protected SmartBoxAppService()
        {
            LocalizationResource = typeof(SmartBoxResource);
            ObjectMapperContext = typeof(SmartBoxApplicationModule);
    }
}
