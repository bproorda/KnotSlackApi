using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using signalrApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Data
{
    public class PrepDb
    {
        public static void PrepDatabase(IApplicationBuilder app)
        {

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<knotSlackDbContext>());
            }
        }

        public static void SeedData(knotSlackDbContext context)
        {
            System.Console.WriteLine("Applying Migrations... ");

            context.Database.Migrate();

            if (!context.Channel.Any())
            {
                System.Console.WriteLine("Seeding Data... ");

                context.Channel.AddRange(
                    new Channel() { Name="General", Type="General"}
                );

                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("Already have data, seeding canceled.");
            }

        }
    }
}
