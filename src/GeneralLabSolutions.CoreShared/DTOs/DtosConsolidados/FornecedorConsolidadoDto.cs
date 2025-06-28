namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    public class FornecedorConsolidadoDto
    {
        public Guid FornecedorId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int TotalDeProdutos { get; set; }
        public decimal ValorTotalGerado { get; set; }
        public List<ProdutoHistoricoDto> ProdutosMaisVendidos { get; set; } = new();
        public List<CategoriaProdutoDto> CategoriasMaisPopulares { get; set; } = new(); // <--- TROCA AQUI
        public double TaxaDevolucao { get; set; }
        public List<VendaHistoricoDto> HistoricoDeVendas { get; set; } = new();
    }
}
