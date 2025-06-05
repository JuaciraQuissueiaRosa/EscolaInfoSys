using EscolaInfoSys.Data.Repositories.Interfaces;

namespace EscolaInfoSys.Data.Repositories
{
    public static class RepositoryConfig
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IFormGroupRepository, FormGroupRepository>();
            services.AddScoped<IMarkRepository, MarkRepository>();
            services.AddScoped<IAbsenceRepository, AbsenceRepository>();
            services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
        }
    }
}
