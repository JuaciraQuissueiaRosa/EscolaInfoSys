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


            // Criar Curso se não existir
            Course course;
            if (!context.Courses.Any())
            {
                course = new Course { Name = "Programming", Description = "Intro to Programming" };
                context.Courses.Add(course);
                await context.SaveChangesAsync();
            }
            else
            {
                course = context.Courses.First();
            }

            FormGroup formGroup;
            if (!context.FormGroups.Any())
            {
                formGroup = new FormGroup
                {
                    Name = "1º A",
                    CourseId = course.Id // associação direta
                };
                context.FormGroups.Add(formGroup);
                await context.SaveChangesAsync();
            }
            else
            {
                formGroup = context.FormGroups.First();
            }

            if (!context.Subjects.Any())
            {
                var subjects = new List<Subject>
                {
          
                  new Subject { Name = "Mathematics", CourseId = course.Id }
                };

                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();
            }


            await CreateUserIfNotExists(userManager, context, "admin@school.com", "Admin123!", "Administrator", "Admin User");
            await CreateUserIfNotExists(userManager, context, "staff@school.com", "Staff123!", "StaffMember", "Prof. João da Silva");
            var randomPupilNumber = GenerateRandomPupilNumber(context);
            await CreateUserIfNotExists(userManager, context, "student@school.com", "Student123!", "Student", "Maria Silva", randomPupilNumber, formGroup.Id, course.Id);


            // Seed de configuração do sistema
            if (!context.SystemSettings.Any())
            {
                context.SystemSettings.Add(new SystemSettings
                {
                    MaxAbsencePercentage = 25
                });
            }

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
            int? formGroupId = null,
            int? courseId = null)
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
                        throw new Exception("Student requer StudentNumber e FormGroupId e CourseId");

                    user.ProfilePhoto = "default.png"; 

                    context.Students.Add(new Student
                    {
                        FullName = fullName,
                        Email = email,
                        PupilNumber = pupilNumber,
                        FormGroupId = formGroupId.Value,
                        CourseId = courseId.Value,
                        ApplicationUserId = user.Id,
                        ProfilePhoto = "default.png" 
                    });
                }
                await context.SaveChangesAsync();
            }
           
        }


        private static string GenerateRandomPupilNumber(ApplicationDbContext context)
        {
            var random = new Random();
            string pupilNumber;

            do
            {
                int number = random.Next(10000, 99999); // 5 dígitos
                pupilNumber = $"STU-{number}";
            }
            while (context.Students.Any(s => s.PupilNumber == pupilNumber)); // Evita duplicados

            return pupilNumber;
        }

    }

}








