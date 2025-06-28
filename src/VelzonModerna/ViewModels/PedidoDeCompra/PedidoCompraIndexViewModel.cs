// Em VelzonModerna.ViewModels.PedidoDeCompra.PedidoCompraIndexViewModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList

namespace VelzonModerna.ViewModels.PedidoDeCompra
{
    public class PedidoCompraIndexViewModel
    {
        // 1. Dados para os Filtros
        public FiltrosPedidoCompraViewModel Filtros { get; set; }

        // 2. Lista de Pedidos de Compra para a Tabela Principal
        public IEnumerable<PedidoDeCompraListViewModel> ListaDePedidos { get; set; }

        // 3. Modelo para o Formulário de Novo Pedido de Compra (Offcanvas)
        //    Este será populado com valores padrão ou vazios inicialmente
        //    e também com as SelectLists para os dropdowns.
        public CriarPedidoDeCompraViewModel FormularioNovoPedido { get; set; }

        // Poderíamos adicionar aqui também modelos para os modais de Detalhes e Editar Item
        // se quiséssemos que eles tivessem alguma estrutura pré-renderizada pelo servidor,
        // mas como eles são frequentemente populados via AJAX, podemos mantê-los simples por enquanto
        // e apenas renderizar suas estruturas vazias.

        public PedidoCompraIndexViewModel()
        {
            Filtros = new FiltrosPedidoCompraViewModel();
            ListaDePedidos = new List<PedidoDeCompraListViewModel>();
            FormularioNovoPedido = new CriarPedidoDeCompraViewModel();
        }
    }

    // --- ViewModels para as seções/partials ---

    public class FiltrosPedidoCompraViewModel
    {
        // Valores selecionados nos filtros (para manter o estado após post/submit)
        public Guid? FornecedorIdSelecionado { get; set; }
        public string? StatusSelecionado { get; set; } // Ou um Enum StatusPedidoCompra?
        public Guid? ResponsavelIdSelecionado { get; set; }
        public DateTime? PeriodoDataSelecionada { get; set; } // Apenas uma data, ou um range?

        // Opções para os dropdowns dos filtros
        public SelectList? FornecedoresOptions { get; set; }
        public SelectList? StatusOptions { get; set; } // Poderia ser List<SelectListItem> se populado manualmente
        public SelectList? ResponsaveisOptions { get; set; }
    }

    public class PedidoDeCompraListViewModel // Para cada linha na tabela da Index
    {
        public Guid Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public string NomeFornecedor { get; set; } = string.Empty;
        public string NomeResponsavel { get; set; } = string.Empty;
        public string DataEmissao { get; set; } = string.Empty; // Formatada
        public string DataPrevisaoEntrega { get; set; } = string.Empty; // Formatada
        public string Status { get; set; } = string.Empty; // Descrição do status
        public string StatusCssClass { get; set; } = string.Empty; // Classe CSS para o badge de status
        public string ValorTotalFormatado { get; set; } = string.Empty;

        // Para determinar quais botões de ação mostrar
        public bool PodeEditar { get; set; }
        public bool PodeCancelar { get; set; }
        public bool PodeRegistrarRecebimento { get; set; }
        public bool PodeEnviarParaAprovacao { get; set; }
    }
}