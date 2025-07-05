using Ecosystem.SmartBox.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.SmartBox;

public abstract class SmartBoxController : AbpControllerBase
{
    protected SmartBoxController()
    {
        LocalizationResource = typeof(SmartBoxResource);
    }
}
