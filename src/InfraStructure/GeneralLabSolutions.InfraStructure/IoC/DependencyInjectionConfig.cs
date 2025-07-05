using GeneralLabSolutions.CoreShared.Interfaces;
using GeneralLabSolutions.Domain.Configurations;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Interfaces.CRM;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Services.Concreted;
using GeneralLabSolutions.InfraStructure.Repository;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using GeneralLabSolutions.InfraStructure.Repository.CRM;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.IoC
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddServicesAndDepencencyInjections(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // Registra o MediatR e escaneia assemblies para handlers
            services.AddMediatRExtencions();

            services.AddScoped<IAgenteDeIARepository, AgenteDeIARepository>();

            // DI Mensageria
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();


            // DI Generic Repositories
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // DI Others Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IPedidoRepositoryDomain, PedidoRepository>();
            services.AddScoped<IPedidoRepositoryDto, PedidoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IVendedorRepository, VendedorRepository>();



            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            services.AddScoped<IContaRepository, ContaRepository>();


            // DI KanbanTask
            services.AddScoped<IKanbanTaskRepository, KanbanTaskRepository>();
            services.AddScoped<IParticipanteRepository, ParticipanteRepository>();


            // DI Consolidados
            services.AddScoped<IConsolidadoClienteRepositoryDomain, ConsolidadoClienteRepository>();
            services.AddScoped<IConsolidadoVendedorRepositoryDomain, ConsolidadoVendedorRepository>();
            services.AddScoped<IConsolidadoFornecedorRepositoryDomain, ConsolidadoFornecedorRepository>();

            services.AddScoped<IConsolidadoClienteRepository, ConsolidadoClienteRepository>();
            services.AddScoped<IConsolidadoVendedorRepository, ConsolidadoVendedorRepository>();
            services.AddScoped<IConsolidadoFornecedorRepository, ConsolidadoFornecedorRepository>();


            // DI DomainService
            services.AddScoped<IClienteDomainService, ClienteDomainService>();
            services.AddScoped<ICategoriaDomainService, CategoriaDomainService>();
            services.AddScoped<IFornecedorDomainService, FornecedorDomainService>();
            services.AddScoped<IVendedorDomainService, VendedorDomainService>();

            services.AddScoped<IKanbanTaskDomainService, KanbanTaskDomainService>();
            services.AddScoped<IParticipanteDomainService, ParticipanteDomainService>();

            services.AddScoped<IContaService, ContaService>();

            // CRM DI
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<ICrmTaskRepository, CrmTaskRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();

            services.AddScoped<ILeadDomainService, LeadDomainService>();
            services.AddScoped<ICrmTaskDomainService, CrmTaskDomainService>();

            return services;

        }
    }
}
