using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Ecosystem.IdentityService;

public class IdentityServiceDataSeedContributor(ICurrentTenant currentTenant)
    : IDataSeedContributor,
        ITransientDependency
{
    private readonly ICurrentTenant _currentTenant = currentTenant;

    public Task SeedAsync(DataSeedContext context)
    {
        /* Instead of returning the Task.CompletedTask, you can insert your test data
         * at this point!
         */

        using (_currentTenant.Change(context?.TenantId))
        {
            return Task.CompletedTask;
        }
    }
}
