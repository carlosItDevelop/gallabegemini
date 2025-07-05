// Em VelzonModerna.ViewModels.PedidoDeCompra.PedidoDeCompraDetailViewModel.cs
using System;
using System.Collections.Generic;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;

namespace VelzonModerna.ViewModels.PedidoDeCompra
{
    public class PedidoDeCompraDetailViewModel
    {
        public Guid Id { get; set; }
        public string NumeroPedidoCompra { get; set; } = string.Empty;

        // Dados do Fornecedor
        public string NomeFornecedor { get; set; } = string.Empty;
        public string? ContatoFornecedorEmail { get; set; }
        public string? ContatoFornecedorTelefone { get; set; }

        // Dados do Responsável
        public string NomeResponsavelCompra { get; set; } = string.Empty;
        public string? DepartamentoResponsavel { get; set; } // Se aplicável

        // Datas e Status
        public string DataEmissaoFormatada { get; set; } = string.Empty;
        public string? DataPrevisaoEntregaFormatada { get; set; }
        public string CondicoesPagamentoNegociadas { get; set; } = string.Empty;
        public StatusPedidoCompra Status { get; set; }
        public string StatusDescricao => Status.ToString(); // Ou usar um helper para obter a [Description]
        public string StatusCssClass { get; set; } = string.Empty;

        public string? Observacoes { get; set; }
        public string ValorTotalEstimadoFormatado { get; set; } = string.Empty;

        public List<ItemPedidoDeCompraDetailViewModel> Itens { get; set; }
        public List<HistoricoPedidoDeCompraViewModel> Historico { get; set; }

        // Para controlar botões de ação no modal
        public bool PodeEnviarParaAprovacao { get; set; }
        public bool PodeRegistrarRecebimento { get; set; }
        public bool PodeCancelar { get; set; }
        public bool PodeAdicionarItens { get; set; } // Se o pedido ainda estiver em elaboração

        public PedidoDeCompraDetailViewModel()
        {
            Itens = new List<ItemPedidoDeCompraDetailViewModel>();
            Historico = new List<HistoricoPedidoDeCompraViewModel>();
        }
    }

    public class ItemPedidoDeCompraDetailViewModel
    {
        public Guid Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string? CategoriaProduto { get; set; }
        public int QuantidadeSolicitada { get; set; }
        public string ValorUnitarioNegociadoFormatado { get; set; } = string.Empty;
        public string ValorTotalItemFormatado { get; set; } = string.Empty;
        public int QuantidadeRecebida { get; set; }
        public StatusItemPedidoCompra Status { get; set; }
        public string StatusDescricao => Status.ToString();
        public string StatusItemCssClass { get; set; } = string.Empty;

        public bool PodeEditarItem { get; set; } // Se o pedido principal permitir
        public bool PodeExcluirItem { get; set; }
    }

    public class HistoricoPedidoDeCompraViewModel
    {
        public string DescricaoEvento { get; set; } = string.Empty; // Ex: "Pedido Criado", "Status alterado para Aprovado"
        public string DataHoraFormatada { get; set; } = string.Empty;
        public string? Responsavel { get; set; } // Nome do usuário
        public string IconeCssClass { get; set; } = string.Empty; // Para o ícone no histórico
        public string CorIconeCssClass { get; set; } = string.Empty; // Para a cor do ícone
    }
}