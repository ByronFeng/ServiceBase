﻿using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceBase.IdentityServer.EntityFramework.DbContexts;
using ServiceBase.IdentityServer.EntityFramework.Interfaces;
using ServiceBase.IdentityServer.EntityFramework.Options;
using ServiceBase.IdentityServer.EntityFramework.Services;
using ServiceBase.IdentityServer.EntityFramework.Stores;
using ServiceBase.IdentityServer.Services;
using System;
using System.Reflection;

namespace ServiceBase.IdentityServer.EntityFramework
{
    public static class IServiceCollectionExtensions
    {
        internal static EntityFrameworkOptions ToOptions(this IConfigurationSection section)
        {
            var options = new EntityFrameworkOptions();
            section.Bind(options);
            return options;
        }

        internal static EntityFrameworkOptions ToOptions(this Action<EntityFrameworkOptions> configure)
        {
            var options = new EntityFrameworkOptions();
            configure?.Invoke(options);
            return options;
        }

        // https://docs.microsoft.com/en-us/ef/core/providers/
        public static void AddEntityFrameworkInMemoryStores(
            this IServiceCollection services,
            Action<EntityFrameworkOptions> configure = null)
        {
            services.Configure<EntityFrameworkOptions>(configure);
            var options = configure.ToOptions();
            services.AddEntityFrameworkStores((builder) =>
            {
                builder.UseInMemoryDatabase();
            }, options);
        }

        public static void AddEntityFrameworkSqlServerStores(
            this IServiceCollection services,
            IConfigurationSection section)
        {
            services.Configure<EntityFrameworkOptions>(section);
            var options = section.ToOptions();

            var migrationsAssembly = typeof(IServiceCollectionExtensions).GetTypeInfo().Assembly.GetName().Name;
            services.AddEntityFrameworkStores((builder) =>
            {
                builder.UseSqlServer(options.ConnectionString, o => o.MigrationsAssembly(migrationsAssembly));
            }, options);
        }

        public static void AddEntityFrameworkStores(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbContextOptionsAction,
            EntityFrameworkOptions options)
        {
            services.AddConfigurationStore(dbContextOptionsAction, options);
            services.AddOperationalStore(dbContextOptionsAction, options);
            services.AddUserAccountStore(dbContextOptionsAction, options);

            // If db inialization or example data seeding is required add a default store initializer
            if (options.MigrateDatabase)
            {
                services.AddDbContext<DefaultDbContext>(dbContextOptionsAction);
                if (options.SeedExampleData)
                {
                    services.AddTransient<IStoreInitializer, DefaultStoreInitializer>();
                }
            }

            services.AddSingleton(options);
        }

        internal static void AddConfigurationStore(
           this IServiceCollection services,
           Action<DbContextOptionsBuilder> dbContextOptionsAction,
           EntityFrameworkOptions options)
        {
            services.AddDbContext<ConfigurationDbContext>(dbContextOptionsAction);
            services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IResourceStore, ResourceStore>();
            services.AddTransient<ICorsPolicyService, CorsPolicyService>();
        }

        internal static void AddOperationalStore(
           this IServiceCollection services,
           Action<DbContextOptionsBuilder> dbContextOptionsAction,
           EntityFrameworkOptions options)
        {
            services.AddDbContext<PersistedGrantDbContext>(dbContextOptionsAction);
            services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            if (options.CleanupTokens)
            {
                services.AddSingleton<TokenCleanup>();
            }
        }

        internal static void AddUserAccountStore(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> dbContextOptionsAction,
           EntityFrameworkOptions options)
        {
            services.AddDbContext<UserAccountDbContext>(dbContextOptionsAction);
            services.AddScoped<UserAccountDbContext>();
            services.AddTransient<IUserAccountStore, UserAccountStore>();
        }
    }
}