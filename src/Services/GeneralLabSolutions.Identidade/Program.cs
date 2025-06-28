
using GeneralLabSolutions.Identidade.Configuration;
using GeneralLabSolutions.WebApiCore.Identidade;

namespace GeneralLabSolutions.Identidade
{
    public class Program
    {
        public static async Task Main(string [] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Carregar configura��es
            var configuration = builder.Configuration;
            configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (builder.Environment.IsDevelopment())
            {
                configuration.AddUserSecrets<Program>();
            }

            // Adicionar servi�os
            builder.Services.AddIdentityConfiguration(configuration);

            builder.Services.AddApiConfiguration();

            builder.Services.AddSwaggerConfiguration();

            // JwtConfig
            builder.Services.AddJwtConfiguration(configuration);


            /*
             * CORS - Cross-Origin Resource Sharing (Compartilhamento de recursos com origens diferentes) � um mecanismo que usa cabe�alhos adicionais HTTP para informar a um navegador que permita que um aplicativo Web seja executado em uma origem (dom�nio) com permiss�o para acessar recursos selecionados de um servidor em uma origem distinta. Um aplicativo Web executa uma requisi��o cross-origin HTTP ao solicitar um recurso que tenha uma origem diferente (dom�nio, protocolo e porta) da sua pr�pria origem.
             * 
             * Leia mais no link abaixo, pois ele traz todas as informa��es necess�rias para a sua comprees�o e aplica��o.
             * https://developer.mozilla.org/pt-BR/docs/Web/HTTP/CORS
             * 
             */
            var allowedOrigins = "_totalAllowedOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(allowedOrigins,
                                      builder =>
                                      {
                                          //builder.WithOrigins("http://localhost:4503")
                                          builder.AllowAnyOrigin()
                                                 .AllowAnyHeader()
                                                 .AllowAnyMethod();
                                      });
            });



            //var allowedOrigins = "_totalAllowedOrigins";
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(name: allowedOrigins,
            //        policy =>
            //        {
            //            policy.WithOrigins("https://gallab.cooperchip.com.br")
            //                  .AllowAnyHeader()
            //                  .AllowAnyMethod();
            //        });
            //});

            var app = builder.Build();


            // Create a service scope to get an AppDbContext instance using DI and seed the database.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await SeedDataUsersAndRoles.InitializeAsync(services, configuration); // Aguardar a conclus�o    
            }

            // Configura��o do middleware
            app.UseSwaggerConfiguration();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UsePathBase("/api"); // define o path base

            app.UseHttpsRedirection();


            app.UseCors(allowedOrigins);

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
