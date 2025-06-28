namespace GeneralLabSolutions.CoreShared.DTOs.DtosGraficos
{
    public class VendasNoAnoDto
    {
        public List<decimal> Pagos { get; set; } = new List<decimal>();
        public List<decimal> EmProcessamento { get; set; } = new List<decimal>();
        public List<decimal> Cancelados { get; set; } = new List<decimal>();
    }

}
