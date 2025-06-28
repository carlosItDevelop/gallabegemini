using GeneralLabSolutions.WebApiCore.Usuario;
using GeneralLabSolutions.WebApiCore.Usuario.GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.AspNetCore.Authentication.Cookies;
using VelzonModerna.Services;

namespace VelzonModerna.Configuration.Extensions
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/identidade/logar"; // Ou a rota do seu controller de login
                    options.AccessDeniedPath = "/identidade/acesso-negado"; // Rota para acesso negado
                    options.ExpireTimeSpan = TimeSpan.FromHours(8); // Mesmo tempo do authProperties
                });

            services.AddScoped<IAspNetUser, AspNetUser>();

            return services;

        }
    }
}
