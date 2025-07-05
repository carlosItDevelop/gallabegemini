using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusItemOrcamento
    {
        [Description("Pendente de Análise")] // Item adicionado, aguardando análise de compra
        PendenteAnalise = 1,

        [Description("Aprovado para Compra")] // Financeiro/Compras aprovou este item para ser comprado do fornecedor
        AprovadoParaCompra = 2,

        [Description("Compra em Andamento")] // Um PedidoDeCompra foi gerado para este item
        CompraEmAndamento = 3,

        [Description("Compra Concluída")] // O item foi totalmente recebido do fornecedor
        CompraConcluida = 4,

        [Description("Rejeitado para Compra")] // Financeiro/Compras não aprovou a compra deste item
        RejeitadoParaCompra = 5,

        [Description("Cancelado")] // Item cancelado do orçamento
        Cancelado = 6
    }
}
