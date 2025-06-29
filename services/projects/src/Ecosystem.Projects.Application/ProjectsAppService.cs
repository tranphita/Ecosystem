using Ecosystem.Projects.Localization;
using Volo.Abp.Application.Services;

namespace Ecosystem.Projects;

public abstract class ProjectsAppService : ApplicationService
{
    protected ProjectsAppService()
    {
        LocalizationResource = typeof(ProjectsResource);
        ObjectMapperContext = typeof(ProjectsApplicationModule);
    }
}
