### Features da Solução GeneralLabSolutions - Parte 2


- Mais enums próprios de Orçamento e Pedido de Compra e Venda:

- `StatusDoPedido`:

```csharp

using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusDoPedido
    {
        [Description("Orçamento do Vendedor")] Orcamento, // Peso: 10 - maior que cancelado
        [Description("Em Processamento")] EmProcessamento, // Peso: 20 - ainda maior
        [Description("Pago")] Pago, // Peso: 30 - mais alto (prioritário)
        [Description("Enviado")] Enviado, // Peso: 10 - médio
        [Description("Entregue")] Entregue, // Peso: 20 - grande
        [Description("Cancelado")] Cancelado // Peso: 5 - mínimo (raridade)
    }
}

```
---


- `StatusItemOrcamento`:

```csharp

using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusItemOrcamento
    {
        [Description("Pendente de Análise")] // Item adicionado, aguardando análise de compra
        PendenteAnalise = 1,

        [Description("Aprovado para Compra")] // Financeiro/Compras aprovou este item para ser comprado do fornecedor
        AprovadoParaCompra = 2,

        [Description("Compra em Andamento")] // Um PedidoDeCompra foi gerado para este item
        CompraEmAndamento = 3,

        [Description("Compra Concluída")] // O item foi totalmente recebido do fornecedor
        CompraConcluida = 4,

        [Description("Rejeitado para Compra")] // Financeiro/Compras não aprovou a compra deste item
        RejeitadoParaCompra = 5,

        [Description("Cancelado")] // Item cancelado do orçamento
        Cancelado = 6
    }
}


```
---


- `StatusItemPedidoCompra`:

```csharp

using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusItemPedidoCompra
    {
        [Description("Pendente de Envio")] // Item aguardando envio do fornecedor
        PendenteDeEnvio = 1,

        [Description("Enviado pelo Fornecedor")]
        EnviadoPeloFornecedor = 2,

        [Description("Em Trânsito")]
        EmTransito = 3,

        [Description("Recebido")] // Item recebido conforme esperado
        Recebido = 4,

        [Description("Recebido com Divergência")] // Item recebido, mas com problemas (quantidade, qualidade)
        RecebidoComDivergencia = 5,

        [Description("Cancelado")] // Item cancelado do pedido de compra
        CanceladoPeloComprador = 6,

        [Description("Cancelado pelo Fornecedor")]
        CanceladoPeloFornecedor = 7
    }
}

```
---


- `StatusOrcamento`:

```csharp

using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusOrcamento
    {
        [Description("Em Elaboração")] // Vendedor está montando
        EmElaboracao = 1,

        [Description("Aguardando Análise de Compra")] // Vendedor finalizou, aguarda financeiro/compras
        AguardandoAnaliseCompra = 2,

        [Description("Análise de Compra Concluída")] // Financeiro/compras analisou todos os itens
        AnaliseCompraConcluida = 3, // Pode ter itens aprovados, rejeitados ou pendentes de decisão final

        [Description("Convertido em Pedido de Venda")] // Orçamento virou um pedido de venda para o cliente
        ConvertidoEmPedidoVenda = 4,

        [Description("Cancelado")]
        Cancelado = 5,

        [Description("Expirado")]
        Expirado = 6
    }
}




```
---


- `StatusPedidoCompra`:

```csharp

using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusPedidoCompra
    {
        [Description("Em Elaboração")]      // Pedido sendo montado, itens sendo adicionados
        EmElaboracao = 1,

        [Description("Aguardando Aprovação Interna")] // Submetido para aprovação interna (se houver esse fluxo)
        AguardandoAprovacaoInterna = 2,

        [Description("Aprovado / Pronto para Envio")] // Aprovado internamente, pronto para ser enviado ao fornecedor
        AprovadoProntoParaEnvio = 3,

        [Description("Enviado ao Fornecedor")]  // Formalmente enviado
        EnviadoAoFornecedor = 4,

        [Description("Confirmado pelo Fornecedor")] // Fornecedor confirmou o recebimento e aceite do pedido
        ConfirmadoPeloFornecedor = 5,

        [Description("Em Produção / Separação")] // Fornecedor está processando
        EmProducaoOuSeparacao = 6,

        [Description("Parcialmente Enviado pelo Fornecedor")] // Fornecedor enviou parte dos itens
        ParcialmenteEnviadoPeloFornecedor = 7,

        [Description("Totalmente Enviado pelo Fornecedor")] // Fornecedor enviou todos os itens
        TotalmenteEnviadoPeloFornecedor = 8,

        [Description("Em Trânsito")]          // Mercadoria a caminho
        EmTransito = 9,

        [Description("Recebido Parcialmente")] // Sua empresa recebeu parte dos itens
        RecebidoParcialmente = 10,

        [Description("Recebido Totalmente / Concluído")] // Sua empresa recebeu todos os itens, pedido finalizado
        RecebidoTotalmenteConcluido = 11,

        [Description("Cancelado")]
        Cancelado = 12,

        [Description("Com Pendência")] // Ex: problema no recebimento, aguardando resolução
        ComPendencia = 13
    }
}

```
---


- `LowerCaseNamingPolicy`:

```csharp

using System.Text.Json;

namespace GeneralLabSolutions.Domain.Extensions
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToLower();
        }
    }

}


```
---


- `EntityAudit` (e suas interfaces - base das entidades auditáveis):


```csharp

using GeneralLabSolutions.Domain.Entities.Base;

namespace GeneralLabSolutions.Domain.Entities.Audit
{

    public interface IAuditable : IAuditableAdd, IAuditableUpd
    {
    }

    public interface IAuditableAdd
    {
        DateTime? DataInclusao { get; set; }
        string UsuarioInclusao { get; set; }
    }

    public interface IAuditableUpd
    {
        DateTime? DataUltimaModificacao { get; set; }
        string UsuarioUltimaModificacao { get; set; }
    }


    public abstract class EntityAudit : EntityBase, IAuditable
    {
        public EntityAudit() { }

        public DateTime? DataInclusao { get; set; }
        public string UsuarioInclusao { get; set; }
        public DateTime? DataUltimaModificacao { get; set; }
        public string UsuarioUltimaModificacao { get; set; }
    }
}


```
---

- `EntityBase` (classe base de todas as entidades):

```csharp

using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
            Id = Guid.NewGuid();
        }


        #region: Notifications and maintenance of the Events

        private List<Event> _notificacoes;
        public IReadOnlyCollection<Event>? Notificacoes => _notificacoes?.AsReadOnly();

        public void AdicionarEvento(Event evento)
        {
            _notificacoes = _notificacoes ?? new List<Event>();
            _notificacoes.Add(evento);
        }

        public void RemoverEvento(Event eventItem) => _notificacoes?.Remove(eventItem);
        public void LimparEventos() => _notificacoes?.Clear();

        #endregion


        #region: Comparações, Validação, ToString
        public virtual bool EhValido() => throw new NotImplementedException();

        public Guid Id { get; set; }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EntityBase;

            if (ReferenceEquals(this, compareTo))
                return true;
            if (ReferenceEquals(null, compareTo))
                return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(EntityBase a, EntityBase b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }
        public static bool operator !=(EntityBase a, EntityBase b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }
        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }
        #endregion
    }
}


```
---

- `ValidacaoDocs.cs` (arquivo com validações de Cpf e Cnpj):


```csharp

using GeneralLabSolutions.Domain.Extensions.Extending;

namespace GeneralLabSolutions.Domain.Entities.Base
{
    public class CpfValidacao
    {
        public const int TamanhoCpf = 11;

        public static bool Validar(string cpf)
        {
            var cpfNumeros = cpf.ApenasNumeros();

            if (!TamanhoValido(cpfNumeros))
                return false;
            return !TemDigitosRepetidos(cpfNumeros) && TemDigitosValidos(cpfNumeros);
        }

        private static bool TamanhoValido(string valor)
        {
            return valor.Length == TamanhoCpf;
        }

        private static bool TemDigitosRepetidos(string valor)
        {
            string [] invalidNumbers =
            {
                "00000000000",
                "11111111111",
                "22222222222",
                "33333333333",
                "44444444444",
                "55555555555",
                "66666666666",
                "77777777777",
                "88888888888",
                "99999999999"
            };
            return invalidNumbers.Contains(valor);
        }

        private static bool TemDigitosValidos(string valor)
        {
            var number = valor.Substring(0, TamanhoCpf - 2);
            var digitoVerificador = new DigitoVerificador(number)
                .ComMultiplicadoresDeAte(2, 11)
                .Substituindo("0", 10, 11);
            var firstDigit = digitoVerificador.CalculaDigito();
            digitoVerificador.AddDigito(firstDigit);
            var secondDigit = digitoVerificador.CalculaDigito();

            return string.Concat(firstDigit, secondDigit) == valor.Substring(TamanhoCpf - 2, 2);
        }
    }

    public class CnpjValidacao
    {
        public const int TamanhoCnpj = 14;

        public static bool Validar(string cnpj)
        {
            var cnpjNumeros = cnpj.ApenasNumeros();

            if (!TemTamanhoValido(cnpjNumeros))
                return false;
            return !TemDigitosRepetidos(cnpjNumeros) && TemDigitosValidos(cnpjNumeros);
        }

        private static bool TemTamanhoValido(string valor)
        {
            return valor.Length == TamanhoCnpj;
        }

        private static bool TemDigitosRepetidos(string valor)
        {
            string [] invalidNumbers =
            {
                "00000000000000",
                "11111111111111",
                "22222222222222",
                "33333333333333",
                "44444444444444",
                "55555555555555",
                "66666666666666",
                "77777777777777",
                "88888888888888",
                "99999999999999"
            };
            return invalidNumbers.Contains(valor);
        }

        private static bool TemDigitosValidos(string valor)
        {
            var number = valor.Substring(0, TamanhoCnpj - 2);

            var digitoVerificador = new DigitoVerificador(number)
                .ComMultiplicadoresDeAte(2, 9)
                .Substituindo("0", 10, 11);
            var firstDigit = digitoVerificador.CalculaDigito();
            digitoVerificador.AddDigito(firstDigit);
            var secondDigit = digitoVerificador.CalculaDigito();

            return string.Concat(firstDigit, secondDigit) == valor.Substring(TamanhoCnpj - 2, 2);
        }
    }

    public class DigitoVerificador
    {
        private string _numero;
        private const int Modulo = 11;
        private readonly List<int> _multiplicadores = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IDictionary<int, string> _substituicoes = new Dictionary<int, string>();
        private bool _complementarDoModulo = true;

        public DigitoVerificador(string numero)
        {
            _numero = numero;
        }

        public DigitoVerificador ComMultiplicadoresDeAte(int primeiroMultiplicador, int ultimoMultiplicador)
        {
            _multiplicadores.Clear();
            for (var i = primeiroMultiplicador; i <= ultimoMultiplicador; i++)
                _multiplicadores.Add(i);

            return this;
        }

        public DigitoVerificador Substituindo(string substituto, params int [] digitos)
        {
            foreach (var i in digitos)
            {
                _substituicoes [i] = substituto;
            }
            return this;
        }

        public void AddDigito(string digito)
        {
            _numero = string.Concat(_numero, digito);
        }

        public string CalculaDigito()
        {
            return !(_numero.Length > 0) ? "" : GetDigitSum();
        }

        private string GetDigitSum()
        {
            var soma = 0;
            for (int i = _numero.Length - 1, m = 0; i >= 0; i--)
            {
                var produto = (int)char.GetNumericValue(_numero [i]) * _multiplicadores [m];
                soma += produto;

                if (++m >= _multiplicadores.Count)
                    m = 0;
            }

            var mod = soma % Modulo;
            var resultado = _complementarDoModulo ? Modulo - mod : mod;

            return _substituicoes.ContainsKey(resultado) ? _substituicoes [resultado] : resultado.ToString();
        }
    }


}


```
---


- `ClienteRegistradoEvent`:


```csharp

using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    public class ClienteRegistradoEvent : DomainEvent
    {
        public ClienteRegistradoEvent(Guid aggregateId, string nome, string documento, TipoDePessoa tipoDePessoa, string email) : base(aggregateId)
        {
            Id = aggregateId;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;
        }

        public Guid Id { get; private set; }

        public string Nome { get; private set; }

        public string Documento { get; private set; }
        public string Email { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }
    }
}


```
---

- `ClienteRegistradoEventHandler`:


```csharp

using GeneralLabSolutions.Domain.Application.Events;
using MediatR;

namespace GeneralLabSolutions.Domain.Application.Handlers
{
    public class ClienteRegistradoEventHandler : INotificationHandler<ClienteRegistradoEvent>
    {
        public async Task Handle(ClienteRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // Enviar evento de confirmação;
            // Enviar emails para os interessados;
            // Fazer outras coisas relevantes;

            Console.WriteLine($"Cliente ID: {notification.Id}, Nome: {notification.Nome} registrado!");

            await Task.CompletedTask;
        }

    }
}


```
---


- Me aguarde, pois trarei mais códigos. Pode atualizar nosso Mapa da Solução em nosso App `Painel de Bordo`.