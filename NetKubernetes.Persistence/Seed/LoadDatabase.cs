using Microsoft.AspNetCore.Identity;
using NetKubernetes.Models;

namespace NetKubernetes.Persistence;

public class LoadDatabase
{
    public static async Task InserData(AppDbContext context, UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new User
            {
                Name = "Admin",
                Surname = "Default",
                Email = "mirkoivowlk@gmail.com",
                UserName = "admin",
                Phone = "3517439345"
            };

            await userManager.CreateAsync(user, "CordobaDev123#");
        }

        if (!context.Properties!.Any())
        {
            context.Properties!.AddRange(
                new Property
                {
                    Name = "Casa",
                    Address = "Santa Rosa 156",
                    Price = 110m,
                    CreationDate = DateTime.Now
                },
                new Property
                {
                    Name = "Trabajo",
                    Address = "Calasanz 80",
                    Price = 0m,
                    CreationDate = DateTime.Now
                }
            );

            await context.SaveChangesAsync();
        }
    }
}