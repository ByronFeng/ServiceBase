﻿

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.EntityFramework.Entities;
using ServiceBase.IdentityServer.EntityFramework.Extensions;
using ServiceBase.IdentityServer.EntityFramework.Interfaces;
using ServiceBase.IdentityServer.EntityFramework.Options;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.EntityFramework
{
    // Only for `dotnet ef migrations` command
    // dotnet ef migrations add init --context DefaultDbContext
    public class Startup
    {
        static void Main()
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DefaultDbContext>((builder) =>
            {
                builder.UseSqlServer("just for migration creation purposes");
            });
        }
    }

    public class DefaultDbContext : DbContext, IConfigurationDbContext, IPersistedGrantDbContext, IUserAccountDbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<IdentityResource> IdentityResources { get; set; }
        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<UserAccountClaim> UserAccountClaims { get; set; }

        public DefaultDbContext()
        {

        }

        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {

        }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureClientContext(new ConfigurationStoreOptions());
            modelBuilder.ConfigureResourcesContext(new ConfigurationStoreOptions());
            modelBuilder.PersistedGrantDbContext(new PersistentGrantStoreOptions());
            modelBuilder.UserAccountDbContext(new UserAccountStoreOptions());

            base.OnModelCreating(modelBuilder);
        }
    }
}
