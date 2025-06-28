using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Produto : EntityBase
    {
        public Produto(string? codigo,
                       string? descricao,
                       string ncm,
                       decimal valorUnitario,
                       Guid categoriaId,
                       Guid fornecedorId)
        {
            Codigo = codigo;
            Descricao = descricao;
            Ncm = ncm;
            ValorUnitario = valorUnitario;
            CategoriaId = categoriaId;
            FornecedorId = fornecedorId;
        }

        // EF
        public Produto() { }

        public string? Codigo { get; init; }

        public string? Descricao { get; private set; }


        public string? Ncm { get; private set; }
        public decimal ValorUnitario { get; private set; }

        // Todo: Deixar este `Set` privado!
        public StatusDoProduto StatusDoProduto { get; set; }
            = StatusDoProduto.Dropshipping;

        public DateTime DataDeValidade { get; set; } = DateTime.UtcNow.AddMonths(10);
        public string Imagem { get; set; } = "img-padrao.jpg";

        // Relacionamentos
        public Guid CategoriaId { get; private set; }
        public virtual CategoriaProduto? CategoriaProduto { get; set; }

        public Guid FornecedorId { get; private set; }
        public virtual Fornecedor? Fornecedor { get; set; }

        // Métodos de atualização
        public void AlterarValorUnitario(decimal novoValor) => this.ValorUnitario = novoValor;
        public void AlterarDescricao(string descricao) => this.Descricao = descricao;
        public void AlterarStatus(StatusDoProduto status) => this.StatusDoProduto = status;
    }
}
