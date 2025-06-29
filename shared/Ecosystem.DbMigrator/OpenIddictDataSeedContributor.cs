using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Ecosystem.DbMigrator;

public class OpenIddictDataSeedContributor(OpenIddictDataSeeder OpenIddictDataSeeder)
    : IDataSeedContributor,
        ITransientDependency
{
    private readonly OpenIddictDataSeeder _OpenIddictDataSeeder = OpenIddictDataSeeder;

    public async Task SeedAsync(DataSeedContext context)
    {
        await _OpenIddictDataSeeder.SeedAsync();
    }
}
