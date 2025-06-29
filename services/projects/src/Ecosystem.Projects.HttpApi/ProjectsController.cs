using Ecosystem.Projects.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ecosystem.Projects;

public abstract class ProjectsController : AbpControllerBase
{
    protected ProjectsController()
    {
        LocalizationResource = typeof(ProjectsResource);
    }
}
