using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MurderGame.DataAccess.Context;

namespace MurderGame.Business.DependencyResolvers.Microsoft
{
    public static class DependencyExtension
    {
        public static void AddDependencies(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}