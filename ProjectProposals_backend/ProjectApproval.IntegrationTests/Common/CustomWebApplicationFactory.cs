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

        // Apply migrations to the test database
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProjectApprovalDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
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
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
