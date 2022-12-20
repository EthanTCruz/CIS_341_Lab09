using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Lab8.Models;

namespace Lab8.Data
{
    public class InitializeUsersRoles
    {
        private readonly static string ManagerRole = "Manager";
        private readonly static string Password = "Admin123!";

        private readonly static string CustomerRole = "Customer";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AuthenticationContext(serviceProvider.GetRequiredService<DbContextOptions<AuthenticationContext>>()))
            {
                var ManagerID = await EnsureUser(serviceProvider, Password,"Manager", "manager@gmail.com");
                await EnsureRole(serviceProvider, ManagerID, ManagerRole);

                var CustomerID = await EnsureUser(serviceProvider, Password,"Customer", "customer@gmail.com");
                await EnsureRole(serviceProvider, CustomerID, CustomerRole);

            }
        }

        // Check that user exists with provided email address --> create new user if none exists
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userPw, string UserName,string Email)
        {
            // Access the UserManager service
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            if (userManager != null)
            {
                // Find user by email address
                var user = await userManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    // Create new user if none exists
                    user = new ApplicationUser { UserName = UserName, Email=Email };
                    await userManager.CreateAsync(user, userPw);
                }

                // Confirm the new user so that we can log in
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                return user.Id;
            }
            else
                throw new Exception("userManager null");
        }

        // Check that role exists --> create new rule if none exists
        private static async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            // Access RoleManager service
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager != null)
            {
                // Check whether role exists --> if not, create new role with the provided role name
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Retrieve user with the provided ID and add to the specified role
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager != null)
                {
                    var user = await userManager.FindByIdAsync(uid);
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                    throw new Exception("userManager null");

            }
            else
                throw new Exception("roleManager null");
        }
    }
}
