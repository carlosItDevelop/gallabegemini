using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLabSolutions.CoreShared.DTOs.DtosAgenteDeIA
{

    public class ClienteIAContextDto
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Documento { get; set; }
        public string? Status { get; set; }
        public List<PedidoResumoIADto>? Pedidos { get; set; } = new();
    }

    public class VendedorIAContextDto
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Documento { get; set; }
        public string? Status { get; set; }
        public List<PedidoResumoIADto>? Pedidos { get; set; } = new();
    }

    public class PedidoResumoIADto
    {
        public Guid Id { get; set; }
        public DateTime? Data { get; set; }
        public string? Status { get; set; }
        public decimal Valor { get; set; }
    }

    public class FornecedorIAContextDto
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Documento { get; set; }
        public string? Status { get; set; }
        public List<ProdutoResumoIADto>? Produtos { get; set; } = new();
    }

    public class ProdutoResumoIADto
    {
        public Guid Id { get; set; }
        public string? Codigo { get; set; }
        public string? NomeProduto { get; set; }
        public string? Categoria { get; set; }
        public string? Status { get; set; }
        public decimal ValorUnitario { get; set; }
    }

}
