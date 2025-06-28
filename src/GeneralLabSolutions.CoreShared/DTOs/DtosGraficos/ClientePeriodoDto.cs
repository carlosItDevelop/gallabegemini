namespace GeneralLabSolutions.CoreShared.DTOs.DtosGraficos
{
    public class ClientePeriodoDto
    {
        public Guid ClienteId { get; set; }
        public string? NomeCliente { get; set; }
        public int Periodo { get; set; }  // Alterado de "Trimestre" para "Periodo"
        public int QuantidadePedidos { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
