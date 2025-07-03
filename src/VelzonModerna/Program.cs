using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Configurations; // Presumo que seja usado por AddServicesAndDepencencyInjections
using GeneralLabSolutions.Domain.Interfaces;   // Presumo que seja usado por AddServicesAndDepencencyInjections
using GeneralLabSolutions.Domain.Services;      // Presumo que seja usado por AddServicesAndDepencencyInjections
using GeneralLabSolutions.Domain.Settings;      // Para OpenAISettings
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.IoC;
using GeneralLabSolutions.InfraStructure.Repository; // Para DbInitializer
using GeneralLabSolutions.WebApiCore.Extensions;  // Para AddIdentityConfiguration (IAspNetUser)
using GeneralLabSolutions.WebApiCore.Usuario;     // Para IAspNetUser
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Configuration.Extensions;
using VelzonModerna.Configuration.Mappings;
using VelzonModerna.Services;
// using VelzonModerna.Workers; // Comentado no seu original

public class Program
{
    public static async Task Main(string [] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configura��es de Configuration
        var configuration = builder.Configuration;
        configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        if (builder.Environment.IsDevelopment())
        {
            configuration.AddUserSecrets<Program>();
        }

        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // Add services to the container.
        builder.Services.AddMvcConfiguration(); // Seu m�todo de extens�o para MVC

         builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        // Inje��es de depend�ncia do seu projeto de Infraestrutura e Dom�nio
        builder.Services.AddServicesAndDepencencyInjections(configuration);

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

        // Valida��o de CPF
        builder.Services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();

        // Pagina��o (se usado no MVC)
        builder.Services.AddScoped<IPaginationService, PaginationService>(); // Se for usado pelo MVC, sen�o remover.

        // Configura��es de Autentica��o e Identity do ASP.NET Core (Cookies, etc.)
        builder.Services.AddIdentityConfiguration(); // Registra IAspNetUser e configura��o de Cookie Auth.

        // Configura��o do AppSettingsMvc
        builder.Services.Configure<AppSettingsMvc>(options =>
        {
            options.AutenticacaoUrl = configuration.GetValue<string>("AutenticacaoUrl");
            options.UserAdminUrl = configuration.GetValue<string>("UserAdminUrl");
        });

        // --- Configura��o dos HttpClients para Servi�os ---

        // Handler de Autoriza��o (Adiciona token JWT �s requisi��es)
        // Deve ser Transient porque ele mesmo resolve servi�os Scoped (IAutenticacaoService)
        builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        // HttpClient para IAutenticacaoService (N�O usa o HttpClientAuthorizationDelegatingHandler)
        // As chamadas de Login, Registro, RefreshToken n�o enviam o token JWT Bearer.
        builder.Services.AddHttpClient<IAutenticacaoService, AutenticacaoService>()
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 3))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpCircuitBreakerPolicy(eventsAllowedBeforeBreaking: 5, durationOfBreakInSeconds: 30))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 30)); // Timeout menor para auth?

        // HttpClient para IUserAdminMvcService (USA o HttpClientAuthorizationDelegatingHandler)
        builder.Services.AddHttpClient<IUserAdminMvcService, UserAdminMvcService>(client =>
        {
            // Obt�m a URL do UserAdmin da configura��o
            var userAdminApiUrl = configuration.GetValue<string>("UserAdminUrl");
            if (string.IsNullOrEmpty(userAdminApiUrl))
            {
                // Lan�ar uma exce��o mais informativa ou logar um erro grave.
                // O servi�o n�o funcionar� corretamente sem esta URL.
                throw new InvalidOperationException("UserAdminUrl n�o est� configurada corretamente no appsettings/secrets e � necess�ria para UserAdminMvcService.");
            }
            client.BaseAddress = new Uri(userAdminApiUrl.TrimEnd('/') + "/");
        })
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() // Adiciona o token
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 3))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpCircuitBreakerPolicy(eventsAllowedBeforeBreaking: 5, durationOfBreakInSeconds: 30))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 60));

        // HttpClient para IOpenAIService (Decida se precisa de token ou n�o)
        // Se n�o precisar de token JWT, n�o adicione o handler.
        builder.Services.AddHttpClient<IOpenAIService, OpenAIService>()
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 2)) // Exemplo de pol�ticas
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 120)); // OpenAI pode demorar
        builder.Services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));


        var app = builder.Build();

        // Chamada para o DbInitializer (se este for o projeto que inicializa o BD)
        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                // Se DbInitializer � do seu projeto de Infra, e AppDbContext est� registrado corretamente, deve funcionar.
                await DbInitializer.InitializeAsync(services);
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error"); // Ou sua p�gina de erro customizada
            app.UseHsts();
        } else
        {
            app.UseDeveloperExceptionPage(); // Mant�m a p�gina de erro detalhada em dev
        }

        // Middleware de tratamento de exce��es customizado (para 401, 404, etc.)
        app.UseMiddleware<ExceptionMiddleware>(); // Certifique-se que este middleware est� correto

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Essencial: Autentica��o DEPOIS de Routing e ANTES de Authorization e Endpoints
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=GalLabs}/{action=GlDashboard}/{id?}"); // Sua rota padr�o

        app.Run();
    }
}