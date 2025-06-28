using System.Reflection;
using Microsoft.OpenApi.Models;

namespace GeneralLabSolutions.Identidade.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Gal Lab Solutions Enterprise - Identity",
                    Description = "Recursos da API de Identidade do Projeto: Gal Lab Solutions Sys.",
                    Contact = new OpenApiContact() 
                    { 
                        Name = "Carlos A Santos", 
                        Email = "contato.cooperchip@gmail.com", 
                        Url = new Uri("https://cooperchip.com.br") 
                    },
                    License = new OpenApiLicense() 
                    { 
                        Name = "MIT", 
                        Url = new Uri("https://opensource.org/licenses/MIT") 
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Inclua a linha abaixo para o arquivo XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gal Lab Solutions - v1.0.0");
            });
            return app;
        }
    }
}
