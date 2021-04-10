using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Formula.SimpleRepo
{
    public static class RepositoryConfiguration
    {
        public static IEnumerable<Type> GetRepositoryList(Assembly assembly)
        {
            var repos = 
            from type in assembly.GetTypes()
            where type.IsDefined(typeof(Repo), false)
            select type;

            return repos;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, Type repositoryAssemblyType = null)
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
            
            return RepositoryConfiguration.AddRepositories(services, assembly);
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, Assembly assembly)
        {
            foreach(var type in RepositoryConfiguration.GetRepositoryList(assembly))
            {
                // services.AddScoped(type);
                services.AddTransient(type);
            }

            return services;
        }
    }
}
