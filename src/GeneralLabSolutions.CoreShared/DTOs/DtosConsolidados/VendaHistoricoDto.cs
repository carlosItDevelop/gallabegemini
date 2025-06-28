namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{
    public class VendaHistoricoDto
    {
        public Guid Id { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }

}
