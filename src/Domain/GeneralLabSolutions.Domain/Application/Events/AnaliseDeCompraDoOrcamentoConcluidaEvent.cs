using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // AnaliseDeCompraDoOrcamentoConcluidaEvent.cs
    public class AnaliseDeCompraDoOrcamentoConcluidaEvent : DomainEvent
    {
        public AnaliseDeCompraDoOrcamentoConcluidaEvent(Guid orcamentoId) : base(orcamentoId) { }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}
