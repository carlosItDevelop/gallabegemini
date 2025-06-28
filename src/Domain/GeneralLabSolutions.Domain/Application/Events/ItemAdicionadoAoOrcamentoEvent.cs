using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // ItemAdicionadoAoOrcamentoEvent.cs
    public class ItemAdicionadoAoOrcamentoEvent : DomainEvent
    {
        public Guid ProdutoId { get; }
        public int Quantidade { get; }
        public decimal ValorUnitario { get; }
        public ItemAdicionadoAoOrcamentoEvent(Guid orcamentoId, Guid produtoId, int quantidade, decimal valorUnitario) : base(orcamentoId)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}
