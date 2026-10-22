using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace ProjectApproval.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("test_project_approval_db")
        .WithUsername("test_user")
        .WithPassword("test_password_123!")
        .WithCleanUp(true)
        .Build();

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        try
        {
            await _dbContainer.StartAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "No se pudo iniciar el contenedor PostgreSQL con Testcontainers. " +
                "Verifica que Docker Desktop (o el daemon de Docker) esté abierto y ejecutándose en el equipo.", ex);
        }

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectApprovalDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:LocalProjectConnection", _dbContainer.GetConnectionString());
        builder.UseSetting("JwtSettings:SecretKey", "SuperSecretJwtKeyForIntegrationTestsAtLeast32BytesLong!");
        builder.UseSetting("JwtSettings:Issuer", "ProjectProposalsApi");
        builder.UseSetting("JwtSettings:Audience", "ProjectProposalsClient");
        builder.UseSetting("JwtSettings:AccessTokenExpirationMinutes", "60");
        builder.UseSetting("JwtSettings:RefreshTokenExpirationDays", "7");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:LocalProjectConnection"] = _dbContainer.GetConnectionString(),
                ["JwtSettings:SecretKey"] = "SuperSecretJwtKeyForIntegrationTestsAtLeast32BytesLong!",
                ["JwtSettings:Issuer"] = "ProjectProposalsApi",
                ["JwtSettings:Audience"] = "ProjectProposalsClient",
                ["JwtSettings:AccessTokenExpirationMinutes"] = "60",
                ["JwtSettings:RefreshTokenExpirationDays"] = "7"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProjectApprovalDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ProjectApprovalDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
