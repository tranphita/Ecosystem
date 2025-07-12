using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ecosystem.SmartBox.EntityFrameworkCore;

public class SmartBoxDbContextFactory : IDesignTimeDbContextFactory<SmartBoxDbContext>
{
    public SmartBoxDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<SmartBoxDbContext>().UseNpgsql(
            GetConnectionStringFromConfiguration()
        );

        return new SmartBoxDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        var connectionString = BuildConfiguration().GetConnectionString(SmartBoxDbProperties.ConnectionStringName);
        if (connectionString == null)
        {
            throw new InvalidOperationException("Connection string not found.");
        }
        return connectionString;
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var parentDirectory = Directory.GetParent(currentDirectory)?.Parent?.FullName;

        if (parentDirectory == null)
        {
            throw new InvalidOperationException("Unable to determine the parent directory.");
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(parentDirectory, $"host{Path.DirectorySeparatorChar}Ecosystem.SmartBox.HttpApi.Host"))
            .AddJsonFile("appsettings.json", false);

        return builder.Build();
    }
}
