using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.SharedKernel.Enums;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // ItemOrcamentoStatusAlteradoEvent.cs
    public class ItemOrcamentoStatusAlteradoEvent : DomainEvent // Para AprovadoParaCompra e RejeitadoParaCompra
    {
        public Guid ItemOrcamentoId { get; }
        public StatusItemOrcamento NovoStatus { get; }
        public Guid ResponsavelAnaliseId { get; }
        public string? MotivoRejeicao { get; } // Opcional

        public ItemOrcamentoStatusAlteradoEvent(Guid orcamentoId, Guid itemOrcamentoId, StatusItemOrcamento novoStatus, Guid responsavelAnaliseId, string? motivoRejeicao = null)
            : base(orcamentoId)
        {
            ItemOrcamentoId = itemOrcamentoId;
            NovoStatus = novoStatus;
            ResponsavelAnaliseId = responsavelAnaliseId;
            MotivoRejeicao = motivoRejeicao;
        }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}
