using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusItemPedidoCompra
    {
        [Description("Pendente de Envio")] // Item aguardando envio do fornecedor
        PendenteDeEnvio = 1,

        [Description("Enviado pelo Fornecedor")]
        EnviadoPeloFornecedor = 2,

        [Description("Em Trânsito")]
        EmTransito = 3,

        [Description("Recebido")] // Item recebido conforme esperado
        Recebido = 4,

        [Description("Recebido com Divergência")] // Item recebido, mas com problemas (quantidade, qualidade)
        RecebidoComDivergencia = 5,

        [Description("Cancelado")] // Item cancelado do pedido de compra
        CanceladoPeloComprador = 6,

        [Description("Cancelado pelo Fornecedor")]
        CanceladoPeloFornecedor = 7
    }
}