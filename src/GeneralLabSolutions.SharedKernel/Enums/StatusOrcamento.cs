using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLabSolutions.SharedKernel.Enums
{
    public enum StatusOrcamento
    {
        [Description("Em Elaboração")] // Vendedor está montando
        EmElaboracao = 1,

        [Description("Aguardando Análise de Compra")] // Vendedor finalizou, aguarda financeiro/compras
        AguardandoAnaliseCompra = 2,

        [Description("Análise de Compra Concluída")] // Financeiro/compras analisou todos os itens
        AnaliseCompraConcluida = 3, // Pode ter itens aprovados, rejeitados ou pendentes de decisão final

        [Description("Convertido em Pedido de Venda")] // Orçamento virou um pedido de venda para o cliente
        ConvertidoEmPedidoVenda = 4,

        [Description("Cancelado")]
        Cancelado = 5,

        [Description("Expirado")]
        Expirado = 6
    }
}


