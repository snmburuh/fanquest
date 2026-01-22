using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FanQuest.Infrastructure.Persistence
{
    public class FanQuestDbContextFactory : IDesignTimeDbContextFactory<FanQuestDbContext>
    {
        public FanQuestDbContext CreateDbContext(string[] args)
        {
            // Build configuration - pointing to API project's appsettings.json
            var apiProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FanQuest.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Create DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<FanQuestDbContext>();
            var connectionString = configuration.GetConnectionString("FanQuestDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'FanQuestDb' not found in appsettings.json. " +
                    $"Looking in: {apiProjectPath}");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new FanQuestDbContext(optionsBuilder.Options);
        }
    }
}
