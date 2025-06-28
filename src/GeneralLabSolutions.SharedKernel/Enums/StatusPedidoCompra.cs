using System.ComponentModel;

namespace GeneralLabSolutions.SharedKernel.Enums
{
    public enum StatusPedidoCompra
    {
        [Description("Em Elaboração")]      // Pedido sendo montado, itens sendo adicionados
        EmElaboracao = 1,

        [Description("Aguardando Aprovação Interna")] // Submetido para aprovação interna (se houver esse fluxo)
        AguardandoAprovacaoInterna = 2,

        [Description("Aprovado / Pronto para Envio")] // Aprovado internamente, pronto para ser enviado ao fornecedor
        AprovadoProntoParaEnvio = 3,

        [Description("Enviado ao Fornecedor")]  // Formalmente enviado
        EnviadoAoFornecedor = 4,

        [Description("Confirmado pelo Fornecedor")] // Fornecedor confirmou o recebimento e aceite do pedido
        ConfirmadoPeloFornecedor = 5,

        [Description("Em Produção / Separação")] // Fornecedor está processando
        EmProducaoOuSeparacao = 6,

        [Description("Parcialmente Enviado pelo Fornecedor")] // Fornecedor enviou parte dos itens
        ParcialmenteEnviadoPeloFornecedor = 7,

        [Description("Totalmente Enviado pelo Fornecedor")] // Fornecedor enviou todos os itens
        TotalmenteEnviadoPeloFornecedor = 8,

        [Description("Em Trânsito")]          // Mercadoria a caminho
        EmTransito = 9,

        [Description("Recebido Parcialmente")] // Sua empresa recebeu parte dos itens
        RecebidoParcialmente = 10,

        [Description("Recebido Totalmente / Concluído")] // Sua empresa recebeu todos os itens, pedido finalizado
        RecebidoTotalmenteConcluido = 11,

        [Description("Cancelado")]
        Cancelado = 12,

        [Description("Com Pendência")] // Ex: problema no recebimento, aguardando resolução
        ComPendencia = 13
    }
}