// Em VelzonModerna.ViewModels.PedidoDeCompra.CriarPedidoDeCompraViewModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VelzonModerna.ViewModels.PedidoDeCompra
{
    public class CriarPedidoDeCompraViewModel
    {
        [Required(ErrorMessage = "O Fornecedor é obrigatório.")]
        [Display(Name = "Fornecedor")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "O Responsável é obrigatório.")]
        [Display(Name = "Responsável")]
        public Guid ResponsavelId { get; set; } // Assumindo que é um usuário do sistema

        [Required(ErrorMessage = "A Data de Emissão é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Emissão")]
        public DateTime DataEmissao { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "A Previsão de Entrega é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Previsão de Entrega")]
        public DateTime DataPrevisaoEntrega { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "Condição de Pagamento")]
        public string? CondicaoPagamentoId { get; set; } // Poderia ser um Guid para uma entidade CondPagto

        [DataType(DataType.MultilineText)]
        public string? Observacoes { get; set; }

        // Lista de itens para o novo pedido
        // Começa com um item vazio para o usuário preencher, JS pode adicionar mais.
        public List<ItemPedidoDeCompraInputViewModel> Itens { get; set; }

        // Opções para os dropdowns do formulário (serão populadas pelo Controller)
        public SelectList? FornecedoresOptions { get; set; }
        public SelectList? ResponsaveisOptions { get; set; }
        public SelectList? CondicoesPagamentoOptions { get; set; }
        public SelectList? ProdutosOptions { get; set; } // Para o select de produto dentro dos itens

        public CriarPedidoDeCompraViewModel()
        {
            Itens = new List<ItemPedidoDeCompraInputViewModel> { new ItemPedidoDeCompraInputViewModel() }; // Começa com um item
        }
    }

    public class ItemPedidoDeCompraInputViewModel // Usado no formulário de criação/edição de itens
    {
        // public Guid? Id { get; set; } // Para identificar o item ao editar, não necessário para criação inicial na lista

        [Required(ErrorMessage = "O Produto é obrigatório.")]
        [Display(Name = "Produto")]
        public Guid ProdutoId { get; set; }

        [Required(ErrorMessage = "A Quantidade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; } = 1;

        [Required(ErrorMessage = "O Valor Unitário é obrigatório.")]
        [DataType(DataType.Currency)]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero.")]
        [Display(Name = "Valor Unitário (Custo)")]
        public decimal ValorUnitarioCusto { get; set; }

        // O valor total será calculado, não precisa de input direto no form do item,
        // mas pode ser exibido.
        // public decimal ValorTotalItem => Quantidade * ValorUnitarioCusto;
    }
}