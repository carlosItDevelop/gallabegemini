using GeneralLabSolutions.Identidade.Services;
using GeneralLabSolutions.WebApiCore.Usuario;
using GeneralLabSolutions.WebApiCore.Usuario.GeneralLabSolutions.WebApiCore.Usuario;

namespace GeneralLabSolutions.Identidade.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<AuthenticationIdentityService>();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
