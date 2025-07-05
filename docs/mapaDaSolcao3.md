### Features da Solução GeneralLabSolutions - Parte 3

> A maioria das features abaixo tem a ver com Cliente, direta ou indiretamente:


- `IUnitOfWork`:
- `IGenericRepository`:
- `GenericRepository` (considerar retirar `new()`; refatorar muitas classes para ter construtor p/EF Core Protected):
- `Pessoa`:
- `Cliente`:
- `ClienteViewModel`:
- `ClienteMap`:
- `ClienteValidation`:
- `IClienteRepository` (refatorar):
- `ClienteRepository` (refatorar):
- `IClienteDomainService`:
- `ClienteDomainService`:

---


- `IUnitOfWork`:

```csharp

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}


```
---


- `IGenericRepository`:

```csharp

using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Interfaces;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IGenericRepository<T, TKey> : IDisposable where T : class
    {

        IUnitOfWork UnitOfWork { get; }

        #region: Métodos de Consulta
        Task<T> GetByIdAsync(TKey id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region: Métodos de Ação

        Task AddAsync(T obj);
        Task DeleteAsync(T obj);
        Task UpdateAsync(T obj);

        #endregion

    }
}


```
---


- `GenericRepository` (considerar retirar `new()`; refatorar muitas classes para ter construtor p/EF Core Protected):

```csharp

using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository.Base
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class, new()
    {
        protected readonly AppDbContext _context;

        /// <summary>
        /// Controlador, onde injeto o contexto do banco de dados.
        /// </summary>
        /// <param name="context"></param>
		public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;


        #region: Métodos de Consulta

        public async Task<T?> GetByIdAsync(TKey id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                return await _context.Set<T>().AsNoTracking().ToListAsync();
            }
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().AnyAsync(predicate);
        }

        #endregion

        #region: Métodos de Ação

        public async Task AddAsync(T obj)
        {
            _context.Set<T>().Add(obj);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T obj)
        {
            _context.Set<T>().Remove(obj);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(T obj)
        {
            _context.Set<T>().Update(obj);
            await Task.CompletedTask;
        }

        #endregion


        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}


```
---


- `Pessoa`:

```csharp

using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Pessoa : EntityBase
    {
        /// <summary>
        /// Construtor vazio para uso pelo EF
        /// </summary>
        public Pessoa() { }


        // Já refatorado para Pessoa 1:N Telefone
        public virtual ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();

        // Já refatorado para Pessoa 1:N Contatos
        public virtual ICollection<Contato> Contatos { get; set; } = new List<Contato>();

        // Já refatorado para Pessoa 1:N DadosBancarios
        public virtual ICollection<DadosBancarios> DadosBancarios { get; set; } = new List<DadosBancarios>();

        // Já refatorado para Pessoa 1:N Enderecos
        public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

    }
}

```
---


- `Cliente`:

```csharp

using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Services.Helpers;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Cliente : EntityAudit, IAggregateRoot, IPessoaContainer
    {
        // EF
        public Cliente() { }

        #region: Contrutor Parametrizado

        /// <summary>
        /// Construtor para criar um cliente com os dados necessários.
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="documento"></param>
        /// <param name="tipoDePessoa"></param>
        /// <param name="email"></param>
        public Cliente(string nome, string documento, TipoDePessoa tipoDePessoa, string email)
        {
            Pessoa = new Pessoa();
            PessoaId = Pessoa.Id;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
        }

        #endregion


        #region: Propriedades

        public Guid PessoaId { get; set; }

        public Pessoa Pessoa { get; set; }

        public string Nome { get; private set; }

        public string Documento { get; private set; }

        public string Email { get; private set; }

        public string? InscricaoEstadual { get; set; } = string.Empty;

        public string? Observacao { get; set; } = string.Empty;


        public string TelefonePrincipal { get; private set; }
        // CRM‑21 Nome contato, crm‑23 Telefone contato
        public string? ContatoRepresentante { get; private set; } = string.Empty;

        public void SetContatoRepresentante(string contatoRepresentante)
            => ContatoRepresentante = contatoRepresentante;

        public void SetTelefonePrincipal(string telefonePrincipal)
            => TelefonePrincipal = telefonePrincipal;




        #endregion


        #region: Enums e Collections

        public TipoDePessoa TipoDePessoa { get; private set; }

        // Status do cliente (e.g., Ativo, Inativo)
        public StatusDoCliente StatusDoCliente { get; set; } = StatusDoCliente.Ativo;
        // Tipo de cliente (e.g., Comum, Especial)
        public TipoDeCliente TipoDeCliente { get; set; }
            = TipoDeCliente.Comum;
        // Coleção de pedidos realizados pelo cliente
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        #endregion


        #region: Métodos Ad Hoc

        // Define o email do cliente
        public void SetEmail(string newEmail) => Email = newEmail;

        public void AddPedido(Pedido pedido)
            => Pedidos.Add(pedido);

        public void SetNome(string nome) => Nome = nome;

        // Define o tipo de pessoa
        public void SetTipoDePessoa(TipoDePessoa tipoDePessoa)
            => TipoDePessoa = tipoDePessoa;

        // Define o documento da pessoa
        public void SetDocumento(string documento)
            => Documento = documento;

        #endregion


        #region: Adição de dados bancários

        /// <summary>
        /// Adiciona uma nova conta bancária associada a este Cliente.
        /// </summary>
        public DadosBancarios AdicionarDadosBancarios(string banco,
                string agencia,
                string conta,
                TipoDeContaBancaria tipoConta)
        {
            return DadosBancariosDomainHelper.AdicionarDadosBancariosGenerico(this, banco, agencia, conta, tipoConta);
        }


        #endregion


        #region: Atualizações de dados bancários

        /// <summary>
        /// Atualiza os dados de uma conta bancária existente associada a este Cliente.
        /// </summary>
        public void AtualizarDadosBancarios(Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            DadosBancariosDomainHelper.AtualizarDadosBancariosGenerico(this, dadosBancariosId, banco, agencia, conta, tipoConta);
        }

        #endregion


        #region: Remoção de dados bancários

        /// <summary>
        /// Remove uma conta bancária associada a este Cliente.
        /// </summary>
        public DadosBancarios RemoverDadosBancarios(Guid dadosBancariosId)
        {
            return DadosBancariosDomainHelper.RemoverDadosBancariosGenerico(this, dadosBancariosId);
        }


        #endregion


        #region: Gerenciamento de Telefones

        /// <summary>
        /// Adiciona um novo telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone criada.</returns>
        public Telefone AdicionarTelefone(string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            return TelefoneDomainHelper.AdicionarTelefoneGenerico(this, ddd, numero, tipoTelefone);
        }

        /// <summary>
        /// Atualiza os dados de um telefone existente associado a este Cliente.
        /// </summary>
        public void AtualizarTelefone(Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            TelefoneDomainHelper.AtualizarTelefoneGenerico(this, telefoneId, ddd, numero, tipoTelefone);
        }

        /// <summary>
        /// Remove um telefone associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Telefone removida.</returns>
        public Telefone RemoverTelefone(Guid telefoneId)
        {
            return TelefoneDomainHelper.RemoverTelefoneGenerico(this, telefoneId);
        }

        #endregion


        #region: Gerenciamento de Contatos

        /// <summary>
        /// Adiciona um novo contato associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Contato criada.</returns>
        public Contato AdicionarContato(
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            return ContatoDomainHelper.AdicionarContatoGenerico(
                this, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
        }

        public void AtualizarContato(
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            ContatoDomainHelper.AtualizarContatoGenerico(
                this, contatoId, nome, email, telefone, tipoDeContato, emailAlternativo, telefoneAlternativo, observacao);
        }

        public Contato RemoverContato(Guid contatoId)
        {
            return ContatoDomainHelper.RemoverContatoGenerico(this, contatoId);
        }

        #endregion


        #region Gerenciamento de Endereços

        /// <summary>
        /// Adiciona um novo endereço associado a este Cliente.
        /// </summary>
        /// <returns>A entidade Endereco criada.</returns>
        public Endereco AdicionarEndereco(
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            return EnderecoDomainHelper.AdicionarEnderecoGenerico(
                this, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }

        public void AtualizarEndereco(
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            EnderecoDomainHelper.AtualizarEnderecoGenerico(
                this, enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
        }

        public Endereco RemoverEndereco(Guid enderecoId)
        {
            return EnderecoDomainHelper.RemoverEnderecoGenerico(this, enderecoId);
        }

        #endregion


    }
}

```
---


- `ClienteViewModel`:

```csharp

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class ClienteViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(200, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(254, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(14, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 11)]
        public string Documento { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Status do Cliente")]
        public StatusDoCliente StatusDoCliente { get; set; } = StatusDoCliente.Ativo;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Cliente")]
        public TipoDeCliente TipoDeCliente { get; set; } = TipoDeCliente.Comum;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Pessoa")]
        public TipoDePessoa TipoDePessoa { get; set; }


        public Guid PessoaId { get; set; }


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Telefone Principal")]
        public string TelefonePrincipal { get; set; }

        [Display(Name = "Contato principal ou Representante")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? ContatoRepresentante { get; set; } // Mantido como nullable string


        [Display(Name = "Inscrição Estadual")]
        [StringLength(80, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? InscricaoEstadual { get; set; } = string.Empty;

        [Display(Name = "Observação")]
        [StringLength(4000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Observacao { get; set; }



        // Já refatorado e pronto
        public List<DadosBancariosViewModel> DadosBancarios { get; set; } = new List<DadosBancariosViewModel>();

        // Coleção para Telefones (confirmando o tipo)
        // Usar List<> é geralmente mais prático que ICollection<> em ViewModels
        public List<TelefoneViewModel> Telefones { get; set; } = new List<TelefoneViewModel>();

        public List<ContatoViewModel> Contatos { get; set; } = new List<ContatoViewModel>();

        public List<EnderecoViewModel> Enderecos { get; set; } = new List<EnderecoViewModel>();


        #region: Campos auditáveis

        // ToDo: Talvez seja desnecessário, pois estes dados de Audit geralmente são gerenciados pelo EF Core

        

        [DisplayName(displayName: "Data de Cadastro")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName(displayName: "Usuário de Cadastro")]
        public string? UsuarioInclusao { get; set; }

        [DisplayName(displayName: "Data de Atualização")]
        public DateTime? DataUltimaModificacao { get; set; }

        [DisplayName(displayName: "Usuário de Atualização")]
        public string? UsuarioUltimaModificacao { get; set; }


        //[DisplayName("Ativo")]
        //public bool Ativo { get; set; }

        //[DisplayName("Excluído")]
        //public bool Excluido { get; set; }

        #endregion: Campos auditáveis


    }
}

```
---


- `ClienteMap`:


```csharp

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasIndex(x => x.Nome).HasDatabaseName("IX_Cliente_Nome");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Cliente_Email");
            builder.HasIndex(x => x.Documento).IsUnique().HasDatabaseName("IX_Cliente_Documento");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Documento)
                .IsRequired()
                .HasColumnType("varchar(14)"); // CPF ou CNPJ

            builder.Property(x => x.TipoDePessoa)
                .HasEnumConversion()
                .IsRequired();


            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(254)");

            #region: Auditoria

            builder.Property(x => x.DataInclusao)
                .HasColumnName("DataInclusao")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.UsuarioInclusao)
                .HasColumnType("varchar(120)")
                .IsRequired();


            builder.Property(x => x.DataUltimaModificacao)
                .HasColumnName("DataUltimaModificacao")
                .HasColumnType("datetime")
                .IsRequired();


            builder.Property(x => x.UsuarioUltimaModificacao)
                .HasColumnType("varchar(120)")
                .IsRequired();

            #endregion


            builder.Property(x => x.InscricaoEstadual)
                .HasColumnType("varchar(20)")
                .IsRequired(false);

            builder.Property(x => x.Observacao)
                .HasColumnType("varchar(4000)")
                .IsRequired(false);


            builder.Property(x => x.StatusDoCliente)
                .HasEnumConversion()
                .IsRequired();


            builder.Property(x => x.TipoDeCliente)
                .HasEnumConversion()
                .IsRequired();

            builder.Property(x => x.TelefonePrincipal)
                .HasColumnType("varchar(15)")
                .IsRequired();


            builder.Property(x => x.ContatoRepresentante)
                   .HasColumnType("varchar(50)")
                   .IsRequired(false); // Explicitamente definido como não obrigatório no banco



            builder.HasMany(x => x.Pedidos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Pessoa) // PONTO DE ATENÇÃO!
                   .WithOne() //  Cliente  tem  uma  Pessoa,  mas  Pessoa  não  tem  Cliente  diretamente
                   .HasForeignKey<Cliente>(c => c.PessoaId);

            builder.ToTable("Cliente");
        }
    }
}


```
---


- `ClienteValidation`:

```csharp

using FluentValidation;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base; // Para CpfValidacao, CnpjValidacao
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Extensions.Extending; // Para .ApenasNumeros()
using System.Text.RegularExpressions;

namespace GeneralLabSolutions.Domain.Validations;

public class ClienteValidation : AbstractValidator<Cliente>
{
    private const int NomeMin = 2;
    private const int NomeMax = 200;
    private const int EmailMin = 6;
    private const int EmailMax = 254;
    private static readonly Regex ApenasNumerosRegex = new(@"^\d+$", RegexOptions.Compiled); // Renomeei para evitar conflito com o método
    // private static readonly Regex MascaraTelefone = new(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
    //                                                 RegexOptions.Compiled);

    public ClienteValidation()
    {
        CascadeMode = CascadeMode.Stop;   // aborta na 1ª falha

        // ───── Nome ────────────────────────────────────────────────
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Length(NomeMin, NomeMax);

        // ───── Email ───────────────────────────────────────────────
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Length(EmailMin, EmailMax)
            .EmailAddress().WithMessage("O {PropertyName} informado é inválido.");

        // ───── Documento: CPF / CNPJ ───────────────────────────────
        RuleFor(c => c.Documento)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.");

        When(c => c.TipoDePessoa == TipoDePessoa.Fisica, () =>
        {
            RuleFor(c => c.Documento.ApenasNumeros().Length) // MODIFICADO
                .Equal(CpfValidacao.TamanhoCpf)
                .WithMessage($"O CPF deve conter {CpfValidacao.TamanhoCpf} dígitos.");

            RuleFor(c => CpfValidacao.Validar(c.Documento)) // CpfValidacao.Validar internamente usa .ApenasNumeros()
                .Equal(true).WithMessage("O CPF fornecido é inválido.");
        });

        When(c => c.TipoDePessoa == TipoDePessoa.Juridica, () =>
        {
            RuleFor(c => c.Documento.ApenasNumeros().Length) // MODIFICADO
                .Equal(CnpjValidacao.TamanhoCnpj)
                .WithMessage($"O CNPJ deve conter {CnpjValidacao.TamanhoCnpj} dígitos.");

            RuleFor(c => CnpjValidacao.Validar(c.Documento)) // CnpjValidacao.Validar internamente usa .ApenasNumeros()
                .Equal(true).WithMessage("O CNPJ fornecido é inválido.");
        });

        // ───── Telefone Principal ─────────────────────────────────
        RuleFor(c => c.TelefonePrincipal)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Must(t => ApenasNumerosRegex.IsMatch(t.ApenasNumeros()) && // MODIFICADO (e usei ApenasNumerosRegex)
                       t.ApenasNumeros().Length is >= 10 and <= 11)     // MODIFICADO
            .WithMessage("O {PropertyName} deve ter 10 ou 11 dígitos (DDD + número).");

        // ou, se preferir aceitar máscara: .Matches(MascaraTelefone)…

        // ───── Campos opcionais com tamanho máximo ────────────────
        RuleFor(c => c.ContatoRepresentante).MaximumLength(50);
        RuleFor(c => c.InscricaoEstadual).MaximumLength(80);
        RuleFor(c => c.Observacao).MaximumLength(4000);

        // ───── Enums obrigatórios ─────────────────────────────────
        RuleFor(c => c.TipoDePessoa).IsInEnum();
        RuleFor(c => c.StatusDoCliente).IsInEnum();
        RuleFor(c => c.TipoDeCliente).IsInEnum();

        // Inclua se realmente existir na entidade Cliente
        // RuleFor(c => c.RegimeTributario).IsInEnum();

        // ───── RuleSets (Create x Update) ─────────────────────────
        RuleSet("create", () =>
        {
            RuleFor(c => c.StatusDoCliente)
                .NotEqual(StatusDoCliente.Inativo)
                .WithMessage("Cliente não pode ser criado como Inativo.");

            RuleFor(c => c.TipoDeCliente)
                .NotEqual(TipoDeCliente.Inadimplente)
                .WithMessage("Cliente não pode ser criado como Inadimplente.");
        });

        RuleSet("update", () =>
        {
            // nada específico por enquanto; mas serve para flexibilizar depois
        });
    }
}

```
---


- `IClienteRepository` (refatorar):

```csharp

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.Interfaces
{
    public interface IClienteRepository : IGenericRepository<Cliente, Guid>
    {
        Task<bool> TemCliente(Guid id);
        Task<PagedResult<Cliente>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null);
        Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate);
        Task<Cliente?> ObterClienteCompleto(Guid clienteId);


        #region: NOVAS ASSINATURAS PARA DADOS BANCÁRIOS 
        Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId);
        Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId);
        Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId);

        Task AdicionarDadosBancariosAsync(Cliente cliente, DadosBancarios novo);
        Task RemoverDadosBancariosAsync(Cliente cliente, DadosBancarios dadosBancarios);

        #endregion


        #region: NOVAS ASSINATURAS PARA TELEFONE 
        
        Task<Cliente?> ObterClienteComTelefones(Guid clienteId);
        Task<List<Telefone>> ObterTelefonesPorClienteId(Guid pessoaId);
        Task<Telefone?> ObterTelefonePorId(Guid telefoneId); // se necessário fora do agregado

        Task AdicionarTelefoneAsync(Cliente cliente, Telefone novo);
        Task RemoverTelefoneAsync(Cliente cliente, Telefone telefone);

        #endregion


        #region: NOVAS ASSINATURAS PARA CONTATO

        Task<Cliente?> ObterClienteComContatos(Guid clienteId);
        Task<List<Contato>> ObterContatosPorClienteId(Guid pessoaId); // se precisar buscar fora do agregado
        Task<Contato?> ObterContatoPorId(Guid contatoId); // se precisar buscar fora do agregado

        Task AdicionarContatoAsync(Cliente cliente, Contato novo);
        Task RemoverContatoAsync(Cliente cliente, Contato contato);
        
        #endregion


        #region Assinaturas Endereço
        Task<Cliente?> ObterClienteComEnderecos(Guid clienteId);
        Task<List<Endereco>> ObterEnderecosPorClienteId(Guid pessoaId);
        Task<Endereco?> ObterEnderecoPorId(Guid enderecoId);

        Task AdicionarEnderecoAsync(Cliente cliente, Endereco novo);
        Task RemoverEnderecoAsync(Cliente cliente, Endereco endereco);

        #endregion

    }
}

```
---


- `ClienteRepository` (refatorar):

```csharp

using System.Linq.Expressions;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Extensions.Helpers.Generics;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.InfraStructure.Data;
using GeneralLabSolutions.InfraStructure.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Repository
{
    public class ClienteRepository : GenericRepository<Cliente, Guid>, IClienteRepository
    {
        // Propriedade pública para acesso ao Context (para debug)
        public AppDbContext Context => _context;

        public ClienteRepository(AppDbContext context) : base(context) { }


        #region: Métodos para busca em Cliente
        public async Task<bool> TemCliente(Guid id)
        {
            return await _context.Cliente.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Cliente>> Buscar(Expression<Func<Cliente, bool>> predicate)
        {
            return await _context.Cliente.AsNoTracking().Where(predicate).ToListAsync();
        }

        #endregion: Fim Métodos para busca em Cliente


        #region: ObterTodosPaginado

        public async Task<PagedResult<Cliente>> ObterTodosPaginado(int pageIndex, int pageSize, string? query = null)
        {
            IEnumerable<Cliente> data = new List<Cliente>();
            var source = _context.Cliente.AsQueryable();

            data = query != null
                ? await source.Where(x => x.Nome.Contains(query)).OrderBy(x => x.Nome).ToListAsync()
                : await source.OrderBy(x => x.Nome).ToListAsync();

            var count = data.Count();
            data = data.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PagedResult<Cliente>()
            {
                List = data,
                TotalPages = totalPages,
                TotalResults = count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Query = query,
                HasPrevious = pageIndex > 1,
                HasNext = pageIndex < totalPages
            };

        }

        #endregion: Fim ObterTodosPaginado


        #region: Métodos para Cliente Completo

        /// <summary>
        /// Obtém o cliente completo com todas as suas coleções de Pessoa.
        /// </summary>
        public async Task<Cliente?> ObterClienteCompleto(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.DadosBancarios)
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Telefones)
                                 .Include(c => c.Pessoa) // Inclui Pessoa novamente para encadear
                                     .ThenInclude(p => p.Contatos)
                                 .Include(c => c.Pessoa) // Inclui Pessoa novamente para encadear
                                     .ThenInclude(p => p.Enderecos)
                                 .Include(c => c.Pedidos) // Se necessário
                                     .ThenInclude(p => p.Itens) // Se necessário
                                 .AsSplitQuery()
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        #endregion: Fim Métodos Cliente Completo


        #region: Métodos para Dados Bancários (Mantidos)

        // ToDo: Mudar ObterDadosBancariosPorClienteId para ObterDadosBancariosPorPessoaId.
        public async Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId)
        {
            return await _context.DadosBancarios
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.DadosBancarios)
                                     .AsSplitQuery()
                                     .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId)
        {
            // Sem AsNoTracking aqui, pois pode ser para edição
            return await _context.DadosBancarios
                .FirstOrDefaultAsync(x => x.Id == dadosBancariosId);
        }


        public async Task AdicionarDadosBancariosAsync(Cliente cliente, DadosBancarios novo)
        {
            _context.Cliente.Attach(cliente); // Garante rastreamento do pai
            _context.Entry(novo).State = EntityState.Added; // Define estado explícito
            await Task.CompletedTask;
        }

        public async Task RemoverDadosBancariosAsync(Cliente cliente, DadosBancarios dadosBancarios)
        {
            _context.Cliente.Attach(cliente); // Garante rastreamento do pai
            _context.Entry(dadosBancarios).State = EntityState.Deleted; // Define estado explícito
            await Task.CompletedTask;
        }
        #endregion: Fim Métodos Dados Bancários


        #region: NOVAS IMPLEMENTAÇÕES PARA TELEFONE

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Telefones.
        /// </summary>
        public async Task<Cliente?> ObterClienteComTelefones(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Telefones) // Inclui a nova coleção
                                     .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }


        public async Task<List<Telefone>> ObterTelefonesPorClienteId(Guid pessoaId)
        {
            return await _context.Telefone
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Telefone?> ObterTelefonePorId(Guid telefoneId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Telefone
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == telefoneId);
        }


        /// <summary>
        /// Define explicitamente o estado de um novo Telefone como 'Added'.
        /// </summary>
        public async Task AdicionarTelefoneAsync(Cliente cliente, Telefone novo)
        {
            // Garantir que o Cliente (pai) esteja sendo rastreado
            _context.Cliente.Attach(cliente);
            // Definir o estado do novo telefone como Adicionado
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Telefone como 'Deleted'.
        /// </summary>
        public async Task RemoverTelefoneAsync(Cliente cliente, Telefone telefone)
        {
            // Garantir que o Cliente (pai) esteja sendo rastreado
            _context.Cliente.Attach(cliente);
            // Definir o estado do telefone como Excluído
            _context.Entry(telefone).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        #endregion: FIM NOVAS IMPLEMENTAÇÕES PARA TELEFONE


        #region Métodos Contatos

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Contatos.
        /// </summary>
        public async Task<Cliente?> ObterClienteComContatos(Guid clienteId)
        {
            return await _context.Cliente
                                 .Include(c => c.Pessoa)
                                     .ThenInclude(p => p.Contatos) // Inclui a nova coleção
                                     .AsSplitQuery()
                                 .FirstOrDefaultAsync(c => c.Id == clienteId);
        }

        public async Task<List<Contato>> ObterContatosPorClienteId(Guid pessoaId)
        {
            return await _context.Contato
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Contato?> ObterContatoPorId(Guid contatoId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Contato
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == contatoId);
        }

        /// <summary>
        /// Define explicitamente o estado de um novo Contato como 'Added'.
        /// </summary>
        public async Task AdicionarContatoAsync(Cliente cliente, Contato novo)
        {
            _context.Cliente.Attach(cliente); // Garante que o pai esteja rastreado
            _context.Entry(novo).State = EntityState.Added; // Define o estado como Adicionado
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Contato como 'Deleted'.
        /// </summary>
        public async Task RemoverContatoAsync(Cliente cliente, Contato contato)
        {
            _context.Cliente.Attach(cliente); // Garante que o pai esteja rastreado
            _context.Entry(contato).State = EntityState.Deleted; // Define o estado como Excluído
            await Task.CompletedTask;
        }
        #endregion


        #region Métodos Endereços

        public async Task<List<Endereco>> ObterEnderecosPorClienteId(Guid pessoaId)
        {
            return await _context.Endereco
                .Where(x => x.PessoaId == pessoaId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Endereco?> ObterEnderecoPorId(Guid enderecoId)
        {
            // Se necessário fora do agregado, mas geralmente não é recomendado
            return await _context.Endereco
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == enderecoId);
        }

        /// <summary>
        /// Obtém um cliente incluindo seus dados de Pessoa e a coleção de Endereços.
        /// </summary>
        public async Task<Cliente?> ObterClienteComEnderecos(Guid clienteId) // Ou ObterClienteCompleto
        {
            var clienteComEnderecos = await _context.Cliente
                                         .Include(c => c.Pessoa)
                                             .ThenInclude(p => p.Enderecos)
                                             .AsSplitQuery()
                                         .FirstOrDefaultAsync(c => c.Id == clienteId);
            // <<< COLOQUE O BREAKPOINT AQUI, APÓS A LINHA ACIMA ^^^ >>>
            return clienteComEnderecos;
        }

        /// <summary>
        /// Define explicitamente o estado de um novo Endereço como 'Added'.
        /// </summary>
        public async Task AdicionarEnderecoAsync(Cliente cliente, Endereco novo)
        {
            _context.Cliente.Attach(cliente);
            _context.Entry(novo).State = EntityState.Added;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Define explicitamente o estado de um Endereço como 'Deleted'.
        /// </summary>
        public async Task RemoverEnderecoAsync(Cliente cliente, Endereco endereco)
        {
            _context.Cliente.Attach(cliente);
            _context.Entry(endereco).State = EntityState.Deleted;
            await Task.CompletedTask;
        }

        #endregion


    }
}

```
---


- `IClienteDomainService`:

```csharp

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums; // Para TipoDeContaBancaria e TipoDeTelefone
using System;
using System.Threading.Tasks;

namespace GeneralLabSolutions.Domain.Services.Abstractions
{
    public interface IClienteDomainService
    {
        // --- Métodos para Cliente (Existentes) ---
        Task AddClienteAsync(Cliente model);
        Task UpdateClienteAsync(Cliente model);
        Task DeleteClienteAsync(Cliente model);

        #region: Métodos para Dados Bancários (Existentes)

        Task AdicionarDadosBancariosAsync(Guid clienteId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task AtualizarDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta);
        Task RemoverDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId);

        #endregion

        #region: NOVAS ASSINATURAS PARA TELEFONE

        Task AdicionarTelefoneAsync(Guid clienteId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task AtualizarTelefoneAsync(Guid clienteId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone);
        Task RemoverTelefoneAsync(Guid clienteId, Guid telefoneId);
        
        #endregion


        #region: NOVAS ASSINATURAS PARA CONTATO
        Task AdicionarContatoAsync(
            Guid clienteId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task AtualizarContatoAsync(
            Guid clienteId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "");

        Task RemoverContatoAsync(Guid clienteId, Guid contatoId);

        #endregion


        #region Assinaturas Endereço
        Task AdicionarEnderecoAsync(
            Guid clienteId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco, // Usa o enum interno de Endereco
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task AtualizarEnderecoAsync(
            Guid clienteId,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null);

        Task RemoverEnderecoAsync(Guid clienteId, Guid enderecoId);
        #endregion


    }
}

```
---


- `ClienteDomainService`:

```csharp

using System.Reflection;
using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Validations;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class ClienteDomainService : BaseService, IClienteDomainService
    {

        private readonly IClienteRepository _clienteRepository;
        private readonly IGenericRepository<Cliente, Guid> _query;
        private readonly IMediatorHandler _mediatorHandler;


        #region: Construtor

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="clienteRepository"></param>
        /// <param name="notificador"></param>
        /// <param name="query"></param>
        /// <param name="mediatorHandler"></param>
        public ClienteDomainService(IClienteRepository clienteRepository,
                                    INotificador notificador,
                                    IGenericRepository<Cliente, Guid> query,
                                    IMediatorHandler mediatorHandler)
            : base(notificador)
        {
            _clienteRepository = clienteRepository;
            _query = query;
            this._mediatorHandler = mediatorHandler;
        }

        #endregion


        #region: Adicionar Cliente

        public async Task AddClienteAsync(Cliente model)
        {
            // O método público agora chama seu próprio método de validação privado.
            if (!await ValidarSalvarCliente(model, isUpdate: false)) return;


            await _clienteRepository.AddAsync(model);

            model.AdicionarEvento(new ClienteRegistradoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));

            // PersistirDados // Já está sendo feito no AppDbContext

        }

        #endregion

        #region: Atualizar Cliente

        public async Task UpdateClienteAsync(Cliente model)
        {
            // O método público agora chama seu próprio método de validação privado.
            if (!await ValidarSalvarCliente(model, isUpdate: true)) return;

            //Não precisa mais disso, pois a entidade já está sendo rastreada
            //e as propriedades já foram atualizadas no controller.
            //await _clienteRepository.UpdateAsync(model);

            model.AdicionarEvento(new ClienteAtualizadoEvent(model.Id,
                                                             model.Nome,
                                                             model.Documento,
                                                             model.TipoDePessoa,
                                                             model.Email));

        }

        #endregion

        #region: Deletar Cliente

        public async Task DeleteClienteAsync(Cliente model)
        {
            // Verifica as regras de negócio e validações
            if (!await ValidarRemocaoCliente(model)) return;

            // 1. Adiciona o evento
            model.AdicionarEvento(new ClienteDeletadoEvent(model.Id, model.Nome));

            //// 2. Publica o evento *ANTES* de qualquer operação de persistência
            //await _mediatorHandler.PublicarEvento(new ClienteDeletadoEvent(model.Id, model.Nome));

            // Talvez seja melhor deletar do repositorio e não do genérico
            // e usar Detach, pois não posso instalar o EF aqui; (Exemplo abaixo)
            // // 3. Remove a entidade do DbContext *DIRETAMENTE*
            //_clienteRepository.UnitOfWork.Entry(model).State = EntityState.Deleted;

            await _clienteRepository.DeleteAsync(model); // Substituir esta linha

            // PersistirDados // Já está sendo feito no AppDbContext

        }

        #endregion

        #region: Add, Upd and Delete Dados Bancários

        /// <summary>
        /// Adiciona os dados bancários de um cliente.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <param name="banco"></param>
        /// <param name="agencia"></param>
        /// <param name="conta"></param>
        /// <param name="tipoConta"></param>
        /// <returns></returns>
        public async Task AdicionarDadosBancariosAsync(
    Guid clienteId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2) Apenas altera o domínio
                var novo = cliente.AdicionarDadosBancarios(banco, agencia, conta, tipoConta);

                // 3) Delega à infraestrutura a persistência do filho
                await _clienteRepository.AdicionarDadosBancariosAsync(cliente, novo);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception)
            {
                Notificar("Ocorreu um erro inesperado ao adicionar os dados bancários.");
            }
        }


        public async Task AtualizarDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente == null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                cliente.AtualizarDadosBancarios(dadosBancariosId, banco, agencia, conta, tipoConta);

                // await _clienteRepository.UpdateAsync(cliente); // Provavelmente não necessário se o EF rastreia a entidade filha modificada
                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar o erro completo (ex)
                Notificar("Ocorreu um erro inesperado ao atualizar os dados bancários.");
            }
        }

        public async Task RemoverDadosBancariosAsync(Guid clienteId, Guid dadosBancariosId)
        {
            // 1) Carrega o agregado com Pessoa + DadosBancarios
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2) Remove apenas no modelo de domínio
                var removido = cliente.RemoverDadosBancarios(dadosBancariosId);

                // 3) Delega à infraestrutura a marcação como Deleted
                await _clienteRepository.RemoverDadosBancariosAsync(cliente, removido);
                // ‑‑ CommitAsync continua sendo chamado pelo Controller ‑‑
            } catch (InvalidOperationException ex)   // Erros lançados pelo domínio
            {
                Notificar(ex.Message);
            } catch (Exception)                      // Qualquer outra exceção inesperada
            {
                Notificar("Ocorreu um erro inesperado ao remover os dados bancários.");
            }
        }


        #endregion

        #region: Métodos Telefone

        /// <summary>
        /// Adiciona um telefone para o cliente especificado.
        /// </summary>
        public async Task AdicionarTelefoneAsync(Guid clienteId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o telefone (e o evento)
                var novoTelefone = cliente.AdicionarTelefone(ddd, numero, tipoTelefone);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _clienteRepository.AdicionarTelefoneAsync(cliente, novoTelefone);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o telefone.");
            }
        }

        /// <summary>
        /// Atualiza um telefone existente do cliente especificado.
        /// </summary>
        public async Task AtualizarTelefoneAsync(Guid clienteId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o telefone (e adicionar evento)
                // A atualização ocorre na entidade Telefone rastreada dentro da coleção de Cliente.
                cliente.AtualizarTelefone(telefoneId, ddd, numero, tipoTelefone);

                // 3. NÃO é necessário chamar _clienteRepository.UpdateAsync(cliente) ou
                //    definir estado explícito para o telefone aqui, pois o EF Core
                //    detecta a modificação na entidade Telefone rastreada.
                //    (Diferente de Add/Remove onde o estado precisa ser explícito)

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o telefone.");
            }
        }

        /// <summary>
        /// Remove um telefone do cliente especificado.
        /// </summary>
        public async Task RemoverTelefoneAsync(Guid clienteId, Guid telefoneId)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Telefones
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var telefoneRemovido = cliente.RemoverTelefone(telefoneId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _clienteRepository.RemoverTelefoneAsync(cliente, telefoneRemovido);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio (ex: telefone não encontrado)
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o telefone.");
            }
        }

        #endregion


        #region Métodos Contatos

        /// <summary>
        /// Adiciona um contato para o cliente especificado.
        /// </summary>
        public async Task AdicionarContatoAsync(
            Guid clienteId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId); // Usar o método específico

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para adicionar o contato (e o evento)
                var novoContato = cliente.AdicionarContato(
                    nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. Chama o repositório para marcar explicitamente o estado como Added
                await _clienteRepository.AdicionarContatoAsync(cliente, novoContato);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o contato.");
            }
        }

        /// <summary>
        /// Atualiza um contato existente do cliente especificado.
        /// </summary>
        public async Task AtualizarContatoAsync(
            Guid clienteId,
            Guid contatoId,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para atualizar o contato (e adicionar evento)
                // A atualização ocorre na entidade Contato rastreada dentro da coleção de Cliente.
                cliente.AtualizarContato(
                    contatoId, nome, email, telefone, tipoDeContato,
                    emailAlternativo, telefoneAlternativo, observacao);

                // 3. NÃO é necessário chamar _clienteRepository.UpdateAsync(cliente) ou
                //    definir estado explícito para o contato aqui, pois o EF Core
                //    detecta a modificação na entidade Contato rastreada.

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros de validação do domínio
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o contato.");
            }
        }

        /// <summary>
        /// Remove um contato do cliente especificado.
        /// </summary>
        public async Task RemoverContatoAsync(Guid clienteId, Guid contatoId)
        {
            // 1. Carrega o agregado Cliente, incluindo a coleção de Contatos
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                // 2. Chama o método de domínio para remover da coleção (e obter o objeto)
                var contatoRemovido = cliente.RemoverContato(contatoId);

                // 3. Chama o repositório para marcar explicitamente o estado como Deleted
                await _clienteRepository.RemoverContatoAsync(cliente, contatoRemovido);

                // O CommitAsync será chamado pela Controller
            } catch (InvalidOperationException ex) // Captura erros (ex: contato não encontrado)
            {
                Notificar(ex.Message);
            } catch (Exception ex) // Outras exceções
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o contato.");
            }
        }

        #endregion

        #region Métodos Endereços

        /// <summary>
        /// Adiciona um endereço para o cliente especificado.
        /// </summary>
        public async Task AdicionarEnderecoAsync(
            Guid clienteId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId); // Carrega com a coleção de endereços

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                var novoEndereco = cliente.AdicionarEndereco(
                    paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

                await _clienteRepository.AdicionarEnderecoAsync(cliente, novoEndereco);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao adicionar o endereço.");
            }
        }

        /// <summary>
        /// Atualiza um endereço existente do cliente especificado.
        /// </summary>
        public async Task AtualizarEnderecoAsync(
            Guid clienteId,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                cliente.AtualizarEndereco(
                    enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco,
                    linhaEndereco2, estadoOuProvincia, informacoesAdicionais);

                // Não é necessário chamar o repositório para marcar estado de atualização aqui,
                // pois o EF Core rastreia as mudanças na entidade Endereco carregada.
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao atualizar o endereço.");
            }
        }

        /// <summary>
        /// Remove um endereço do cliente especificado.
        /// </summary>
        public async Task RemoverEnderecoAsync(Guid clienteId, Guid enderecoId)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);

            if (cliente is null)
            {
                Notificar($"Cliente com ID {clienteId} não encontrado.");
                return;
            }

            try
            {
                var enderecoRemovido = cliente.RemoverEndereco(enderecoId);
                await _clienteRepository.RemoverEnderecoAsync(cliente, enderecoRemovido);
            } catch (InvalidOperationException ex)
            {
                Notificar(ex.Message);
            } catch (Exception ex)
            {
                // Logar erro ex
                Notificar("Ocorreu um erro inesperado ao remover o endereço.");
            }
        }

        #endregion




        #region: Métodos de Validação Privados e Otimizados

        /// <summary>
        /// Valida as regras de negócio para salvar (adicionar ou atualizar) um cliente.
        /// Otimizado para usar uma única consulta para checar duplicidade.
        /// </summary>
        private async Task<bool> ValidarSalvarCliente(Cliente model, bool isUpdate = false)
        {
            // 1. Validação de formato e regras básicas (FluentValidation)
            if (!ExecutarValidacao(new ClienteValidation(), model))
                return false;

            // 2. Validação de duplicidade (otimizada)
            // Busca por qualquer outro cliente que tenha o mesmo Documento OU o mesmo Email.
            var query = _query.SearchAsync(c => (c.Documento == model.Documento || c.Email == model.Email) && c.Id != model.Id);
            var clienteExistente = await query;

            if (clienteExistente.Any())
            {
                if (clienteExistente.Any(c => c.Documento == model.Documento))
                {
                    Notificar("Já existe um Cliente com o documento informado.");
                }
                if (clienteExistente.Any(c => c.Email == model.Email))
                {
                    Notificar("Já existe um Cliente com o E-mail informado.");
                }
                return false;
            }

            // 3. Regras específicas de Adição (Create)
            if (!isUpdate)
            {
                if (model.StatusDoCliente == StatusDoCliente.Inativo)
                {
                    Notificar("Nenhum cliente pode ser cadastrado com o status 'Inativo'.");
                }
                if (model.TipoDeCliente == TipoDeCliente.Inadimplente)
                {
                    Notificar("Nenhum cliente pode ser cadastrado como 'Inadimplente'.");
                }
            }

            // 4. Regras específicas de Edição (Update)
            if (isUpdate)
            {
                // Exemplo: se uma regra de update for necessária no futuro, ela entra aqui.
                // var clienteAtual = await _query.GetByIdAsync(model.Id); // Já temos a entidade 'model'
                if (model.TipoDeCliente == TipoDeCliente.Inadimplente)
                {
                    Notificar("A edição de clientes inadimplentes deve ser feita pela tela de negociação.");
                }
            }

            // Se chegou até aqui, todas as notificações foram tratadas
            return !TemNotificacao();
        }

        /// <summary>
        /// Valida as regras de negócio para remover um cliente.
        /// </summary>
        private async Task<bool> ValidarRemocaoCliente(Cliente model)
        {
            // Implementação do Soft-Delete (sugestão da conversa anterior):
            // Se a regra de negócio for não excluir, mas sim inativar, a lógica mudaria aqui.
            // Por enquanto, mantemos a regra original.

            var clienteComPedidos = await _clienteRepository.SearchAsync(c => c.Id == model.Id && c.Pedidos.Any());

            if (clienteComPedidos.Any())
            {
                Notificar("O cliente possui pedidos e não pode ser excluído. Considere inativá-lo.");
                return false;
            }

            return true;
        }

        #endregion

    }
}


```
---


- Já volto com mais códigos. Preciso que conheça bem minha Solution. Pelo menos uma boa parte dela, por enquanto. Me aguarde!
- Se quiser pode atualizar nosso `Painel de Bordo`. :)