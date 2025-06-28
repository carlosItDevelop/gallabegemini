using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums; // Certifique-se de ter este using

namespace GeneralLabSolutions.Domain.Entities
{
    public class Conta : EntityBase
    {
        // Construtor principal (domínio) - MELHORADO
        public Conta(
            string instituicao,
            string documento,
            DateTime dataVencimento,
            decimal valor,
            TipoDeConta tipoDeConta,
            string? observacao = null)
        {
            // Validações *NO CONSTRUTOR* (Domain-Driven Design)
            if (string.IsNullOrWhiteSpace(instituicao))
                throw new ArgumentException("Instituição deve ser informada.", nameof(instituicao));

            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("Documento deve ser informado.", nameof(documento));

            if (dataVencimento <= DateTime.Now) //Já validando se a data de vencimento é válida!
                throw new ArgumentException("Data de vencimento deve ser futura.", nameof(dataVencimento));

            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero.", nameof(valor));


            Instituicao = instituicao;
            Documento = documento;
            DataVencimento = dataVencimento;
            Valor = valor;
            TipoDeConta = tipoDeConta;
            Observacao = observacao;

            Inativa = false;
            EstaPaga = false;
        }

        // Construtor protegido (EF Core) - OK
        public Conta() { }

        public string Instituicao { get; private set; }
        public string Documento { get; private set; }
        public DateTime DataVencimento { get; private set; }
        public decimal Valor { get; private set; }
        public TipoDeConta TipoDeConta { get; private set; }
        public bool EstaPaga { get; private set; }
        public DateTime? DataPagamento { get; private set; }
        public string? Observacao { get; private set; }
        public bool Inativa { get; private set; }

        // Métodos de domínio - REFINADOS
        public void MarcarComoPaga()
        {
            if (Inativa)
                throw new InvalidOperationException("Conta inativa não pode ser paga."); //Melhor usar Exception!

            if (EstaPaga)
                return; // Idempotência

            EstaPaga = true;
            DataPagamento = DateTime.Now;

            // Adiciona Evento de Dominio!
            AdicionarEvento(new ContaPagaEvent(Id));
        }

        public void MarcarComoNaoPaga()
        {
            if (Inativa)
                throw new InvalidOperationException("Conta inativa não pode ser estornada.");

            if (!EstaPaga)
                return; // Idempotência

            EstaPaga = false;
            DataPagamento = null;

            // Adiciona Evento de Dominio!
            AdicionarEvento(new ContaEstornadaEvent(Id));
        }

        public void InativarConta()
        {
            if (Inativa)
                return; // Idempotência
            Inativa = true;
            // AdicionarEvento(new ContaInativadaEvent(Id)); //Consistência!
        }

        public void AlterarDataVencimento(DateTime novaData)
        {
            if (novaData <= DateTime.Now)
                throw new ArgumentException("Nova data de vencimento deve ser futura.", nameof(novaData));

            DataVencimento = novaData;
            //AdicionarEvento(new ContaAlteradaEvent(Id)); //Se precisar de rastreabilidade
        }


        //Validação da Entidade (No seu contexto, usaremos FluentValidation no Service)
        public override bool EhValido() => true; //A validação será feita no Service (ContaService)

    }
}