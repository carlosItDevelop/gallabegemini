### Features da Solução GeneralLabSolutions - Parte 4

> A maioria das features abaixo ainda tem a ver com Cliente, direta ou indiretamente:

- `IPessoaContainer`:
- `DadosBancariosDomainHelper`:
- `TelefoneDomainHelper`:
- `ContatoDomainHelper`:
- `EnderecoDomainHelper`:
- `AutoMapperConfig`:
- `AspNetUser.cs` (interface e classe para manter o usuário logado acessível na solution):
- `ClaimsPrincipalExtensions` (Extension Methods para ClaimsPrincipal):
- `AppDbContext`:
- `AppDbContextFactory`:
- `TempDataKeys`:
- `BaseMvcController`:
- `ClienteController` (refatorar):
- `AppSettingsMvc`:
- `MvcConfig` (documentar a linha que libera o .Net de validar os campos que deixei opcional):
- `Program.cs` (refatorar: verificar se tem como usar mais o `OCP` do `SOLID` para enxugar esta classe):
- `MappingExtensions` (auxiliar configurações de enums nos mapping Fluent API):
- `SeedDataCliente` (refatorar: considerar o uso "ou não" de `using` e `ServiceProvider`):
- `DbInitializer` (refatorar - encontrar uma solução elegante para tratar a transação):
- `DependencyInjectionConfig`:
- `CustomHttpRequestException`:
---


- `IPessoaContainer`:

```csharp

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public interface IPessoaContainer
    {
        Pessoa Pessoa { get; }
        Guid Id { get; }
        void AdicionarEvento(Event evento);
    }
}

```
---


- `DadosBancariosDomainHelper`:

```csharp

using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class DadosBancariosDomainHelper
    {
        public static DadosBancarios AdicionarDadosBancariosGenerico<T>(
            T aggregateRoot,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novo = new DadosBancarios(banco, agencia, conta, tipoConta, aggregateRoot.Pessoa.Id);

            aggregateRoot.Pessoa.DadosBancarios.Add(novo);

            aggregateRoot.AdicionarEvento(new DadosBancariosAdicionadosEvent(
                aggregateRoot.Id,
                dadosBancariosId: novo.Id,
                banco: banco,
                agencia: agencia,
                conta: conta,
                tipoDeContaBancaria: tipoConta
            ));

            return novo;
        }

        public static void AtualizarDadosBancariosGenerico<T>(
            T aggregateRoot,
            Guid dadosBancariosId,
            string banco,
            string agencia,
            string conta,
            TipoDeContaBancaria tipoConta)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.DadosBancarios is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Dados Bancários associada ao {typeof(T).Name} não foi carregada.");

            var dadosBancariosParaAtualizar = aggregateRoot.Pessoa.DadosBancarios.FirstOrDefault(db => db.Id == dadosBancariosId);

            if (dadosBancariosParaAtualizar is null)
            {
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este {typeof(T).Name.ToLower()}.");
            }

            dadosBancariosParaAtualizar.SetBanco(banco);
            dadosBancariosParaAtualizar.SetAgencia(agencia);
            dadosBancariosParaAtualizar.SetConta(conta);
            dadosBancariosParaAtualizar.SetTipoDeContaBancaria(tipoConta);

            aggregateRoot.AdicionarEvento(new DadosBancariosAtualizadosEvent(
               aggregateId: aggregateRoot.Id,
               dadosBancariosId: dadosBancariosId,
               banco: banco,
               agencia: agencia,
               conta: conta,
               tipoDeContaBancaria: tipoConta
           ));
        }

        public static DadosBancarios RemoverDadosBancariosGenerico<T>(T aggregateRoot, Guid dadosBancariosId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.DadosBancarios is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Dados Bancários associada ao {typeof(T).Name} não foi carregada.");

            var db = aggregateRoot.Pessoa.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId);

            if (db is null)
            {
                throw new InvalidOperationException($"Dados bancários com ID {dadosBancariosId} não encontrados para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.DadosBancarios.Remove(db);

            aggregateRoot.AdicionarEvento(new DadosBancariosRemovidosEvent(aggregateRoot.Id, dadosBancariosId));
            return db;
        }
    }
}

```
---


- `TelefoneDomainHelper`:

```csharp

using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class TelefoneDomainHelper
    {
        public static Telefone AdicionarTelefoneGenerico<T>(T aggregateRoot, string ddd, string numero, TipoDeTelefone tipoTelefone)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoTelefone = new Telefone(ddd, numero, tipoTelefone, aggregateRoot.Pessoa.Id);

            aggregateRoot.Pessoa.Telefones.Add(novoTelefone);

            aggregateRoot.AdicionarEvento(new TelefoneAdicionadoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: novoTelefone.Id,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));

            return novoTelefone;
        }

        public static void AtualizarTelefoneGenerico<T>(T aggregateRoot, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Telefones is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Telefones associada ao {typeof(T).Name} não foi carregada.");

            var telefoneParaAtualizar = aggregateRoot.Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaAtualizar is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            telefoneParaAtualizar.SetDDD(ddd);
            telefoneParaAtualizar.SetNumero(numero);
            telefoneParaAtualizar.SetTipoDeTelefone(tipoTelefone);

            aggregateRoot.AdicionarEvento(new TelefoneAtualizadoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: telefoneId,
                ddd: ddd,
                numero: numero,
                tipoDeTelefone: tipoTelefone
            ));
        }

        public static Telefone RemoverTelefoneGenerico<T>(T aggregateRoot, Guid telefoneId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var telefoneParaRemover = aggregateRoot.Pessoa.Telefones.FirstOrDefault(t => t.Id == telefoneId);

            if (telefoneParaRemover is null)
            {
                throw new InvalidOperationException($"Telefone com ID {telefoneId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Telefones.Remove(telefoneParaRemover);

            aggregateRoot.AdicionarEvento(new TelefoneRemovidoEvent(
                aggregateId: aggregateRoot.Id,
                telefoneId: telefoneId
            ));

            return telefoneParaRemover;
        }
    }
}

```
---


- `ContatoDomainHelper`:

```csharp

using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class ContatoDomainHelper
    {
        public static Contato AdicionarContatoGenerico<T>(
            T aggregateRoot,
            string nome,
            string email,
            string telefone,
            TipoDeContato tipoDeContato,
            string emailAlternativo = "",
            string telefoneAlternativo = "",
            string observacao = "")
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoContato = new Contato(
                                    nome,
                                    email,
                                    telefone,
                                    tipoDeContato,
                                    aggregateRoot.Pessoa.Id
                                )
            {
                EmailAlternativo = emailAlternativo ?? string.Empty,
                TelefoneAlternativo = telefoneAlternativo ?? string.Empty,
                Observacao = observacao ?? string.Empty
            };

            aggregateRoot.Pessoa.Contatos.Add(novoContato);

            aggregateRoot.AdicionarEvento(new ContatoAdicionadoEvent(
                                aggregateId: aggregateRoot.Id,
                                contatoId: novoContato.Id,
                                nome: nome,
                                email: email,
                                telefone: telefone,
                                tipoDeContato: tipoDeContato
            ));

            return novoContato;
        }

        public static void AtualizarContatoGenerico<T>(
                                T aggregateRoot,
                                Guid contatoId,
                                string nome,
                                string email,
                                string telefone,
                                TipoDeContato tipoDeContato,
                                string emailAlternativo = "",
                                string telefoneAlternativo = "",
                                string observacao = "")
                                where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Contatos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Contatos associada ao {typeof(T).Name} não foi carregada.");

            var contatoParaAtualizar = aggregateRoot.Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            contatoParaAtualizar.AtualizarNome(nome);
            contatoParaAtualizar.AtualizarEmail(email);
            contatoParaAtualizar.AtualizarTelefone(telefone);
            contatoParaAtualizar.DefineTipoDeContato(tipoDeContato);
            contatoParaAtualizar.AtualizarEmailAlternativo(emailAlternativo);
            contatoParaAtualizar.AtualizarTelefoneAlternativo(telefoneAlternativo);
            contatoParaAtualizar.AtualizarObservacao(observacao);

            aggregateRoot.AdicionarEvento(new ContatoAtualizadoEvent(
                                aggregateId: aggregateRoot.Id,
                                contatoId: contatoId,
                                nome: nome,
                                email: email,
                                telefone: telefone,
                                tipoDeContato: tipoDeContato
            ));
        }

        public static Contato RemoverContatoGenerico<T>(T aggregateRoot, Guid contatoId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Contatos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Contatos associada ao {typeof(T).Name} não foi carregada.");

            var contatoParaRemover = aggregateRoot.Pessoa.Contatos.FirstOrDefault(c => c.Id == contatoId);

            if (contatoParaRemover is null)
            {
                throw new InvalidOperationException($"Contato com ID {contatoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Contatos.Remove(contatoParaRemover);

            aggregateRoot.AdicionarEvento(new ContatoRemovidoEvent(
                                aggregateId: aggregateRoot.Id,
                                contatoId: contatoId
            ));

            return contatoParaRemover;
        }
    }
}

```
---


- `EnderecoDomainHelper`:

```csharp

using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Services.Helpers
{
    public static class EnderecoDomainHelper
    {
        public static Endereco AdicionarEnderecoGenerico<T>(
            T aggregateRoot,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null)
                throw new InvalidOperationException($"A Pessoa associada ao {typeof(T).Name} não foi carregada.");

            var novoEndereco = new Endereco(
                aggregateRoot.Pessoa.Id, // FK para Pessoa
                paisCodigoIso,
                linhaEndereco1,
                cidade,
                codigoPostal,
                tipoDeEndereco,
                linhaEndereco2,
                estadoOuProvincia,
                informacoesAdicionais
            );

            aggregateRoot.Pessoa.Enderecos.Add(novoEndereco);

            aggregateRoot.AdicionarEvento(new EnderecoAdicionadoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: novoEndereco.Id,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));

            return novoEndereco;
        }

        public static void AtualizarEnderecoGenerico<T>(
            T aggregateRoot,
            Guid enderecoId,
            string paisCodigoIso,
            string linhaEndereco1,
            string cidade,
            string codigoPostal,
            Endereco.TipoDeEnderecoEnum tipoDeEndereco,
            string? linhaEndereco2 = null,
            string? estadoOuProvincia = null,
            string? informacoesAdicionais = null)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Enderecos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Endereços associada ao {typeof(T).Name} não foi carregada.");

            var enderecoParaAtualizar = aggregateRoot.Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaAtualizar is null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            enderecoParaAtualizar.SetPaisCodigoIso(paisCodigoIso);
            enderecoParaAtualizar.SetLinhaEndereco1(linhaEndereco1);
            enderecoParaAtualizar.SetLinhaEndereco2(linhaEndereco2);
            enderecoParaAtualizar.SetCidade(cidade);
            enderecoParaAtualizar.SetEstadoOuProvincia(estadoOuProvincia);
            enderecoParaAtualizar.SetCodigoPostal(codigoPostal);
            enderecoParaAtualizar.SetTipoDeEndereco(tipoDeEndereco);
            enderecoParaAtualizar.SetInformacoesAdicionais(informacoesAdicionais);

            aggregateRoot.AdicionarEvento(new EnderecoAtualizadoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: enderecoId,
                paisCodigoIso: paisCodigoIso,
                linhaEndereco1: linhaEndereco1,
                cidade: cidade,
                codigoPostal: codigoPostal,
                tipoDeEndereco: tipoDeEndereco
            ));
        }

        public static Endereco RemoverEnderecoGenerico<T>(T aggregateRoot, Guid enderecoId)
            where T : IPessoaContainer
        {
            if (aggregateRoot.Pessoa is null || aggregateRoot.Pessoa.Enderecos is null)
                throw new InvalidOperationException($"A Pessoa ou a coleção de Endereços associada ao {typeof(T).Name} não foi carregada.");

            var enderecoParaRemover = aggregateRoot.Pessoa.Enderecos.FirstOrDefault(e => e.Id == enderecoId);

            if (enderecoParaRemover is null)
            {
                throw new InvalidOperationException($"Endereço com ID {enderecoId} não encontrado para este {typeof(T).Name.ToLower()}.");
            }

            aggregateRoot.Pessoa.Enderecos.Remove(enderecoParaRemover);

            aggregateRoot.AdicionarEvento(new EnderecoRemovidoEvent(
                aggregateId: aggregateRoot.Id,
                enderecoId: enderecoId
            ));

            return enderecoParaRemover;
        }
    }
}

```
---


- `AutoMapperConfig`:

```csharp

using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using VelzonModerna.ViewModels;
using VelzonModerna.ViewModels.CRM;

namespace VelzonModerna.Configuration.Mappings
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Cliente, ClienteViewModel>()
                .ForMember(dest => dest.PessoaId, opt => opt.MapFrom(src => src.PessoaId)) // Cliente -> ViewModel (OK)
                .ForMember(dest => dest.DadosBancarios, opt => opt.MapFrom(src => src.Pessoa.DadosBancarios))
                .ForMember(dest => dest.Telefones, opt => opt.MapFrom(src => src.Pessoa.Telefones))
                .ForMember(dest => dest.Contatos, opt => opt.MapFrom(src => src.Pessoa.Contatos))
                .ForMember(dest => dest.Enderecos, opt => opt.MapFrom(src => src.Pessoa.Enderecos))
                .ReverseMap() // ViewModel -> Cliente
                              // *** ADICIONAR ESTA LINHA PARA O MAPEAMENTO REVERSO ***
                    .ForMember(dest => dest.PessoaId, opt => opt.Ignore())
                    // Ignora PessoaId ao mapear ViewModel -> Cliente
                    // Ignorar Pessoa também, pois não queremos substituir a instância carregada
                    .ForMember(dest => dest.Pessoa, opt => opt.Ignore())
                    // Ignorar coleções também, elas são gerenciadas separadamente
                    .ForMember(dest => dest.Pedidos, opt => opt.Ignore());
            // As coleções em Pessoa (DadosBancarios, etc.) não existem no ClienteViewModel, então não precisam ser ignoradas explicitamente aqui.

            CreateMap<Contato, ContatoViewModel>().ReverseMap();
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<CategoriaProduto, CategoriaProdutoViewModel>().ReverseMap();
            CreateMap<Vendedor, VendedorViewModel>()
                .ForMember(dest => dest.PessoaId, opt => opt.MapFrom(src => src.PessoaId))
                .ForMember(dest => dest.DadosBancarios, opt => opt.MapFrom(src => src.Pessoa.DadosBancarios))
                .ForMember(dest => dest.Telefones, opt => opt.MapFrom(src => src.Pessoa.Telefones))
                .ForMember(dest => dest.Contatos, opt => opt.MapFrom(src => src.Pessoa.Contatos))
                .ForMember(dest => dest.Enderecos, opt => opt.MapFrom(src => src.Pessoa.Enderecos))
                .ReverseMap()
                    .ForMember(dest => dest.PessoaId, opt => opt.Ignore())
                    .ForMember(dest => dest.Pessoa, opt => opt.Ignore())
                    .ForMember(dest => dest.Pedidos, opt => opt.Ignore());
            CreateMap<Produto, ProdutoViewModel>().ReverseMap();
            CreateMap<Pedido, PedidoViewModel>().ReverseMap();

            CreateMap<Telefone, TelefoneViewModel>().ReverseMap(); // PessoaId será mapeado por convenção

            CreateMap<Conta, ContaViewModel>().ReverseMap();

            CreateMap<Endereco, EnderecoViewModel>()
                            // Mapeia o enum interno para o enum do ViewModel (se os nomes/valores forem iguais, pode ser por convenção)
                            .ForMember(dest => dest.TipoDeEndereco, opt => opt.MapFrom(src => (Endereco.TipoDeEnderecoEnum)src.TipoDeEndereco))
                            .ReverseMap()
                            .ForMember(dest => dest.TipoDeEndereco, opt => opt.MapFrom(src => (Endereco.TipoDeEnderecoEnum)src.TipoDeEndereco)); // Para mapeamento reverso

            CreateMap<DadosBancarios, DadosBancariosViewModel>().ReverseMap();

            // Kanban Mapper
            CreateMap<KanbanTask, KanbanTaskViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Participantes, opt => opt.Ignore());
            CreateMap<Participante, ParticipanteViewModel>().ReverseMap();

            // Para CRM Leads

            // --- ✅ INÍCIO DOS MAPEAMENTOS DO MÓDULO CRM ---
            CreateMap<Lead, LeadViewModel>().ReverseMap();
            CreateMap<CrmTask, CrmTaskViewModel>().ReverseMap();
            CreateMap<Activity, ActivityViewModel>().ReverseMap();
            CreateMap<LeadNote, LeadNoteViewModel>().ReverseMap();
            CreateMap<Log, LogViewModel>().ReverseMap();
            CreateMap<CrmTaskComment, CrmTaskCommentViewModel>()
            // ✅ Mapeia a propriedade 'DataInclusao' da entidade para 'CreatedAt' na ViewModel
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.DataInclusao));
            // O mapeamento reverso pode continuar simples, pois não precisamos mapear a data de volta
            CreateMap<CrmTaskCommentViewModel, CrmTaskComment>();
            CreateMap<CrmTaskAttachment, CrmTaskAttachmentViewModel>().ReverseMap();
            // --- FIM DOS MAPEAMENTOS DO MÓDULO CRM ---


        }
    }
}

```
---

- `AspNetUser.cs` (interface e classe para manter o usuário logado acessível na solution):

```csharp

using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GeneralLabSolutions.WebApiCore.Usuario
{

    public interface IAspNetUser
    {
        string Name { get; }
        Guid ObterUserId();
        string ObterUserEmail();
        string ObterUserToken();
        string ObterUserRefreshToken();
        bool EstaAutenticado();
        bool PossuiRole(string role);
        IEnumerable<Claim> ObterClaims();
        HttpContext ObterHttpContext();

        // Novas propriedades/métodos
        string ObterNomeCompleto();
        string ObterApelido();
        DateTime? ObterDataNascimento(); // Nullable se a claim puder não existir ou falhar no parse
        string ObterImgProfilePath();

    }



    namespace GeneralLabSolutions.WebApiCore.Usuario
    {
        // Interface IAspNetUser permanece a mesma

        public class AspNetUser : IAspNetUser
        {
            private readonly IHttpContextAccessor _accessor;

            public AspNetUser(IHttpContextAccessor accessor)
            {
                _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor)); // Boa prática adicionar null check no construtor
            }

            // Helper para verificar se o contexto e o usuário são válidos
            private ClaimsPrincipal? GetUser() => _accessor?.HttpContext?.User;

            // Propriedade Name precisa de proteção
            public string Name => GetUser()?.Identity?.Name ?? string.Empty; // Retorna string vazia se algo for nulo

            public Guid ObterUserId()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? Guid.Parse(GetUser()?.GetUserId() ?? Guid.Empty.ToString()) : Guid.Empty;
            }

            public string ObterUserEmail()
            {
                return EstaAutenticado() ? GetUser()?.GetUserEmail() ?? "" : "";
            }

            public string ObterUserToken()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? GetUser()?.GetUserToken() ?? "" : "";
            }

            public string ObterUserRefreshToken()
            {
                // EstaAutenticado() já fará a checagem de null
                return EstaAutenticado() ? GetUser()?.GetUserRefreshToken() ?? "" : "";
            }

            // *** MÉTODO CRÍTICO CORRIGIDO ***
            public bool EstaAutenticado()
            {
                // Verifica se HttpContext existe E se a identidade existe E se está autenticada
                return GetUser()?.Identity?.IsAuthenticated ?? false; // Retorna false se qualquer parte for nula
            }

            public bool PossuiRole(string role)
            {
                // Verifica se HttpContext existe antes de chamar IsInRole
                return GetUser()?.IsInRole(role) ?? false; // Retorna false se HttpContext ou User for nulo
            }

            public IEnumerable<Claim> ObterClaims()
            {
                // Retorna claims ou uma coleção vazia se HttpContext ou User for nulo
                return GetUser()?.Claims ?? Enumerable.Empty<Claim>();
            }

            public HttpContext ObterHttpContext()
            {
                // Retorna o HttpContext diretamente (pode ser nulo, o chamador deve saber)
                return _accessor?.HttpContext;
            }

            // Os métodos abaixo já usavam o operador null-conditional (?.), o que é bom,
            // mas vamos garantir consistência usando GetUser() onde apropriado.
            public string ObterNomeCompleto()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetNomeCompleto() ?? string.Empty;
            }

            public string ObterApelido()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetApelido() ?? string.Empty;
            }

            public DateTime? ObterDataNascimento()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetDataNascimento(); // Já retorna nullable
            }

            public string ObterImgProfilePath()
            {
                // Usar GetUser() garante a checagem do HttpContext também
                return GetUser()?.GetImgProfilePath() ?? string.Empty;
            }
        }
    }
}


```
---


- `ClaimsPrincipalExtensions` (Extension Methods para ClaimsPrincipal):


```csharp

using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GeneralLabSolutions.WebApiCore.Usuario
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtém o User ID (sub ou NameIdentifier) da Claim.
        /// Retorna string.Empty se a claim não for encontrada.
        /// </summary>
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                // Considerar lançar ArgumentNullException se principal não deveria ser nulo aqui.
                // Mas para um método de extensão, retornar um valor padrão pode ser aceitável.
                return string.Empty;
            }

            // Tenta primeiro "sub" (padrão OIDC para ID do usuário)
            var claim = principal.FindFirst("sub") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o email do usuário da Claim "email" ou ClaimTypes.Email.
        /// Retorna string.Empty se a claim não for encontrada.
        /// </summary>
        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // Tenta primeiro "email" (comum em JWTs) e depois o ClaimTypes.Email padrão
            var claim = principal.FindFirst("email") ?? principal.FindFirst(ClaimTypes.Email);
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o Access Token (JWT) que foi armazenado como uma claim.
        /// Retorna string.Empty se a claim "JWT" não for encontrada.
        /// </summary>
        public static string GetUserToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // A claim "JWT" foi adicionada manualmente em AutenticacaoService.RealizarLogin
            var claim = principal.FindFirst("JWT");
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Obtém o Refresh Token que foi armazenado como uma claim.
        /// Retorna string.Empty se a claim "RefreshToken" não for encontrada.
        /// </summary>
        public static string GetUserRefreshToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }
            // A claim "RefreshToken" foi adicionada manualmente em AutenticacaoService.RealizarLogin
            var claim = principal.FindFirst("RefreshToken");
            return claim?.Value ?? string.Empty;
        }


        // Novos métodos para Claims adicionais
        public static string GetNomeCompleto(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("nome_completo") ?? principal.FindFirst(JwtRegisteredClaimNames.Name);
            return claim?.Value ?? string.Empty;
        }

        public static string GetApelido(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("apelido");
            return claim?.Value ?? string.Empty;
        }

        public static DateTime? GetDataNascimento(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return null;

            var claim = principal.FindFirst("birthdate") ?? principal.FindFirst(ClaimTypes.DateOfBirth);
            if (claim?.Value != null)
            {
                // Se armazenou como "o" (round-trip date/time pattern)
                if (DateTime.TryParse(claim.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dataNascimento))
                {
                    return dataNascimento;
                }
                // Se armazenou como "yyyy-MM-dd"
                if (DateTime.TryParseExact(claim.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataNascimento))
                {
                    return dataNascimento;
                }
            }
            return null;
        }

        public static string GetImgProfilePath(this ClaimsPrincipal principal)
        {
            if (principal == null)
                return string.Empty;
            // Use o nome da claim que você definiu na API de Identidade
            var claim = principal.FindFirst("img_profile_path");
            return claim?.Value ?? string.Empty;
        }


    }
}

```
---


- `AppDbContext`:

```csharp

using FluentValidation.Results;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Data.ORM
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


```
---


- `AppDbContextFactory`:

```csharp

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GeneralLabSolutions.InfraStructure.Data.ORM
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string [] args)
        {
            // 1. Construir a configuração
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Define o diretório base
                .AddJsonFile("appsettings.json", optional: true) // Lê appsettings.json se existir
                .AddJsonFile($"appsettings.Development.json", optional: true) // Lê appsettings.Development.json se existir e se for o ambiente de desenvolvimento
                .AddUserSecrets<AppDbContextFactory>() // Lê User Secrets (importante para desenvolvimento!)
                .Build();

            // 2. Obter a string de conexão da configuração
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // Ou o nome que você estiver usando na configuração

            // 3. Usar a string de conexão lida
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // No design-time, não tentamos injetar IMediatorHandler – passamos null
            return new AppDbContext(optionsBuilder.Options, null);
        }
    }
}

```
---


- `TempDataKeys`:

```csharp


namespace VelzonModerna.Helpers
{
    public static class TempDataKeys
    {
        public const string SuccessMessage = "Success";
        public const string ErrorMessage = "Error";   // Ou "Error"
        public const string ExclusionMessage = "Excluido";
        public const string InfoMessage = "Info"; // Ou "Informação"
    }
}


```
---


- `BaseMvcController` (refatorar):

```csharp

using GeneralLabSolutions.Domain.Notifications;
using Microsoft.AspNetCore.Mvc;
using velzon.Models;

namespace VelzonModerna.Controllers.Base
{
    public abstract class BaseMvcController : Controller
    {
        private readonly INotificador _notificador;

        protected BaseMvcController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        // *** NOVO MÉTODO ***
        // Prática menos comum, mas podemos refatorar, depois, em um único lugar
        // Como INotificador é injetado, podemos usar diretamente.
        // Isso é útil para evitar duplicação de código em cada controller.
        // Como o método Notificar é protegido na classe base, que está em outro projeto,
        // não podemos chamá-lo diretamente na controller.
        // ToDo: Refatorar para usar o mesmo método (Notificar) em todos os lugares!
        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

    }
}


```
---


- `ClienteController` (refatorar: encontrar falha na implementação de obter 'agregados' para 'offCanvas' para edição/exclusão):

```csharp

using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;


namespace VelzonModerna.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClienteController : BaseMvcController
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepository _clienteRepository;
        private readonly IClienteDomainService _clienteDomainService;

        #region: Construtor (Existente)
        public ClienteController(
            INotificador notificador,
            IMapper mapper,
            IClienteRepository clienteRepository,
            IClienteDomainService clienteDomainService)
            : base(notificador)
        {
            _mapper = mapper;
            _clienteRepository = clienteRepository;
            _clienteDomainService = clienteDomainService;
        }
        #endregion

        #region: Métodos Cliente (Index, Create, Edit, Delete - Ajustar Details/Edit GET)

        //[Authorize(Roles = "Admin")]
        //[ClaimsAuthorize("Cliente", "Visualizar", "Admin")] // Role alternativa não especificada
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            var listClienteViewModel = _mapper.Map<IEnumerable<ClienteViewModel>>(clientes);
            return View(listClienteViewModel);
        }

        [HttpGet]
        //[ClaimsAuthorize("role", "Admin")]
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Details(Guid id)
        {

            // *** ATUALIZAR: Usar ObterClienteCompleto ou carregar Telefones também ***
            // var cliente = await _clienteRepository.ObterClienteComDadosBancarios(id); // Linha antiga
            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega DadosBancarios e Telefones (e futuros)

            if (cliente is null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteViewModel);
        }

        public IActionResult Create() => View(new ClienteViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel clienteViewModel)
        {
            if (!ModelState.IsValid)
                return View(clienteViewModel);
            var cliente = _mapper.Map<Cliente>(clienteViewModel);
            await _clienteDomainService.AddClienteAsync(cliente);
            if (!OperacaoValida())
                return View(clienteViewModel);
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados do cliente.");
                return View(clienteViewModel);
            }
            TempData ["Success"] = "Cliente Adicionado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            // *** ATUALIZAR: Usar ObterClienteCompleto ou carregar Telefones também ***
            // var cliente = await _clienteRepository.ObterClienteComDadosBancarios(id); // Linha antiga
            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega DadosBancarios e Telefones (e futuros)

            if (cliente == null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);

            return View(clienteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ClienteViewModel clienteViewModel)
        {
            if (id != clienteViewModel.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(clienteViewModel);

            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega com relações
            if (cliente is null)
                return NotFound();

            _mapper.Map(clienteViewModel, cliente); // Mapeia ClienteViewModel para a entidade Cliente rastreada

            await _clienteDomainService.UpdateClienteAsync(cliente);

            if (!OperacaoValida())
                return View(clienteViewModel);
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao atualizar os dados do cliente.");
                var clienteAtualizado = await _clienteRepository.ObterClienteCompleto(id);
                var viewModelAtualizado = _mapper.Map<ClienteViewModel>(clienteAtualizado);
                return View(viewModelAtualizado);
            }
            TempData ["Success"] = "Cliente Atualizado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null)
                return NotFound();
            await _clienteDomainService.DeleteClienteAsync(cliente);
            if (!OperacaoValida())
            {
                var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
                return View(clienteViewModel); // Retorna para a view Delete com erros
            }
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir o cliente. Verifique dependências.");
                var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
                return View("Delete", clienteViewModel);
            }
            TempData ["Success"] = "Cliente excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        #endregion



        #region: Actions AJAX para Dados Bancários (Existente)
        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosListPartial(Guid clienteId)
        {
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var dadosBancariosEntities = cliente?.Pessoa?.DadosBancarios ?? new List<DadosBancarios>();
                var dadosBancariosViewModels = _mapper.Map<List<DadosBancariosViewModel>>(dadosBancariosEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "DadosBancarios",
                    items = dadosBancariosViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosFormData(Guid? dadosBancariosId, Guid clienteId)
        {
            // Busca o cliente primeiro para todas as operações, garantindo que ele existe.
            var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
            if (cliente?.Pessoa is null)
            {
                return NotFound("Cliente ou dados de Pessoa associados não encontrados.");
            }

            if (dadosBancariosId is null || dadosBancariosId == Guid.Empty)
            {
                // Modo Criação: Retorna um ViewModel com o PessoaId correto.
                var newViewModel = new DadosBancariosViewModel { PessoaId = cliente.PessoaId };
                return Json(newViewModel);

            } else
            {
                // Modo Edição: Busca o item específico na coleção já carregada.
                var dadosBancarios = cliente.Pessoa.DadosBancarios.FirstOrDefault(d => d.Id == dadosBancariosId.Value);

                if (dadosBancarios is null)
                    return NotFound("Dados bancários não encontrados ou não pertencem a este cliente.");

                var viewModel = _mapper.Map<DadosBancariosViewModel>(dadosBancarios);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarDadosBancarios(
            Guid clienteId,
            DadosBancariosViewModel viewModel)
        {
            // 1. Validação do modelo recebido
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // 2. Validação do parâmetro de rota
            if (clienteId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "O ID do Cliente não foi fornecido." }
                });
            }

            // 3. Inclusão ou atualização dos dados bancários
            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarDadosBancariosAsync(
                    clienteId,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            } else
            {
                await _clienteDomainService.AtualizarDadosBancariosAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            }

            // 4. Checagem de notificações de domínio
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            // 5. Persistência e tratamento de exceções
            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new
                    {
                        success = false,
                        errors = new List<string> { "Commit retornou falso." }
                    });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine(
                    $"DbUpdateException: {dbEx.Message} " +
                    $"Inner: {dbEx.InnerException?.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro no banco." }
                });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro inesperado." }
                });
            }

            // 6. Resposta final de sucesso
            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirDadosBancarios(
            Guid clienteId,
            Guid dadosBancariosId)
        {
            // 1. Validação de parâmetros
            if (clienteId == Guid.Empty || dadosBancariosId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "IDs inválidos." }
                });
            }

            // 2. Remoção dos dados bancários
            await _clienteDomainService.RemoverDadosBancariosAsync(
                clienteId,
                dadosBancariosId);

            // 3. Verificação de notificações de domínio
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            // 4. Persistência da exclusão
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro ao excluir." }
                });
            }

            // 5. Resposta final de sucesso
            return Json(new { success = true });
        }


        #endregion

        #region Actions AJAX para Telefones

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de telefones para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTelefonesListPartial(Guid clienteId)
        {
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var telefoneEntities = cliente?.Pessoa?.Telefones ?? new List<Telefone>();
                var telefoneViewModels = _mapper.Map<List<TelefoneViewModel>>(telefoneEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "Telefone",
                    items = telefoneViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }
        }


        /// <summary>
        /// Retorna os dados de um telefone específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTelefoneFormData(Guid? telefoneId, Guid clienteId) // clienteId para caso de criação
        {
            if (telefoneId is null || telefoneId == Guid.Empty)
            {
                // ...
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar telefone.");

                // Para Telefone, precisamos do PessoaId do Cliente para preencher o formulário
                // *** CORREÇÃO APLICADA AQUI ***
                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);
                // ******************************

                if (clienteParaPessoaId is null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId.");

                var newViewModel = new TelefoneViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
                // ...
            } else
            {
                // Modo Edição: Buscar telefone pelo seu ID.
                // Idealmente, ter um _telefoneRepository.GetByIdAsync(telefoneId.Value)
                // Por enquanto, vamos buscar através do cliente para garantir que pertence a ele.
                var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);
                var telefone = cliente?.Pessoa?.Telefones.FirstOrDefault(t => t.Id == telefoneId.Value);

                if (telefone is null)
                    return NotFound("Telefone não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<TelefoneViewModel>(telefone);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (adiciona ou atualiza) o telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTelefone(
            Guid clienteId,
            TelefoneViewModel viewModel)
        {
            /* 1. Validação do modelo --------------------------------------------- */
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            /* 2. Validação do parâmetro clienteId -------------------------------- */
            if (clienteId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "O ID do Cliente não foi fornecido."
                    }
                });
            }

            /* 3. Inclusão ou atualização do telefone ----------------------------- */
            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarTelefoneAsync(
                    clienteId,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            } else
            {
                await _clienteDomainService.AtualizarTelefoneAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            }

            /* 4. Verificação de notificações de domínio -------------------------- */
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            /* 5. Persistência e tratamento de exceções --------------------------- */
            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new
                    {
                        success = false,
                        errors = new List<string>
                        {
                            "Erro ao salvar o telefone no banco de dados (Commit retornou falso)."
                        }
                    });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine(
                    $"DbUpdateException ao salvar telefone: {dbEx.Message} " +
                    $"Inner: {dbEx.InnerException?.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "Erro no banco de dados ao salvar o telefone."
                    }
                });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar telefone: {ex.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "Ocorreu um erro inesperado ao salvar o telefone."
                    }
                });
            }

            /* 6. Resposta final de sucesso --------------------------------------- */
            return Json(new { success = true });
        }


        /// <summary>
        /// Exclui um telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTelefone(
            Guid clienteId,
            Guid telefoneId)
        {
            /* 1. Validação de parâmetros ----------------------------------------- */
            if (clienteId == Guid.Empty || telefoneId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "IDs inválidos fornecidos para exclusão."
                    }
                });
            }

            /* 2. Remoção do telefone --------------------------------------------- */
            await _clienteDomainService.RemoverTelefoneAsync(
                clienteId,
                telefoneId);

            /* 3. Verificação de notificações de domínio -------------------------- */
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            /* 4. Persistência da exclusão ---------------------------------------- */
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
            {
                "Erro ao excluir o telefone no banco de dados."
            }
                });
            }

            /* 5. Resposta final de sucesso --------------------------------------- */
            return Json(new { success = true });
        }


        #endregion




        #region Actions AJAX para Contatos

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de contatos para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContatosListPartial(Guid clienteId)
        {
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var contatoEntities = cliente?.Pessoa?.Contatos ?? new List<Contato>();
                var contatoViewModels = _mapper.Map<List<ContatoViewModel>>(contatoEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "Contato",
                    items = contatoViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        /// <summary>
        /// Retorna os dados de um contato específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContatoFormData(Guid? contatoId, Guid clienteId)
        {
            if (contatoId is null || contatoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar contato.");
                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);
                if (clienteParaPessoaId is null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o contato.");

                var newViewModel = new ContatoViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
            } else
            {
                // Modo Edição: Buscar contato pelo seu ID.
                var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);
                var contato = cliente?.Pessoa?.Contatos.FirstOrDefault(c => c.Id == contatoId.Value);

                if (contato is null)
                    return NotFound("Contato não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<ContatoViewModel>(contato);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (Adiciona ou Atualiza) o contato de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarContato(Guid clienteId, ContatoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (clienteId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            }

            // Poderia validar se viewModel.PessoaId corresponde ao PessoaId do Cliente com clienteId,
            // mas o serviço de domínio fará a associação correta com base no clienteId buscado.
            // A entidade Cliente.AdicionarContato usará o Cliente.PessoaId correto.

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarContatoAsync(
                    clienteId, // ID do Cliente
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao
                );
            } else
            {
                await _clienteDomainService.AtualizarContatoAsync(
                    clienteId,    // ID do Cliente
                    viewModel.Id, // ID do Contato a ser atualizado
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao
                );
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new { success = false, errors = new List<string> { "Erro ao salvar o contato no banco de dados (Commit retornou falso)." } });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException ao salvar contato: {dbEx.ToString()}"); // Log completo
                return Json(new { success = false, errors = new List<string> { "Erro no banco de dados ao salvar o contato." } });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar contato: {ex.ToString()}"); // Log completo
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro inesperado ao salvar o contato." } });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Exclui um contato de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirContato(Guid clienteId, Guid contatoId)
        {
            if (clienteId == Guid.Empty || contatoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _clienteDomainService.RemoverContatoAsync(clienteId, contatoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o contato no banco de dados." } });
            }

            return Json(new { success = true });
        }
        #endregion



        #region Actions AJAX para Endereços

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de endereços para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEnderecosListPartial(Guid clienteId)
        {

            try
            { 
            var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
            var enderecoEntities = cliente?.Pessoa?.Enderecos ?? new List<Endereco>();
            var enderecoViewModels = _mapper.Map<List<EnderecoViewModel>>(enderecoEntities);

            return ViewComponent("AggregateList", new
            {
                parentEntityType = "Cliente",
                parentEntityId = clienteId,
                parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                aggregateType = "Endereco",
                items = enderecoViewModels
            });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        /// <summary>
        /// Retorna os dados de um endereço específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEnderecoFormData(Guid? enderecoId, Guid clienteId)
        {
            if (enderecoId is null || enderecoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar endereço.");

                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);

                if (clienteParaPessoaId is null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o endereço.");

                var newViewModel = new EnderecoViewModel { PessoaId = clienteParaPessoaId.PessoaId };

                return Json(newViewModel);
            } else
            {
                // Modo Edição
                var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);
                var endereco = cliente?.Pessoa?.Enderecos.FirstOrDefault(e => e.Id == enderecoId.Value);

                if (endereco is null)
                    return NotFound("Endereço não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<EnderecoViewModel>(endereco);

                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (Adiciona ou Atualiza) o endereço de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEndereco(Guid clienteId, EnderecoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (clienteId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarEnderecoAsync(
                    clienteId,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais
                );
            } else
            {
                await _clienteDomainService.AtualizarEnderecoAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais
                );
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new { success = false, errors = new List<string> { "Erro ao salvar o endereço (Commit retornou falso)." } });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException ao salvar endereço: {dbEx.ToString()}");
                return Json(new { success = false, errors = new List<string> { "Erro no banco de dados ao salvar o endereço." } });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar endereço: {ex.ToString()}");
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro inesperado ao salvar o endereço." } });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Exclui um endereço de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEndereco(Guid clienteId, Guid enderecoId)
        {
            if (clienteId == Guid.Empty || enderecoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _clienteDomainService.RemoverEnderecoAsync(clienteId, enderecoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o endereço no banco de dados." } });
            }

            return Json(new { success = true });
        }
        #endregion

        #region Métodos Privados Auxiliares (Existente)
        private async Task<bool> ClienteExists(Guid id) => await _clienteRepository.TemCliente(id);

        private List<Notificacao> ObterNotificacoes() => HttpContext.RequestServices.GetRequiredService<INotificador>().ObterNotificacoes();

        #endregion


    }
}

```
---


- `AppSettingsMvc`:

```csharp

namespace VelzonModerna.Configuration.Extensions
{
    public class AppSettingsMvc
    {
        public string AutenticacaoUrl { get; set; }
        public string UserAdminUrl { get; set; } = "https://localhost:5013/api/admin";
    }
}


```
---


- `MvcConfig` (documentar a linha que libera o .Net de validar os campos que deixei opcional):

```csharp

using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Configuration.Extensions
{
    public static class MvcConfig
    {
        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
        {

            services.AddControllers();

            services.AddControllersWithViews(o =>
            {
                o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "O valor preenchido é inválido para este campo.");
                o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => "Este campo precisa ser preenchido.");
                o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este campo precisa ser preenchido.");
                o.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "É necessário que o body na requisição não esteja vazio.");
                o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
                o.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor preenchido é inválido para este campo.");
                o.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "O campo deve ser numérico");
                o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
                o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
                o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => "O campo deve ser numérico.");
                o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "Este campo precisa ser preenchido.");
                o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

                o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

            }).AddJsonOptions(options =>
            {
                // Isso força a serialização de Enums para camelCase (ex: "novo", "pendente")
                // e resolve a maioria dos nossos problemas de filtro e labels.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });


            services.AddRazorPages();

            return services;
        }
    }
}


```
---


- `Program.cs` (do projeto MVC)=>(refatorar: verificar se tem como usar mais o `OCP` do `SOLID` para enxugar esta classe):

```csharp

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

        // Paginação (se usado no MVC) - Se for usado pelo MVC, senão remover.
        builder.Services.AddScoped<IPaginationService, PaginationService>();

        // Configurações de Autenticação e Identity do ASP.NET Core (Cookies, etc.)
        builder.Services.AddIdentityConfiguration();

        builder.Services.Configure<AppSettingsMvc>(options =>
        {
            // ToDo: Verifique se as chaves estão corretas no appsettings.json ou secrets.json
            // ToDo: Refatorar para evitar hardcoding de chaves
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
            app.UseExceptionHandler("/Home/Error"); // Ou página de erro customizada
            app.UseHsts();
        } else
        {
            app.UseDeveloperExceptionPage();
        }

        // Middleware de tratamento de exceções customizado (para 401, 404, etc.)
        app.UseMiddleware<ExceptionMiddleware>(); // Certificar se este middleware está 100% funcional

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Essencial: Autenticação DEPOIS de Routing e ANTES de Authorization e Endpoints
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=GalLabs}/{action=GlDashboard}/{id?}");

        app.Run();
    }
}

```
---


- `MappingExtensions` (auxiliar configurações de enums nos mapping Fluent API):

```csharp

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace GeneralLabSolutions.InfraStructure.Extensions
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Configura a conversão de um Enum para sua representação em string,
        /// respeitando o atributo [EnumMember(Value = "...")].
        /// </summary>
        public static PropertyBuilder<TEnum> HasEnumConversion<TEnum>(this PropertyBuilder<TEnum> propertyBuilder)
            where TEnum : struct, Enum
        {
            // O conversor que transforma o Enum em string para o banco
            var toProvider = new ValueConverter<TEnum, string>(
                v => v.ToEnumString(), // Usa nosso método customizado
                v => v.ToEnum<TEnum>() // Usa nosso método customizado
            );

            propertyBuilder.HasConversion(toProvider);
            propertyBuilder.HasColumnType("varchar(40)");

            return propertyBuilder;
        }

        // --- MÉTODOS HELPERS PRIVADOS ---

        // Converte um Enum para a string (lendo o atributo)
        private static string ToEnumString<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            var name = Enum.GetName(enumType, value);
            if (name == null)
                return string.Empty;

            var enumMemberAttribute = enumType.GetField(name)?
                .GetCustomAttributes(typeof(EnumMemberAttribute), true)
                .Cast<EnumMemberAttribute>()
                .FirstOrDefault();

            return enumMemberAttribute?.Value ?? name; // Se não tiver atributo, usa o nome do membro
        }

        // Converte uma string de volta para o Enum (procurando pelo atributo)
        private static TEnum ToEnum<TEnum>(this string value) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = enumType.GetField(name)?
                    .GetCustomAttributes(typeof(EnumMemberAttribute), true)
                    .Cast<EnumMemberAttribute>()
                    .FirstOrDefault();

                if (enumMemberAttribute?.Value == value)
                {
                    return (TEnum)Enum.Parse(enumType, name);
                }
            }

            // Fallback para o nome do membro se não encontrar no atributo
            if (Enum.TryParse<TEnum>(value, true, out var result))
                return result;

            return default;
        }
    }
}

```
---


- `SeedDataCliente` (refatorar: considerar o uso "ou não" de `using` e `ServiceProvider`):

```csharp

using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class SeedDataCliente
    {
        // Função para retornar o tipo de cliente baseado em pesos
        public static TipoDeCliente GetTipoDeClienteByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDeCliente, int>
            {
                { TipoDeCliente.Especial, 9 },
                { TipoDeCliente.Comum, 32 },
                { TipoDeCliente.Inadimplente, 4 }
            };
            return GetRandomEnumByWeight(pesos, random, TipoDeCliente.Comum);
        }

        // Função para retornar o status do cliente baseado em pesos (ATUALIZADA)
        public static StatusDoCliente GetStatusDoClienteByWeight(Random random)
        {
            var pesos = new Dictionary<StatusDoCliente, int>
            {
                { StatusDoCliente.Ativo, 30 },
                { StatusDoCliente.Inativo, 8 },
                { StatusDoCliente.Bloqueado, 5 }
            };
            return GetRandomEnumByWeight(pesos, random, StatusDoCliente.Ativo);
        }

        // Função para retornar o tipo de pessoa baseado em pesos
        public static TipoDePessoa GetTipoDePessoaByWeight(Random random)
        {
            var pesos = new Dictionary<TipoDePessoa, int>
            {
                { TipoDePessoa.Fisica, 5 },
                { TipoDePessoa.Juridica, 25 }
            };
            return GetRandomEnumByWeight(pesos, random, TipoDePessoa.Fisica);
        }

        // Função para retornar o regime tributário baseado em pesos
        public static RegimeTributario GetRegimeTributarioByWeight(Random random)
        {
            var pesos = new Dictionary<RegimeTributario, int>
            {
                { RegimeTributario.SimplesNacional, 8 },
                { RegimeTributario.LucroPresumido, 12 },
                { RegimeTributario.LucroReal, 20 },
                { RegimeTributario.Outro, 2 }
            };
            return GetRandomEnumByWeight(pesos, random, RegimeTributario.SimplesNacional);
        }

        // Função genérica para obter um enum aleatório baseado em pesos
        private static TEnum GetRandomEnumByWeight<TEnum>(Dictionary<TEnum, int> weightedEnums, Random random, TEnum fallback) where TEnum : Enum
        {
            int pesoTotal = weightedEnums.Values.Sum();
            if (pesoTotal == 0)
                return fallback; // Evita divisão por zero se todos os pesos forem 0

            int randomValue = random.Next(0, pesoTotal);
            int acumulado = 0;
            foreach (var entry in weightedEnums)
            {
                acumulado += entry.Value;
                if (randomValue < acumulado)
                {
                    return entry.Key;
                }
            }
            return fallback; // Fallback (segurança)
        }


        public static string GerarDocumento(TipoDePessoa tipoPessoa, Random random)
        {
            if (tipoPessoa == TipoDePessoa.Fisica)
            {
                return string.Join("", Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString()));
            } else
            {
                return string.Join("", Enumerable.Range(0, 14).Select(_ => random.Next(0, 10).ToString()));
            }
        }

        public static string GerarEmail(string nomeCliente, Random random)
        {
            var nomeTratado = nomeCliente.ToLower().Replace(" ", ".").Replace(",", "").Replace("Ltda", "").Replace("Inc", "").Replace("SA", "").Replace("Ass", "");
            var dominios = new List<string> { "empresa.com", "cliente.com", "mail.com", "exemplo.com", "negocios.com", "provedor.net" };
            var dominioAleatorio = dominios [random.Next(dominios.Count)];
            return $"{nomeTratado.Split('.').First()}{random.Next(1, 99)}@{dominioAleatorio}"; // Adiciona um número para mais unicidade
        }
        
        public static string GerarTelefonePrincipal(Random random)
        {
            return $"({random.Next(11, 99)}) 9{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        }

        // NOVA Função para gerar contato/representante (pode ser null/vazio)
        public static string? GerarContatoRepresentante(string nomeCliente, Random random)
        {
            if (random.Next(0, 3) == 0) // 1 em 3 chance de não ter representante
            {
                return null;
            }
            var nomesRepresentante = new List<string> { "Sr. Silva", "Sra. Oliveira", "Gerente " + nomeCliente.Split(' ') [0], "Contato Vendas", "Financeiro" };
            return nomesRepresentante [random.Next(nomesRepresentante.Count)];
        }


        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                if (context == null || context.Cliente == null || context.Pessoa == null)
                {
                    throw new ArgumentNullException("Null AppDbContext or required DbSets");
                }

                if (context.Cliente.Any())
                {
                    Console.WriteLine("O SeedData para Cliente já foi gerado!");
                    return;
                }

                Console.WriteLine("Gerando SeedData para Cliente...");
                var random = new Random();

                var clientesData = new List<string> // Simplificado para apenas nomes, email será gerado
                {
                    "Ana Paula Silva", "Empresa X Soluções Ltda", "Carlos Eduardo Moreira", "Construtora ABC Obras",
                    "Arnaldo Santiago Filho", "Clarice Borges de Almeida", "ABC Med Diagnósticos Inc", "Start Cor Tintas SA",
                    "Medical Group Assistência Ltda", "Ricardo Amaral Consultoria", "Empório Leonardo & Associados",
                    "Cooperchip CPES Tecnologia Ltda", "Ana Beatriz Souza e Silva", "Lumac Labs Pesquisas SA", "Ana Clara Vasconcelos",
                    "Mariana Costa Fotografia", "Tech Global Inovações Ltda", "Oliveira & Filhos Comércio", "Ferreira Advogados Associados", "Bar e Restaurante Sabor Local"
                };

                foreach (var nomeCliente in clientesData)
                {
                    var tipoPessoa = GetTipoDePessoaByWeight(random);
                    var documento = GerarDocumento(tipoPessoa, random);
                    var email = GerarEmail(nomeCliente, random);

                    var cliente = new Cliente(
                        nome: nomeCliente,
                        documento: documento,
                        tipoDePessoa: tipoPessoa,
                        email: email
                    )
                    {
                        TipoDeCliente = GetTipoDeClienteByWeight(random),
                        StatusDoCliente = GetStatusDoClienteByWeight(random),
                        DataInclusao = DateTime.UtcNow.AddDays(-10),
                        UsuarioInclusao = "SeedData - Inclusão",
                        DataUltimaModificacao = DateTime.UtcNow.AddDays(-2),
                        UsuarioUltimaModificacao = "SeedData - Modificação",
                        InscricaoEstadual = random.Next(0, 2) == 0 ? null : "1234567890", // 50% de chance de ser null
                        Observacao = "Observação padrão para SeedData",
                    };


                    // Usando os métodos Set para as propriedades com setters privados
                    cliente.SetTelefonePrincipal(GerarTelefonePrincipal(random));

                    var contatoRep = GerarContatoRepresentante(nomeCliente, random);
                    if (!string.IsNullOrEmpty(contatoRep))
                    {
                        cliente.SetContatoRepresentante(contatoRep);
                    }
                    // Se ContatoRepresentante for null, ele permanecerá null na entidade, o que é ok.

                    context.Cliente.Add(cliente);
                }

                try
                {
                    context.SaveChanges(); // Mantendo o SaveChanges aqui por enquanto
                    Console.WriteLine("SeedData para Cliente gerado com sucesso!");
                } catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao salvar SeedData de Cliente: {ex.Message}");
                    // Considerar logar ex.ToString() para mais detalhes em caso de falha
                }
            }
        }
    }
}

```
---


- `DbInitializer` (refatorar - encontrar uma solução elegante para tratar a transação):

```csharp

// ... (outros using)
using GeneralLabSolutions.InfraStructure.Data.ORM;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralLabSolutions.InfraStructure.Data.Seeds
{
    public static class DbInitializer
    {
        /// <summary>
        /// Classe que unifica a criação dos SeedDatas da Solução
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            // ToDo: Vou manter como está, por enquanto.

            using (var scope = serviceProvider.CreateScope())
            {
                var scopedProvider = scope.ServiceProvider;
                var context = scopedProvider.GetRequiredService<AppDbContext>();

                // Usar uma única transação para todos os SeedData (refatorar)
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Chamar os SeedData na ordem correta
                        SeedDataCategoriaProduto.Initialize(scopedProvider);
                        SeedDataFornecedor.Initialize(scopedProvider);
                        SeedDataCliente.Initialize(scopedProvider);
                        SeedDataVendedor.Initialize(scopedProvider);
                        SeedDataProduto.Initialize(scopedProvider);
                        SeedDataStatusDoItem.Initialize(scopedProvider);
                        SeedDataStatusDoItemIncompativel.Initialize(scopedProvider);
                        SeedDataPedido.Initialize(scopedProvider);


                        // --- INÍCIO DA ADIÇÃO ---
                        SeedDataCrm.Initialize(context); 
                        // Adicionando nosso novo seeder de CRM
                        // --- FIM DA ADIÇÃO ---


                        SeedDataParticipante.Initialize(scopedProvider); // ANTES de KanbanTask
                        SeedDataKanbanTask.Initialize(scopedProvider);



                        // ToDo: Refatorar os SeedDatas acima, para usar o contexto ativo, salvando tudo em uma única Trasaction;

                        // (depois dos outros seeds que criam Pessoa)
                        SeedDataContato.Initialize(context);
                        SeedDataTelefone.Initialize(context);
                        SeedDataEndereco.Initialize(context);
                        SeedDataDadosBancarios.Initialize(context);
                        // ------------------------------------- //

                        SeedDataCalendarEvent.Initialize(context);

                        // Se chegou até aqui sem erros, commita a transação
                        //Salvar todas as alterações na mesma transação.
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        Console.WriteLine("\n\n================================\nSeedData executado com sucesso!");
                    } catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Erro durante a execução do SeedData: {ex.Message}");
                        // Aqui podemos logar o erro, lançar a exceção novamente, etc.
                        throw;
                    }
                }
            }
        }
    }
}

```
---


- `DependencyInjectionConfig`:

```csharp

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


```
---

- `CustomHttpRequestException`:

```csharp

using System.Net;

namespace VelzonModerna.Configuration.Extensions
{
    public class CustomHttpRequestException : Exception
    {
        public HttpStatusCode StatusCode;

        public CustomHttpRequestException() { }

        public CustomHttpRequestException(string message, Exception innerException)
            : base(message, innerException) { }

        public CustomHttpRequestException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}


```
---


- Sei que estou te enviando mais código do que o necessário, mas quero que conheça o máximo da minha Solution, que seja relevante para o meu propósito. Algumas outras features serão importantes no futuro.

Já volto. Me aguarde, por favor!