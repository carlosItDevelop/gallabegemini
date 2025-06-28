using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // OrcamentoEnviadoParaAnaliseEvent.cs
    public class OrcamentoEnviadoParaAnaliseEvent : DomainEvent
    {
        public OrcamentoEnviadoParaAnaliseEvent(Guid orcamentoId) : base(orcamentoId) { }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}
