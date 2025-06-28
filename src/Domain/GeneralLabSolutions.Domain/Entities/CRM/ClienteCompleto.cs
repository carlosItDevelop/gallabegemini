//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GeneralLabSolutions.Domain.DomainObjects;
//using GeneralLabSolutions.Domain.Entities.Base;

//namespace GeneralLabSolutions.Domain.Entities.CRM;

///// <summary>
///// Cliente / Fornecedor  (CRM‑02)
///// </summary>
//public class ClienteCompleto : EntityBase, IAggregateRoot
//{
//    /* ----------  CTOR protegido p/ EF  ---------- */
//    protected ClienteCompleto() { }

//    public ClienteCompleto(
//        string razaoSocial,
//        string nomeFantasia,
//        string documento,
//        RegimeTributario regimeTributario,
//        string inscricaoEstadual,
//        SegmentoCliente segmento)
//    {
//        Id = Guid.NewGuid();
//        RazaoSocial = razaoSocial ?? throw new DomainException("Razão Social obrigatória.");
//        NomeFantasia = nomeFantasia;
//        Documento = documento;          // Validação do dígito feita no handler/VO
//        RegimeTributario = regimeTributario;
//        InscricaoEstadual = inscricaoEstadual; // Aplicável se Cliente for do Brasil
//        Segmento = segmento;
//        Status = StatusCliente.Ativo;
//        DataCadastro = DateTime.UtcNow;
//    }

//    /* ----------  Identificação  ---------- */
//    [Required, MaxLength(150)]
//    public string RazaoSocial { get; private set; }

//    [Required, MaxLength(150)]
//    public string NomeFantasia { get; private set; }

//    [Required, MaxLength(14)]
//    public string Documento { get; private set; }      // CNPJ ou CPF, conforme card CRM‑02

//    [MaxLength(14)]
//    public string InscricaoEstadual { get; private set; }   // obrigatório se empresa não for isenta:contentReference[oaicite:1]{index=1}:contentReference[oaicite:2]{index=2}

//    public RegimeTributario RegimeTributario { get; private set; }  // Enum abaixo

//    public SegmentoCliente Segmento { get; private set; }           // Campo CRM‑08:contentReference[oaicite:3]{index=3}:contentReference[oaicite:4]{index=4}

//    /* ----------  Contato e comunicação  ---------- */
//    [MaxLength(20)]
//    public string? TelefonePrincipal { get; private set; }

//    [EmailAddress, MaxLength(150)]
//    public string? EmailFinanceiro { get; private set; }

//    /* ----------  Status & Auditoria  ---------- */
//    public StatusCliente Status { get; private set; }

//    public DateTime DataCadastro { get; private set; }
//    public DateTime UltimaAlteracao { get; private set; }
//    public Guid? UsuarioUltAlteracaoId { get; private set; }

//    /* ----------  Extras (Should‑Have fáceis de incluir)  ---------- */
//    public string? ObservacoesInternas { get; private set; }  // CRM‑18:contentReference[oaicite:5]{index=5}:contentReference[oaicite:6]{index=6}

//    /* ----------  Navegações  ---------- */
//    private readonly List<UnidadeCliente> _unidades = new();
//    public IReadOnlyCollection<UnidadeCliente> Unidades => _unidades.AsReadOnly();   // CRM‑09 (endereços múltiplos):contentReference[oaicite:7]{index=7}:contentReference[oaicite:8]{index=8}

//    private readonly List<ContatoCliente> _contatos = new();
//    public IReadOnlyCollection<ContatoCliente> Contatos => _contatos.AsReadOnly();

//    /* ----------  Comportamentos essenciais  ---------- */
//    public void Inativar(Guid usuarioId)
//    {
//        Status = StatusCliente.Inativo;
//        UsuarioUltAlteracaoId = usuarioId;
//        UltimaAlteracao = DateTime.UtcNow;
//        // DomainEvents.Add(new ClienteInativadoEvent(Id));
//    }

//    public void Bloquear(Guid usuarioId)
//    {
//        Status = StatusCliente.Bloqueado;
//        UsuarioUltAlteracaoId = usuarioId;
//        UltimaAlteracao = DateTime.UtcNow;
//    }

//    // …Outros métodos de mudança de estado/atualização (ChangeRazaoSocial, etc.)
//}

///* =========  Enums essenciais ========= */

//public enum RegimeTributario   // CRM‑14 campos fiscais adicionais:contentReference[oaicite:9]{index=9}
//{
//    SimplesNacional = 1,
//    LucroPresumido = 2,
//    LucroReal = 3,
//    Outro = 99
//}

//public enum StatusCliente
//{
//    Ativo = 1,
//    Inativo = 2,
//    Bloqueado = 3        // usado pelo FIN‑16 (bloqueio de faturamento)
//}

//public enum SegmentoCliente      // Valor proveniente do card CRM‑08
//{
//    Saude = 1,
//    Pesquisa = 2,
//    Industria = 3,
//    // …carregar a lista inicial na seed
//}


