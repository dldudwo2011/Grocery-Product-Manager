using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Backend.BLL;
using Backend.DAL;

namespace Backend
{
    public static class StartupExtensions
    {
        public static void AddBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<GrocerylistContext>(options);
            services.AddTransient<GroceryListService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<GrocerylistContext>();
                return new GroceryListService(context);
            });
        }
    }
}
