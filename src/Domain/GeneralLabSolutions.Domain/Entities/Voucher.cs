using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Voucher : EntityBase
    {
        public Voucher(string? codigo, decimal? percentual, decimal? valorDesconto, int quantidade, TipoDescontoVoucher tipoDescontoVoucher, DateTime dataCriacao, DateTime? dataUtilizacao, DateTime dataValidade, bool ativo, bool utilizado)
        {
            Codigo = codigo;
            Percentual = percentual;
            ValorDesconto = valorDesconto;
            Quantidade = quantidade;
            TipoDescontoVoucher = tipoDescontoVoucher;
            DataCriacao = dataCriacao;
            DataUtilizacao = dataUtilizacao;
            DataValidade = dataValidade;
            Ativo = ativo;
            Utilizado = utilizado;
        }

        public string? Codigo { get; private set; }
        public decimal? Percentual { get; private set; }
        public decimal? ValorDesconto { get; private set; }
        public int Quantidade { get; private set; }
        public TipoDescontoVoucher TipoDescontoVoucher { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataUtilizacao { get; private set; }
        public DateTime DataValidade { get; private set; }
        public bool Ativo { get; private set; }
        public bool Utilizado { get; private set; }

        // EF Relationship
        public ICollection<Pedido> Pedidos { get; set; }
               = new List<Pedido>();
    }
}
