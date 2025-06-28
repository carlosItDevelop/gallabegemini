namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    public class VendedorConsolidadoDto
    {
        public Guid VendedorId { get; set; }
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Total de Vendas feitas por este Vendedor
        /// </summary>
        public int TotalDeVendas { get; set; } = 0;


        /// <summary>
        /// Ticket Médio por Venda deste Vendedor
        /// Calculado pelo total vendido dividido pelo número de vendas
        /// </summary>
        public decimal TicketMedioPorVenda { get; set; } = decimal.Zero;

        /// <summary>
        /// Data da última venda realizada pelo vendedor
        /// </summary>
        public DateTime? UltimaVenda { get; set; }

        /// <summary>
        /// Total de vendas por categoria
        /// </summary>
        public int QuantidadeDeVendasPorCategoria { get; set; } = 0;

        /// <summary>
        /// Categoria mais vendida por este vendedor
        /// </summary>
        public CategoriaProdutoDto? CategoriaMaisVendida { get; set; }

        /// <summary>
        /// Total de vendas por produto
        /// </summary>
        public int QuantidadeDeVendasPorProduto { get; set; } = 0;

        /// <summary>
        /// Produto mais vendido por este vendedor
        /// </summary>
        public ProdutoDto? ProdutoMaisVendido { get; set; }

        /// <summary>
        /// Posição do vendedor no ranking de vendas por valor
        /// </summary>
        public int RankingPorValorEmVendasFinalizadas { get; set; }

        /// <summary>
        /// Posição do vendedor no ranking por quantidade de vendas finalizadas
        /// </summary>
        public int RankingPorQuantidadeDeVendasFinalizadas { get; set; }

        /// <summary>
        /// Percentual de conversão entre orçamentos e vendas finalizadas
        /// </summary>
        public int PercentualDeConversao { get; set; }

        /// <summary>
        /// Intervalo médio entre as vendas realizadas pelo vendedor
        /// </summary>
        public int IntervaloMedioEntreVendas { get; set; }

        /// <summary>
        /// Valor total das vendas realizadas pelo vendedor
        /// </summary>
        public decimal ValorTotalDeVendas { get; set; } = decimal.Zero;

        /// <summary>
        /// Frequência média de vendas
        /// </summary>
        public double FrequenciaDeVendasMedia { get; set; }

        /// <summary>
        /// Histórico das vendas realizadas pelo vendedor
        /// </summary>
        public List<VendaHistoricoDto> HistoricoDeVendas { get; set; } = new List<VendaHistoricoDto>();

        /// <summary>
        /// Percentual de devoluções ou cancelamentos
        /// </summary>
        public double PercentualDeDevolucoes { get; set; }
    }

}
