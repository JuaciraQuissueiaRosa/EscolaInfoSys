using EscolaInfoSys.Data.Repositories.Interfaces;

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

            return services;
        }
    }
}
