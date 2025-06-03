using Microsoft.AspNetCore.Identity;

namespace EscolaInfoSys.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Administrator", "StaffMember", "Student" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin
            var adminEmail = "admin@school.com";
            var adminPassword = "Admin123!";
            await CreateUserIfNotExists(userManager, adminEmail, adminPassword, "Administrator");

            // (Staff)
            var staffEmail = "staff@school.com";
            var staffPassword = "Staff123!";
            await CreateUserIfNotExists(userManager, staffEmail, staffPassword, "StaffMember");

            // (Student)
            var studentEmail = "student@school.com";
            var studentPassword = "Student123!";
            await CreateUserIfNotExists(userManager, studentEmail, studentPassword, "Student");
        }

        private static async Task CreateUserIfNotExists(UserManager<ApplicationUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

