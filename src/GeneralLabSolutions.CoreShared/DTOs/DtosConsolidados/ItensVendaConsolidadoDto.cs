namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    public class ItensVendaConsolidadoDto
    {
        public List<ItemVendaDto> Itens { get; set; } // Alterado para ItemVendaDto
        public int QuantidadeTotalItens { get; set; }
        public decimal ValorTotalItens { get; set; }
    }

}
