
namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    public class ClienteConsolidadoDto
    {
        public Guid ClienteId { get; set; }

        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Total de Pedidos deste Cliente;
        /// Observação: (Pedido deve ser != Cancelado)
        /// </summary>
        public int QuantidadeDePedidos { get; set; } = 0;

        /// <summary>
        /// Me interessa saber qual o Ticket Médio deste Cliente,
        /// considerando o total que ele comprou em qualquer período
        /// dividindo-o pelo número de Pedidos do Cliente;
        /// - Observação: Só valem para Clientes (Ativos) e Pedidos != de Cancelados;
        /// </summary>
        public decimal TicketMedioPorPedido { get; set; } = decimal.Zero;

        /// <summary>
        /// Me interessa saber qual o Ticket Médio deste Cliente,
        /// considerando o total que ele comprou (num determinado período), 
        /// dividindo-o pelo número de (Período) que especificarmos;
        /// - Observação: Só valem para Clientes (Ativos) e Pedidos != de Cancelados;
        /// </summary>
        public decimal TicketMedioPorPeriodo { get; set; } = decimal.Zero;

        /// <summary>
        /// Data da última compra deste Cliente;
        /// </summary>
        public DateTime? UltimaCompraDesteCliente { get; set; }

        /// <summary>
        /// Quantidade de Compras por Categoria;
        /// </summary>
        public int QuantidadeDeComprasPorCategoria { get; set; } = 0;

        /// <summary>
        /// Categoria mais comprada por este Cliente;
        /// </summary>
        public CategoriaProdutoDto? CategoriaMaisComprada { get; set; }

        /// <summary>
        /// Quantidade de compras por Produto,
        /// desde que estes Produtos estejam Ativos;
        /// </summary>
        public int QuantidadeDeComprasPorProduto { get; set; } = 0;

        /// <summary>
        /// Produto mais comprado por este Cliente;
        /// </summary>
        public ProdutoDto? ProdutoMaisComprado { get; set; }

        /// <summary>
        /// Posição do Cliente em relação a todos os outros;
        /// Observação: Apenas Pedidos != Cancelados;
        /// </summary>
        public int RankingPorValorEmPedidosFinalizados { get; set; }

        /// <summary>
        /// Posição do Cliente em relação a todos os outros;
        /// Observação: Apenas Pedidos != Cancelados;
        /// </summary>
        public int RankingPorQuantidadePorPedidosFinalizados { get; set; }

        /// <summary>
        /// Percentual de Conversão entre Orçamentos e Pedidos EmProcessamento (ou Pagos, Finalizados);
        /// Todo: Saber se ele quer guardar os históricos de Orçamentos, convertidos em Vendas ou Não;
        /// </summary>
        public int PercentualDeConversao { get; set; }

        /// <summary>
        /// Pegar a Data da Primeira e Última compra, traçar uma média,
        /// mas é importante que ele não tenha ficado muito tempo sem comprar;
        /// Isso gerará uma Observação no Consolidado:
        /// </summary>
        public int IntervaloMedioEntrePedidos { get; set; }

        // Novas Propriedades

        /// <summary>
        /// Valor Total de Compras deste Cliente;
        /// Considera o somatório dos valores de todos os pedidos finalizados (exceto cancelados).
        /// </summary>
        public decimal ValorTotalDeCompras { get; set; } = decimal.Zero;

        /// <summary>
        /// Frequência Média de Compras;
        /// Calcula quantos dias, em média, o cliente demora para fazer uma nova compra.
        /// </summary>
        public double FrequenciaDeComprasMedia { get; set; }

        /// <summary>
        /// Percentual de Desconto Médio oferecido a este Cliente;
        /// Calculado com base nos descontos aplicados nos pedidos do cliente.
        /// </summary>
        public double PercentualDeDescontoMedio { get; set; }

        /// <summary>
        /// Lista dos Produtos mais comprados por este Cliente;
        /// Mostra os produtos mais adquiridos, incluindo o nome e a quantidade comprada.
        /// </summary>
        public List<ProdutoCompradoDto> ProdutosMaisComprados { get; set; } = new List<ProdutoCompradoDto>();

        /// <summary>
        /// Histórico de Pedidos do Cliente;
        /// Uma lista com os detalhes dos pedidos realizados, incluindo data e valor.
        /// </summary>
        public List<PedidoHistoricoDto> HistoricoDePedidos { get; set; } = new List<PedidoHistoricoDto>();

        /// <summary>
        /// Tempo Médio de Processamento dos Pedidos do Cliente;
        /// Calcula o tempo médio em dias para processar os pedidos, desde a criação até o pagamento.
        /// </summary>
        public double TempoMedioProcessamento { get; set; }

        /// <summary>
        /// Percentual de Devoluções ou Cancelamentos;
        /// Calcula a porcentagem de pedidos do cliente que foram devolvidos ou cancelados.
        /// </summary>
        public double PercentualDeDevolucoes { get; set; }
    }

    /// <summary>
    /// DTO para Produtos Comprados
    /// </summary>
    public class ProdutoCompradoDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int QuantidadeComprada { get; set; } = 0;
    }

    /// <summary>
    /// DTO para Histórico de Pedidos
    /// </summary>
    public class PedidoHistoricoDto
    {
        public Guid Id { get; set; }  // Adiciona a propriedade Id
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }
    }

    public class ItemPedidoDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }


    public class ItensPedidoConsolidadoDto
    {
        public List<ItemPedidoDto> Itens { get; set; } = new List<ItemPedidoDto>();
        public int QuantidadeTotalItens { get; set; }
        public decimal ValorTotalItens { get; set; }
    }

}
