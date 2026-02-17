using Application.Common;
using Infrastructure;
using IntegrationTests.Handler;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using System.Diagnostics;
using System.Threading.RateLimiting;
using Testcontainers.PostgreSql;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres =
            new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("testdb")
                .WithUsername("test")
                .WithPassword("test")
                .Build();

        private Respawner _respawner = default!;
        private string _connectionString = default!;

        public async ValueTask InitializeAsync()
        {
            await _postgres.StartAsync();
            _connectionString = _postgres.GetConnectionString();
            Debug.Write("PostgreSQL container started with connection string: " + _connectionString);

            await InitializeDatabaseAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "test-key-test-key-test-key-test-key",
                    ["Jwt:Issuer"] = "MyApp",
                    ["Jwt:Audience"] = "MyApp.Client",
                    ["TokenHash:Pepper"] = "test-pepper"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove the production DbContext registration
                services.RemoveAll<DbContextOptions<AppDbContext>>();

                // Register DbContext pointing to container Postgres
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_connectionString));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName, _ => { });

            });
        }

        private async Task InitializeDatabaseAsync()
        {
            // Apply migrations + setup Respawn
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            });
        }

        public async Task ResetDatabaseAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await _respawner.ResetAsync(conn);
        }

        public new async ValueTask DisposeAsync()
        {
            await _postgres.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
