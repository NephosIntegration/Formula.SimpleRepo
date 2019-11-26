using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Formula.SimpleRepo
{
    public static class RepositoryConfiguration
    {
        public static IEnumerable<Type> GetRepositoryList()
        {
            //var assembly = Assembly.GetExecutingAssembly();
            var assembly = Assembly.GetEntryAssembly();

            var repos = 
            from type in assembly.GetTypes()
            where type.IsDefined(typeof(Repo), false)
            select type;

            return repos;
        }

        public static void AddRepositories(this IServiceCollection services) 
        {
            foreach(var type in RepositoryConfiguration.GetRepositoryList())
            {
                services.AddScoped(type);
            }
        }
    }
}
