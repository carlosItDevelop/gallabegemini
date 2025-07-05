### Features da Solução GeneralLabSolutions

- Vamos começar com algumas features em códigos da minha Solution, mas ainda não cheguei no problema:


- `Message`:

```csharp

namespace GeneralLabSolutions.Domain.Mensageria
{
    public abstract class Message
    {
        public string? MessageType { get; private set; }

        public Guid AggregateId { get; set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }

    }
}

```
---

- `Event`:

```csharp

using MediatR;

namespace GeneralLabSolutions.Domain.Mensageria
{
    public abstract class Event : Message, INotification
    {
        public DateTime TimesTamp { get; private set; }

        protected Event()
        {
            TimesTamp = DateTime.Now;
        }
    }
}


```
---

- `DomainEvent`:

```csharp

namespace GeneralLabSolutions.Domain.Mensageria
{
    public class DomainEvent : Event
    {
        public DomainEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}


```
---

- `IMediatorHandler`:


```csharp

using FluentValidation.Results;

namespace GeneralLabSolutions.Domain.Mensageria
{
    public interface IMediatorHandler
    {
        Task PublicarEvento<T>(T evento) where T : Event;
    }
}

```
---

- `MediatorHandler`:


```csharp

using FluentValidation.Results;
using MediatR;

namespace GeneralLabSolutions.Domain.Mensageria
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task PublicarEvento<T>(T evento) where T : Event
        {
            await _mediator.Publish(evento);
        }
    }
}

```
---


- `PagedResult`:

```csharp

namespace GeneralLabSolutions.Domain.Extensions.Helpers.Generics
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T>? List { get; set; }
        public int TotalResults { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Query { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}


```
---


- `StringUtils`:

```csharp

namespace GeneralLabSolutions.Domain.Extensions.Extending
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string input) // 'input' agora é a string estendida
        {
            if (string.IsNullOrEmpty(input)) // Boa prática: tratar nulo/vazio
            {
                return string.Empty;
            }
            return new string(input.Where(char.IsDigit).ToArray());
        }
    }

}


```
---


- `INotificador`:

```csharp

namespace GeneralLabSolutions.Domain.Notifications
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}


```
---

- `Notificador`:

```csharp

namespace GeneralLabSolutions.Domain.Notifications
{
    public class Notificador : INotificador
    {
        private List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}


```
---


- `Notificacao`:

```csharp

namespace GeneralLabSolutions.Domain.Notifications
{
    public class Notificacao
    {
        public Notificacao(string mensagem)
        {
            Mensagem = mensagem;
        }
        public string Mensagem { get; }
    }
}

```
---


- `BaseService`:


```csharp

using FluentValidation;
using FluentValidation.Results;
using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Notifications
{
    public abstract class BaseService
    {
        private readonly INotificador _notificador;
        
        protected BaseService(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected void Notificar(ValidationResult validationResult) 
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }


        // NOVO MÉTODO PROTEGIDO ADICIONADO AQUI
        protected bool TemNotificacao()
        {
            return _notificador.TemNotificacao();
        }


        protected bool ExecutarValidacao<TVal, T>(TVal validacao, T entidade) where TVal : AbstractValidator<T> where T : EntityBase
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid)
                return true;

            Notificar(validator);
                return false;
        }


    }
}

```
---


- `MeusEnums.cs` (arquivo '.cs' com vários enums da aplicação):

```csharp

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace GeneralLabSolutions.Domain.Enums
{

    public enum TipoDeConta
    {
        [Description("Conta a Receber")] Receber = 1,
        [Description("Conta a Pagar")] Pagar = 2
    }

    public enum Sexo
    {
        [Description("Feminino")] Feminino, // Peso: 48
        [Description("Masculino")] Masculino, // Peso: 48
        [Description("Outro")] Outro // Peso: 4

    }
    public enum StatusDoCliente
    {
        [Description("Ativo")] Ativo, // Peso: 30
        [Description("Inativo")] Inativo, // Peso 8
        [Description("Bloqueado")] Bloqueado // Peso 3  // // usado pelo FIN‑16 (bloqueio de faturamento)
    }

    public enum RegimeTributario   // CRM‑14 campos fiscais
    {
        [Description("Simples Nacional")] SimplesNacional = 1, // Peso: 8
        [Description("Lucro Presumido")] LucroPresumido = 2,  // Peso: 12
        [Description("Lucro Real")] LucroReal = 3,       // Peso: 20
        [Description("Outro")] Outro = 99           // Peso: 2
    }

    public enum StatusDoFornecedor
    {
        Ativo, // Peso: 30
        Inativo // Peso: 5
    }


    public enum StatusDoProduto
    {
        [Description("Dropshipping")] Dropshipping, // Peso: 30 => Trocado por Dropshipping
        [Description("Inativo")] Inativo, // Peso: 5
        [Description("Reservado")] Reservado, // 20
        [Description("Em Estoque")] EmEstoque, // 10
        [Description("Esgotado")] Esgotado // Todo: Peso 5
    }
    public enum StatusDoVendedor
    {
        Admin, // Peso: 15
        Contratado, // Peso: 10
        FreeLance, // Peso: 20
        Inativo // Peso: 5
    }
    public enum TipoDeCliente
    {
        [Description("Especial")] Especial, // Peso 9
        [Description("Comum")] Comum, // Peso 32
        [Description("Inadimplente")] Inadimplente // Peso 4
    }
    public enum TipoDeContato
    {
        Inativo, // Peso: 5
        Comercial, // Peso: 35
        Pessoal, // Peso: 10
        ProspeccaoCliente, // Peso: 25
        ProspeccaoVendedor, // Peso: 10
        ProspeccaoFornecedor // Peso: 15
    }
    public enum TipoDePessoa
    {
        [Description("Física")] Fisica, // Peso: 5
        [Description("Jurídica")] Juridica // Peso: 25
    }
    public enum TipoDeTelefone
    {
        Celular, // Peso: 10
        Residencial, // Peso: 5
        Comercial, // Peso: 15
        Recado, // Peso: 5
        Outro // Peso: 3
    }

    // TipoDescontoVoucher
    public enum TipoDescontoVoucher
    {
        Porcentagem = 0,
        Valor = 1
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Column
    {
        [EnumMember(Value = "todo")]
        todo,

        [EnumMember(Value = "review")]
        review,

        [EnumMember(Value = "progress")]
        progress,

        [EnumMember(Value = "done")]
        done
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Priority
    {
        [EnumMember(Value = "alta")]
        alta,

        [EnumMember(Value = "media")]
        media,

        [EnumMember(Value = "baixa")]
        baixa
    }


    public enum TipoEvento
    {
        [Display(Name = "bg-danger-subtle")] Perigo,
        [Display(Name = "bg-success-subtle")] Sucesso,
        [Display(Name = "bg-primary-subtle")] Primario,
        [Display(Name = "bg-info-subtle")] Info,
        [Display(Name = "bg-dark-subtle")] Escuro,
        [Display(Name = "bg-warning-subtle")] Alerta
    }

    // Enums para CRM Leads


    public enum LeadStatus
    {
        [EnumMember(Value = "novo")]
        Novo,
        [EnumMember(Value = "contato")]
        Contato,
        [EnumMember(Value = "qualificado")]
        Qualificado,
        [EnumMember(Value = "proposta")]
        Proposta,
        [EnumMember(Value = "negociacao")]
        Negociacao,
        [EnumMember(Value = "ganho")]
        Ganho,
        [EnumMember(Value = "perdido")]
        Perdido
    }

    public enum LeadTemperature
    {
        [EnumMember(Value = "quente")]
        Quente,
        [EnumMember(Value = "morno")]
        Morno,
        [EnumMember(Value = "frio")]
        Frio
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CrmTaskStatus
    {
        [EnumMember(Value = "pending")]
        Pendente,

        [EnumMember(Value = "completed")]
        Concluida,

        [EnumMember(Value = "cancelled")]
        Cancelada
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {
        [EnumMember(Value = "high")]
        Alta,

        [EnumMember(Value = "medium")]
        Media,

        [EnumMember(Value = "low")]
        Baixa
    }

    public enum TipoDeContaBancaria
    {
        Corrente = 1,
        Poupanca = 2,
        ContaSalario = 3,
        ContaConjunta = 4,
        ContaUniversitaria = 5,
        ContaEmpresarial = 6,
        Pagamento = 7 // contas digitais
    }
}


```
---


- `Result`:

```csharp

namespace GeneralLabSolutions.Domain.DomainObjects
{
    public class Result
    {
        public Result()
        {
            Errors = new List<string>();
            Data = new object();
        }

        public object Data { get; set; }
        public List<string> Errors { get; set; }
    }


    /*
     
     ToDo: Sugestão de refatoração;


        public class Result
        {
            public bool IsSuccess { get; }
            public bool IsFailure => !IsSuccess;
            public List<string> Errors { get; }
            protected Result(bool isSuccess, List<string> errors)
            {
                IsSuccess = isSuccess;
                Errors = errors ?? new List<string>();
            }

            public static Result Ok() => new Result(true, null);
            public static Result Fail(string error) => new Result(false, new List<string> { error });
            public static Result Fail(List<string> errors) => new Result(false, errors);
        }

        public class Result<T> : Result
        {
            public T Data { get; }

            protected Result(T data, bool isSuccess, List<string> errors) : base(isSuccess, errors)
            {
                Data = data;
            }

            public static Result<T> Ok(T data) => new Result<T>(data, true, null);
            public new static Result<T> Fail(string error) => new Result<T>(default(T), false, new List<string> { error });
            public new static Result<T> Fail(List<string> errors) => new Result<T>(default(T), false, errors);
        }
     
     */

}


```
---


- `IAggregateRoot`:

```csharp

namespace GeneralLabSolutions.Domain.DomainObjects
{
    public interface IAggregateRoot { }
}


```
---


- `GeneralLabSolutions.Domain.csproj`:

```xml

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.0.0" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="Configurations\" />
    <Folder Include="Enums\" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\GeneralLabSolutions.SharedKernel\GeneralLabSolutions.SharedKernel.csproj" />
  </ItemGroup>

</Project>


```
---

- `DomainException`:

```csharp

namespace GeneralLabSolutions.Domain.DomainObjects
{
    public class DomainException : Exception
    {
        public DomainException()
        { }

        public DomainException(string message) : base(message)
        { }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}


```
---


- `MediatRExtensions`:

```csharp

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

```
---
