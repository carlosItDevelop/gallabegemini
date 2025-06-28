namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    // DTO para histórico de produtos vendidos

        public class ProdutoHistoricoDto
        {
            public Guid ProdutoId { get; set; }
            public string Nome { get; set; } = string.Empty;
            public int QuantidadeVendida { get; set; }
            public decimal ValorTotalVendido { get; set; }
        }


}