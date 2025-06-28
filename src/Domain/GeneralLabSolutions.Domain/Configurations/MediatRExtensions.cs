using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.Domain.Configurations
{
    public static class MediatRExtensions
    {
        public static IServiceCollection AddMediatRExtencions(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            //// DI Eventos > Já configurado acima!!!
            //services.AddScoped<INotificationHandler<ClienteRegistradoEvent>, ClienteRegistradoEventHandler>();
            //services.AddScoped<INotificationHandler<ClienteDeletadoEvent>, ClienteDeletadoEventHandler>();

            return services;
        }
    }
}