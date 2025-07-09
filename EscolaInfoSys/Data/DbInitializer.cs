using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Identity;

namespace EscolaInfoSys.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

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
            await CreateUserIfNotExists(userManager, context, adminEmail, adminPassword, "Administrator");

            // Staff
            var staffEmail = "staff@school.com";
            var staffPassword = "Staff123!";
            await CreateUserIfNotExists(userManager, context, staffEmail, staffPassword, "StaffMember");

            // Student
            var studentEmail = "student@school.com";
            var studentPassword = "Student123!";
            await CreateUserIfNotExists(userManager, context, studentEmail, studentPassword, "Student");

            await context.SaveChangesAsync(); // Salva tudo no final
        }

        private static async Task CreateUserIfNotExists(UserManager<ApplicationUser> userManager, ApplicationDbContext context, string email, string password, string role)
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

                    // Adicionar à tabela StaffMembers ou Students
                    if (role == "StaffMember" && !context.StaffMembers.Any(s => s.ApplicationUserId == user.Id))
                    {
                        context.StaffMembers.Add(new StaffMember
                        {
                            FullName = "Prof. João da Silva",
                            Email = email,
                            ApplicationUserId = user.Id
                        });
                    }
                    else if (role == "Student" && !context.Students.Any(s => s.ApplicationUserId == user.Id))
                    {
                        context.Students.Add(new Student
                        {
                            FullName = "Maria Silva",
                            Email = "student@school.com",
                            PupilNumber = "STU001", 
                            FormGroupId = 1
                        });
                    }
                }
            }
        }
    }

}


