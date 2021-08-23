using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Formula.SimpleRepo
{
    public static class RepositoryConfiguration
    {
        public static IEnumerable<Type> GetRepositoryList(Assembly assembly)
        {
            var repos = assembly.GetExportedTypes()
                                .Where(e => e.IsDefined(typeof(Repo), false))
                                .ToList();
            return repos;
        }

        public static IServiceCollection AddRepositoryByType(this IServiceCollection services, Type repositoryAssemblyType = null)
        {
            Assembly assembly = null;

            if (repositoryAssemblyType == null)
            {
                //assembly = Assembly.GetExecutingAssembly();
                assembly = Assembly.GetEntryAssembly();
            }
            else
            {
                assembly = repositoryAssemblyType.GetTypeInfo().Assembly;
            }

            return AddRepositoriesInAssembly(services, assembly);
        }

        public static IServiceCollection AddRepositoriesInAssembly(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in GetRepositoryList(assembly))
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
}
