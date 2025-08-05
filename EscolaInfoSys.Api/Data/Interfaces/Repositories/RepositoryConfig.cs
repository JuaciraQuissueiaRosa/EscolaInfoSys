using EscolaInfoSys.Api.Services;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Services;

namespace EscolaInfoSys.Api.Data.Interfaces.Repositories
{
    public static class RepositoryConfig
    {
     
        public static IServiceCollection RegisterApiRepositories(this IServiceCollection services)
        {
            // Apenas o necessário para a API
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentExclusionRepository, StudentExclusionRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<IFormGroupRepository, FormGroupRepository>();
            services.AddScoped<IApiAccountService, ApiAccountService>();
            services.AddScoped<IStudentExclusionStatusService, StudentExclusionStatusService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }

}
