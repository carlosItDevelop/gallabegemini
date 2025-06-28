using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Mensageria;

namespace GeneralLabSolutions.Domain.Application.Events
{
    // OrcamentoCriadoEvent.cs
    public class OrcamentoCriadoEvent : DomainEvent
    {
        public Guid VendedorId { get; }
        public string? NomeClientePotencial { get; }
        public DateTime DataCriacao { get; }
        public OrcamentoCriadoEvent(Guid orcamentoId, Guid vendedorId, string? nomeClientePotencial, DateTime dataCriacao) : base(orcamentoId)
        {
            VendedorId = vendedorId;
            NomeClientePotencial = nomeClientePotencial;
            DataCriacao = dataCriacao;
        }
    }

    // OrcamentoConvertidoEmPedidoVendaEvent.cs (a ser implementado quando tivermos o Pedido de Venda)
    // OrcamentoCanceladoEvent.cs
}
