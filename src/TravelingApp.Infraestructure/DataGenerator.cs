using Microsoft.AspNetCore.Identity;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Constants;
using TravelingApp.Application.DependencyInjection;
using TravelingApp.Domain.Entities;
using TravelingApp.Infraestructure.Context;

namespace TravelingApp.Infraestructure
{
    [ScopedService(typeof(IDataSeeder))]
    public class DataGenerator(
        TravelingAppDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager) : IDataSeeder
    {
        public async Task SeedAsync()
        {
            if (context.Users.Any() && context.Roles.Any()) return;

            var roles = new List<IdentityRole>
            {
                new() { Id = "47cab13e-ff81-4f9f-b040-7c4994bef9d3", Name = AppConstant.RolAdmin, NormalizedName = AppConstant.RolAdmin.ToUpper() },
                new() { Id = "e0d547ff-3c97-4c0d-a03b-d9613509b4af", Name = AppConstant.RolCustomer, NormalizedName = AppConstant.RolCustomer.ToUpper() },
                new() { Id = "820c2aa2-f18d-4eba-b1d6-687537cc0eba", Name = AppConstant.RolSupplier, NormalizedName = AppConstant.RolSupplier.ToUpper() },
            };

            await context.Roles.AddRangeAsync(roles);

            var users = new List<(string idusuario, string username, string email, string password, string role)>
            {
                ("1234567A", "admin", "admin@test.com", "Admin123!", "admin")
            };

            foreach (var (idusuario, username, email, password, role) in users)
            {
                var user = new User { Id = idusuario, UserName = username, Email = email };

                var result = await userManager.CreateAsync(user, password: password);
                if (result.Succeeded)
                {
                    var identityRole = await roleManager.FindByNameAsync(role);
                    if (identityRole == null)
                    {
                        identityRole = new IdentityRole(role);
                        await roleManager.CreateAsync(identityRole);
                    }

                    await userManager.AddToRoleAsync(user, role);
                }
            }

            await context.SaveChangesAsync();

            await SeedDestinationsAsync();
        }

        private async Task SeedDestinationsAsync()
        {
            if (context.Destinations.Any()) return;

            var destinations = new List<Destination>
            {
                new()
                {
                    Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"),
                    Name = "Playa de Cancún",
                    Description = "Hermosas playas de arena blanca y aguas turquesa en la Riviera Maya",
                    Country = "México",
                    ImageUrl = "local:destination_cancun",
                    Category = "beach",
                    CreatedBy = "1234567A"
                },
                new()
                {
                    Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"),
                    Name = "Alpes Suizos",
                    Description = "Montañas nevadas espectaculares con vistas panorámicas",
                    Country = "Suiza",
                    ImageUrl = "local:destination_alps",
                    Category = "mountain",
                    CreatedBy = "1234567A"
                }
            };

            await context.Destinations.AddRangeAsync(destinations);
            await context.SaveChangesAsync();
        }
    }
}
