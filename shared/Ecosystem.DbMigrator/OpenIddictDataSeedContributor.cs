using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Ecosystem.DbMigrator;

public class OpenIddictDataSeedContributor(OpenIddictDataSeeder OpenIddictDataSeeder)
    : IDataSeedContributor,
        ITransientDependency
{
    private readonly OpenIddictDataSeeder _OpenIddictDataSeeder = OpenIddictDataSeeder;

    public Task SeedAsync(DataSeedContext context)
    {
        return _OpenIddictDataSeeder.SeedAsync();
    }
}
