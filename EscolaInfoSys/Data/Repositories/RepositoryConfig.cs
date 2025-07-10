using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Services;

namespace EscolaInfoSys.Data.Repositories
{
    public static class RepositoryConfig
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IFormGroupRepository, FormGroupRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
            services.AddScoped<IMarkRepository, MarkRepository>();
            services.AddScoped<IAbsenceRepository, AbsenceRepository>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<AbsenceCheckerService>();
            services.AddScoped<IStudentExclusionRepository, StudentExclusionRepository>();
            services.AddScoped<ISystemSettingsRepository, SystemSettingsRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }
}
