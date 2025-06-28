namespace GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents
{
    public class PedidoResumoDto
    {
        public int Quantidade { get; set; } = 0;
        public decimal ValorTotal { get; set; } = decimal.Zero;

        public PedidoResumoDto(int quantidade, decimal valorTotal)
        {
            Quantidade = quantidade;
            ValorTotal = valorTotal;
        }
    }

}
