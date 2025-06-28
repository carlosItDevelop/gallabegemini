namespace VelzonModerna.ViewModels
{
    public class DonutEstadosPedidosViewModel
    {
        public int QuantidadeOrcamento { get; set; } = 0;
        public int QuantidadeEmProcessamento { get; set; } = 0;
        public int QuantidadeCancelado { get; set; } = 0;
        public int QuantidadeEntregue { get; set; } = 0;

        public string ValorOrcamento { get; set; } = string.Empty;
        public string ValorEmProcessamento { get; set; } = string.Empty;
        public string ValorCancelado { get; set; } = string.Empty;
        public string ValorEntregue { get; set; } = string.Empty;

    }
}