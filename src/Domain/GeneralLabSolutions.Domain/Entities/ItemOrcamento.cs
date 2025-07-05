using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;

namespace GeneralLabSolutions.Domain.Entities
{
    public class ItemOrcamento : EntityBase
    {
        public Guid OrcamentoId { get; private set; }
        // Não ter a propriedade de navegação `Orcamento` aqui simplifica e reforça que ItemOrcamento é parte do agregado Orcamento.

        public Guid ProdutoId { get; private set; }
        public virtual Produto Produto { get; private set; }

        public string NomeProdutoSnapshot { get; private set; } // "Congela" o nome do produto no momento
        public int Quantidade { get; private set; }
        public decimal ValorUnitarioOrcado { get; private set; } // Preço de VENDA para o cliente
        public decimal ValorItem => Quantidade * ValorUnitarioOrcado; // Propriedade calculada

        public Guid? FornecedorSugeridoId { get; private set; }
        public virtual Fornecedor? FornecedorSugerido { get; private set; }

        public StatusItemOrcamento Status { get; private set; }
        public string? MotivoRejeicaoCompra { get; private set; } // Se Status for RejeitadoParaCompra
        public Guid? ResponsavelAnaliseCompraId { get; private set; } // Quem aprovou/rejeitou

        // Construtor para EF Core
        protected ItemOrcamento() { }

        // Construtor chamado pelo Orcamento
        internal ItemOrcamento(Guid orcamentoId, Produto produto, int quantidade, decimal valorUnitarioOrcado, Guid? fornecedorSugeridoId)
        {
            OrcamentoId = orcamentoId;
            ProdutoId = produto.Id;
            Produto = produto; // Pode ser útil para acesso rápido, mas não essencial se o ID for suficiente.
            NomeProdutoSnapshot = produto.Descricao; // Ou o nome que você preferir
            Quantidade = quantidade;
            ValorUnitarioOrcado = valorUnitarioOrcado;
            FornecedorSugeridoId = fornecedorSugeridoId;
            Status = StatusItemOrcamento.PendenteAnalise;
        }

        internal void IncrementarQuantidade(int quantidadeAdicional)
        {
            if (Status == StatusItemOrcamento.Cancelado)
                throw new DomainException("Item cancelado não pode ter quantidade incrementada.");
            if (quantidadeAdicional <= 0)
                throw new ArgumentException("Quantidade adicional deve ser maior que zero.");
            Quantidade += quantidadeAdicional;
        }

        internal void AtualizarQuantidade(int novaQuantidade)
        {
            if (Status == StatusItemOrcamento.Cancelado)
                throw new DomainException("Item cancelado não pode ter quantidade atualizada.");
            if (novaQuantidade <= 0)
                throw new ArgumentException("Nova quantidade deve ser maior que zero.");
            Quantidade = novaQuantidade;
        }

        internal void Cancelar()
        {
            if (Status == StatusItemOrcamento.CompraEmAndamento || Status == StatusItemOrcamento.CompraConcluida)
            {
                throw new DomainException($"Item com status '{Status}' não pode ser cancelado diretamente no orçamento. Verifique o Pedido de Compra associado.");
            }
            Status = StatusItemOrcamento.Cancelado;
        }

        // Métodos para o processo de análise de compra (chamados pelo Orcamento)
        internal void AprovarParaCompra(Guid responsavelId)
        {
            if (Status != StatusItemOrcamento.PendenteAnalise && Status != StatusItemOrcamento.RejeitadoParaCompra) // Pode reaprovar um rejeitado
                throw new DomainException($"Item com status '{Status}' não pode ser aprovado para compra.");
            Status = StatusItemOrcamento.AprovadoParaCompra;
            ResponsavelAnaliseCompraId = responsavelId;
            MotivoRejeicaoCompra = null;
        }

        internal void RejeitarParaCompra(Guid responsavelId, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Motivo da rejeição é obrigatório.", nameof(motivo));
            if (Status != StatusItemOrcamento.PendenteAnalise && Status != StatusItemOrcamento.AprovadoParaCompra) // Pode rejeitar um já aprovado
                throw new DomainException($"Item com status '{Status}' não pode ser rejeitado para compra.");
            Status = StatusItemOrcamento.RejeitadoParaCompra;
            ResponsavelAnaliseCompraId = responsavelId;
            MotivoRejeicaoCompra = motivo;
        }

        // Chamado quando um Pedido de Compra é gerado para este item
        internal void MarcarComoCompraEmAndamento()
        {
            if (Status != StatusItemOrcamento.AprovadoParaCompra)
                throw new DomainException($"Somente itens 'AprovadoParaCompra' podem ter sua compra iniciada. Status atual: '{Status}'.");
            Status = StatusItemOrcamento.CompraEmAndamento;
        }

        // Chamado quando o item correspondente no PedidoDeCompra é totalmente recebido
        internal void MarcarComoCompraConcluida()
        {
            if (Status != StatusItemOrcamento.CompraEmAndamento)
                throw new DomainException($"Somente itens com 'CompraEmAndamento' podem ser marcados como compra concluída. Status atual: '{Status}'.");
            Status = StatusItemOrcamento.CompraConcluida;
        }
    }
}
