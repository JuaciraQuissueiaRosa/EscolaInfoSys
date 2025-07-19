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
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            
            FormGroup formGroup;
            if (!context.FormGroups.Any())
            {
                formGroup = new FormGroup { Name = "1º A" };
                context.FormGroups.Add(formGroup);
                await context.SaveChangesAsync();
            }
            else
            {
                formGroup = context.FormGroups.First();
            }

          
            await CreateUserIfNotExists(userManager, context, "admin@school.com", "Admin123!", "Administrator", "Admin User");
            await CreateUserIfNotExists(userManager, context, "staff@school.com", "Staff123!", "StaffMember", "Prof. João da Silva");
            await CreateUserIfNotExists(userManager, context, "student@school.com", "Student123!", "Student", "Maria Silva", "STU001", formGroup.Id);

            await context.SaveChangesAsync();
        }

        private static async Task CreateUserIfNotExists(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            string email,
            string password,
            string role,
            string fullName,
            string? pupilNumber = null,
            int? formGroupId = null)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new Exception($"Erro ao criar {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                await userManager.AddToRoleAsync(user, role);

                if (role == "StaffMember")
                {
                    context.StaffMembers.Add(new StaffMember
                    {
                        FullName = fullName,
                        Email = email,
                        ApplicationUserId = user.Id
                    });
                }
                else if (role == "Student")
                {
                    if (string.IsNullOrEmpty(pupilNumber) || formGroupId == null)
                        throw new Exception("Student requer PupilNumber e FormGroupId");

                    context.Students.Add(new Student
                    {
                        FullName = fullName,
                        Email = email,
                        PupilNumber = pupilNumber,
                        FormGroupId = formGroupId.Value,
                        ApplicationUserId = user.Id
                    });
                }
            }
        }
    }

  }








