namespace GeneralLabSolutions.CoreShared.DTOs.DtosConsolidados
{

    public class ItemVendaDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }

        // Calcular o valor total com base no valor unitário e quantidade
        public decimal ValorTotal { get; set; } = decimal.Zero;
    }


}
