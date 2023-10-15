using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlatformService.Models;

namespace PlatformService.Data;




public static class PrepDb
{
    public static void PrepPopulation(this IApplicationBuilder app, bool IsProduction)
    {

        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProduction);

        }
    }
    private static void SeedData(AppDbContext context, bool IsProduction)
    {
        if (IsProduction)
        {
            try
            {
                Console.Write("--> db creating...  ");
                context.Database.EnsureCreated();
            }
            catch (System.Exception e)
            {
                Console.Write("error");
                Console.Write(e.Message);
                throw new Exception(message: "db olusturulamadı");
            }
            Console.Write("success");

        }
        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding Data...");

            context.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We Already have data");
        }
    }
}
