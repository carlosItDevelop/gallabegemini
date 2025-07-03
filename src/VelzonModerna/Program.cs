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

        // Configurações de Configuration
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
        builder.Services.AddMvcConfiguration(); // Seu método de extensão para MVC

         builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        // Injeções de dependência do seu projeto de Infraestrutura e Domínio
        builder.Services.AddServicesAndDepencencyInjections(configuration);

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

        // Validação de CPF
        builder.Services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();

        // Paginação (se usado no MVC)
        builder.Services.AddScoped<IPaginationService, PaginationService>(); // Se for usado pelo MVC, senão remover.

        // Configurações de Autenticação e Identity do ASP.NET Core (Cookies, etc.)
        builder.Services.AddIdentityConfiguration(); // Registra IAspNetUser e configuração de Cookie Auth.

        // Configuração do AppSettingsMvc
        builder.Services.Configure<AppSettingsMvc>(options =>
        {
            options.AutenticacaoUrl = configuration.GetValue<string>("AutenticacaoUrl");
            options.UserAdminUrl = configuration.GetValue<string>("UserAdminUrl");
        });

        // --- Configuração dos HttpClients para Serviços ---

        // Handler de Autorização (Adiciona token JWT às requisições)
        // Deve ser Transient porque ele mesmo resolve serviços Scoped (IAutenticacaoService)
        builder.Services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        // HttpClient para IAutenticacaoService (NÃO usa o HttpClientAuthorizationDelegatingHandler)
        // As chamadas de Login, Registro, RefreshToken não enviam o token JWT Bearer.
        builder.Services.AddHttpClient<IAutenticacaoService, AutenticacaoService>()
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 3))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpCircuitBreakerPolicy(eventsAllowedBeforeBreaking: 5, durationOfBreakInSeconds: 30))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 30)); // Timeout menor para auth?

        // HttpClient para IUserAdminMvcService (USA o HttpClientAuthorizationDelegatingHandler)
        builder.Services.AddHttpClient<IUserAdminMvcService, UserAdminMvcService>(client =>
        {
            // Obtém a URL do UserAdmin da configuração
            var userAdminApiUrl = configuration.GetValue<string>("UserAdminUrl");
            if (string.IsNullOrEmpty(userAdminApiUrl))
            {
                // Lançar uma exceção mais informativa ou logar um erro grave.
                // O serviço não funcionará corretamente sem esta URL.
                throw new InvalidOperationException("UserAdminUrl não está configurada corretamente no appsettings/secrets e é necessária para UserAdminMvcService.");
            }
            client.BaseAddress = new Uri(userAdminApiUrl.TrimEnd('/') + "/");
        })
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>() // Adiciona o token
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 3))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpCircuitBreakerPolicy(eventsAllowedBeforeBreaking: 5, durationOfBreakInSeconds: 30))
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 60));

        // HttpClient para IOpenAIService (Decida se precisa de token ou não)
        // Se não precisar de token JWT, não adicione o handler.
        builder.Services.AddHttpClient<IOpenAIService, OpenAIService>()
            .AddPolicyHandler(PollyHttpPolicies.GetHttpRetryPolicy(retryCount: 2)) // Exemplo de políticas
            .AddPolicyHandler(PollyHttpPolicies.GetHttpTimeoutPolicy(timeoutInSeconds: 120)); // OpenAI pode demorar
        builder.Services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));


        var app = builder.Build();

        // Chamada para o DbInitializer (se este for o projeto que inicializa o BD)
        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                // Se DbInitializer é do seu projeto de Infra, e AppDbContext está registrado corretamente, deve funcionar.
                await DbInitializer.InitializeAsync(services);
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error"); // Ou sua página de erro customizada
            app.UseHsts();
        } else
        {
            app.UseDeveloperExceptionPage(); // Mantém a página de erro detalhada em dev
        }

        // Middleware de tratamento de exceções customizado (para 401, 404, etc.)
        app.UseMiddleware<ExceptionMiddleware>(); // Certifique-se que este middleware está correto

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Essencial: Autenticação DEPOIS de Routing e ANTES de Authorization e Endpoints
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=GalLabs}/{action=GlDashboard}/{id?}"); // Sua rota padrão

        app.Run();
    }
}