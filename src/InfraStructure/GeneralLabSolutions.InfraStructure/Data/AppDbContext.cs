using FluentValidation.Results;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Data
{
    public class AppDbContext : DbContext, IUnitOfWork
    {

        private readonly IMediatorHandler? _mediatorHandler;

        private readonly IAspNetUser _user;


        /// <summary>
        /// Construtor padrão para o EntityFramework Core.
        /// Estou utilizando o padrão de repositório e unidade de trabalho.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mediatorHandler"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options,
                            IMediatorHandler? mediatorHandler = null,
                            IAspNetUser user = null)
                            : base(options)
        {
            _mediatorHandler = mediatorHandler;
            _user = user;
        }

        public DbSet<Produto> Produto { get; set; }
        public DbSet<CategoriaProduto> CategoriaProduto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<Vendedor> Vendedor { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<ItemPedido> ItemPedido { get; set; }
        public DbSet<Telefone> Telefone { get; set; }

        public DbSet<Endereco> Endereco { get; set; }

        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Contato> Contato { get; set; }
        public DbSet<Voucher> Voucher { get; set; }

        public DbSet<EstadoDoItem> EstadoDoItem { get; set; }

        public DbSet<StatusDoItem> StatusDoItem { get; set; }
        public DbSet<StatusDoItemIncompativel> StatusDoItemIncompativel { get; set; }
        public DbSet<HistoricoPedido> HistoricoPedido { get; set; }
        public DbSet<HistoricoItem> HistoricoItem { get; set; }

        // Novos modelos para QuadroKanban
        public DbSet<KanbanTask> KanbanTask { get; set; }
        public DbSet<Participante> Participante { get; set; }

        public DbSet<Conta> Conta { get; set; }

        public DbSet<CalendarEvent> CalendarEvents { get; set; }

        public DbSet<DadosBancarios> DadosBancarios { get; set; }

        public DbSet<MensagemChat> MensagensChat { get; set; }

        // Novos modelos para CRM

        public DbSet<Activity> Activities { get; set; }
        public DbSet<CrmTask> CrmTasks { get; set; }
        public DbSet<CrmTaskAttachment> CrmTaskAttachments { get; set; }
        public DbSet<CrmTaskComment> CrmTaskComments { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<LeadNote> LeadNotes { get; set; }
        public DbSet<Log> Logs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();


            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public async Task<bool> CommitAsync()
        {
            //var sucesso = await base.SaveChangesAsync() > 0;
            var sucesso = await SaveChangesAsync() > 0;
            if (sucesso)
                await _mediatorHandler.PublicarEventos(this);
            return sucesso;
        }



        // Em AppDbContext.cs

        #region: SaveChanges
        public override int SaveChanges()
        {
            try
            {
                EditableCall();
                return base.SaveChanges();
            } catch (Exception ex)
            {
                // Log ex aqui se precisar (usando ILogger)
                Console.WriteLine($"Erro original em SaveChanges: {ex}"); // Log simples para depuração
                throw; // <-- Re-lança a exceção original preservando o stack trace
            }
        }
        #endregion

        #region: SaveChangesAsync
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                EditableCall();
                return await base.SaveChangesAsync(true, cancellationToken); // Passar true para acceptAllChangesOnSuccess é o padrão
            } catch (Exception ex)
            {
                // Log ex aqui se precisar (usando ILogger)
                Console.WriteLine($"Erro original em SaveChangesAsync: {ex}"); // Log simples para depuração
                throw; // <-- Re-lança a exceção original preservando o stack trace
            }
        }
        #endregion

        #region: EditableCall
        // Relembrando a solução recomendada para EditableCall
        private void EditableCall()
        {
            var currentTime = DateTime.UtcNow; // <-- Usar UtcNow é uma boa prática para DB
            var usuario = _user?.EstaAutenticado() == true ? _user.ObterApelido() : "Sistema";

            if (string.IsNullOrWhiteSpace(usuario))
            {
                usuario = "Sistema";
            }

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IAuditable))
            {
                var auditableEntity = (IAuditable)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditableEntity.DataInclusao = currentTime;
                    auditableEntity.UsuarioInclusao = usuario;
                    // Definir também na criação para satisfazer NOT NULL
                    auditableEntity.DataUltimaModificacao = currentTime;
                    auditableEntity.UsuarioUltimaModificacao = usuario;
                } else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IAuditableAdd.DataInclusao)).IsModified = false;
                    entry.Property(nameof(IAuditableAdd.UsuarioInclusao)).IsModified = false;
                    auditableEntity.DataUltimaModificacao = currentTime;
                    auditableEntity.UsuarioUltimaModificacao = usuario;
                }
            }
        }
        #endregion




    }

    #region: Persistindo Eventos

    public static class MediatorExtension
    {
        public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            // Exemplo de código padrão para publicar Domain Events
            var domainEntities = ctx.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any())
                .ToList();

            // Para cada entidade com eventos de domínio, publicar
            foreach (var entityEntry in domainEntities)
            {
                var events = entityEntry.Entity?.Notificacoes?.ToArray();
                entityEntry.Entity?.LimparEventos(); // se quiser limpar depois
                foreach (var domainEvent in events!)
                {
                    await mediator.PublicarEvento(domainEvent);
                }
            }
        }
    }

    #endregion
}
