namespace GeneralLabSolutions.CoreShared.DTOs.DtosViewComponents
{
    public class CardPedidoViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public int Quantidade { get; set; } = 0;
        public string Valor { get; set; } = "0.00";
        public string CssColor { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
