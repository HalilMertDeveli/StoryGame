using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MurderGame.DataAccess.Context;

namespace MurderGame.Business.DependencyResolvers.Microsoft
{
    public static class DependencyExtension
    {
        public static void AddDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(" Server=localhost; Database=MurderGameDatabase; Integrated Security=True; TrustServerCertificate=True"));

        }
    }
}
