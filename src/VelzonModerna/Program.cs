using GeneralLabSolutions.Domain.Services;
using GeneralLabSolutions.Domain.Settings;      // Para OpenAISettings
using GeneralLabSolutions.InfraStructure.Data.ORM;
using GeneralLabSolutions.InfraStructure.Data.Seeds;
using GeneralLabSolutions.InfraStructure.IoC;
using GeneralLabSolutions.WebApiCore.Extensions;  // Para AddIdentityConfiguration (IAspNetUser)
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Configuration.Extensions;
using VelzonModerna.Configuration.Mappings;
using VelzonModerna.Services;
// using VelzonModerna.Workers; // Comentado por enquanto;

public class Program
{
    public static async Task Main(string [] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;
        configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        if (builder.Environment.IsDevelopment())
        {
            configuration.AddUserSecrets<Program>();
        }

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddMvcConfiguration()
            .AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddServicesAndDepencencyInjections(configuration)
             .AddAutoMapper(typeof(AutoMapperConfig));

        builder.Services.AddSingleton<IValidationAttributeAdapterProvider, CpfValidationAttributeAdapterProvider>();

        // Pagina��o (se usado no MVC) - Se for usado pelo MVC, sen�o remover.
        builder.Services.AddScoped<IPaginationService, PaginationService>();

        // Configura��es de Autentica��o e Identity do ASP.NET Core (Cookies, etc.)
        builder.Services.AddIdentityConfiguration();

        builder.Services.Configure<AppSettingsMvc>(options =>
        {
            // ToDo: Verifique se as chaves est�o corretas no appsettings.json ou secrets.json
            // ToDo: Refatorar para evitar hardcoding de chaves
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


        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbInitializer.InitializeAsync(services);
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error"); // Ou p�gina de erro customizada
            app.UseHsts();
        } else
        {
            app.UseDeveloperExceptionPage();
        }

        // Middleware de tratamento de exce��es customizado (para 401, 404, etc.)
        app.UseMiddleware<ExceptionMiddleware>(); // Certificar se este middleware est� 100% funcional

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Essencial: Autentica��o DEPOIS de Routing e ANTES de Authorization e Endpoints
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=GalLabs}/{action=GlDashboard}/{id?}");

        app.Run();
    }
}