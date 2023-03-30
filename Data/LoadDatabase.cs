using Microsoft.AspNetCore.Identity;
using NetKubernetes.Models;

namespace NetKubernetes.Data;

public class LoadDatabase
{
    public static async Task InserData(AppDbContext context, UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user = new User
            {
                Name = "Mirko",
                Surname = "Wlk",
                Email = "mirkoivowlk@gmail.com",
                UserName = "mwlk",
                PhoneNumber = "3517439345"
            };

            await userManager.CreateAsync(user, "Admin1234#");

            if (!context.Properties.Any())
            {
                context.Properties.AddRange(
                    new Property
                    {
                        Name = "Casa Playa",
                        Address = "Bv San Juan 85",
                        Price = 4500M,
                        CreationDate = DateTime.Now
                    },
                    new Property
                    {
                        Name = "Casa Invierno",
                        Address = "Chaco 577",
                        Price = 2000M,
                        CreationDate = DateTime.Now
                    }
                );
            }

            context.SaveChanges();
        }
    }
}