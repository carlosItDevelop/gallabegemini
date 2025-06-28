using GeneralLabSolutions.Identidade.Data;
using GeneralLabSolutions.Identidade.Extensions;
using GeneralLabSolutions.WebApiCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.Identidade.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {

            var appSettingsSection = configuration.GetSection(nameof(AppTokenSettings));
            services.Configure<AppTokenSettings>(appSettingsSection);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetDefaultConnectionString()));

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddErrorDescriber<IdentityMensagensPortugues>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Configurações de senha
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;

                // Configurações de bloqueio
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configurações de usuário
                options.User.RequireUniqueEmail = true;
            });


            // Esta linha, abaixo, deve ser inserida após registrar o Identity;
            services.AddScoped<RoleManager<IdentityRole>>();

            return services;
        }
    }
}
