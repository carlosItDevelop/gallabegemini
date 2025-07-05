using System.ComponentModel;

namespace GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos
{
    public enum StatusDoPedido
    {
        [Description("Orçamento do Vendedor")] Orcamento, // Peso: 10 - maior que cancelado
        [Description("Em Processamento")] EmProcessamento, // Peso: 20 - ainda maior
        [Description("Pago")] Pago, // Peso: 30 - mais alto (prioritário)
        [Description("Enviado")] Enviado, // Peso: 10 - médio
        [Description("Entregue")] Entregue, // Peso: 20 - grande
        [Description("Cancelado")] Cancelado // Peso: 5 - mínimo (raridade)
    }
}