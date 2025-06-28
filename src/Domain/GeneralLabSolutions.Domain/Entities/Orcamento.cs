using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.SharedKernel.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    /// <summary>
    /// Representa um orçamento de venda, que pode ser elaborado por um vendedor para um cliente potencial ou existente.
    /// </summary>
    public class Orcamento : EntityAudit, IAggregateRoot
    {
        public Guid VendedorId { get; private set; }
        public virtual Vendedor Vendedor { get; private set; } // Ou ApplicationUser

        public string? NomeClientePotencial { get; private set; } // Pode ser um ClienteId se já existir
        public Guid? ClienteId { get; private set; } // FK para Cliente, se aplicável
        public virtual Cliente? Cliente { get; private set; }

        public DateTime DataCriacao { get; private set; }
        public DateTime? DataValidade { get; private set; }
        public StatusOrcamento Status { get; private set; }
        public string? Observacoes { get; private set; }
        public decimal ValorTotalOrcado { get; private set; } // Calculado

        private readonly List<ItemOrcamento> _itens = new();
        public IReadOnlyCollection<ItemOrcamento> Itens => _itens.AsReadOnly();

        // Construtor para EF Core
        protected Orcamento() { }

        public Orcamento(Guid vendedorId, string? nomeClientePotencial, Guid? clienteId = null, DateTime? dataValidade = null)
        {
            VendedorId = vendedorId;
            NomeClientePotencial = nomeClientePotencial;
            ClienteId = clienteId;
            DataCriacao = DateTime.UtcNow;
            DataValidade = dataValidade ?? DateTime.UtcNow.AddDays(30); // Padrão de validade: 30 dias
            Status = StatusOrcamento.EmElaboracao;
            ValorTotalOrcado = 0;

            AdicionarEvento(new OrcamentoCriadoEvent(Id, VendedorId, NomeClientePotencial, DataCriacao));
        }

        private void ValidarSeModificavel()
        {
            if (Status != StatusOrcamento.EmElaboracao && Status != StatusOrcamento.AguardandoAnaliseCompra && Status != StatusOrcamento.AnaliseCompraConcluida)
            {
                throw new DomainException($"Orçamento com status '{Status}' não pode ser modificado.");
            }
        }

        private void RecalcularValorTotal()
        {
            ValorTotalOrcado = _itens.Where(i => i.Status != StatusItemOrcamento.Cancelado && i.Status != StatusItemOrcamento.RejeitadoParaCompra)
                                     .Sum(i => i.ValorItem);
        }

        public void AdicionarItem(Produto produto, int quantidade, decimal? precoUnitarioVendaSugerido = null, Guid? fornecedorSugeridoId = null)
        {
            ValidarSeModificavel();
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

            // Lógica para determinar o precoUnitarioVenda
            // Aqui entraria a busca em ConfiguracaoSistema pelo IndiceLucroPadrao e a lógica de impostos
            // Por simplicidade, vamos assumir que o precoUnitarioVendaSugerido é o que será usado,
            // ou um valor padrão do produto se não for fornecido.
            decimal valorUnitarioFinal = precoUnitarioVendaSugerido ?? produto.ValorUnitario; // Simplificação

            var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id && i.Status != StatusItemOrcamento.Cancelado);
            if (itemExistente != null)
            {
                itemExistente.IncrementarQuantidade(quantidade);
                // Poderia ter lógica para atualizar o preço se o novo preço sugerido for diferente.
            } else
            {
                var novoItem = new ItemOrcamento(Id, produto, quantidade, valorUnitarioFinal, fornecedorSugeridoId);
                _itens.Add(novoItem);
            }
            RecalcularValorTotal();
            AdicionarEvento(new ItemAdicionadoAoOrcamentoEvent(Id, produto.Id, quantidade, valorUnitarioFinal));
        }

        public void RemoverItem(Guid itemOrcamentoId)
        {
            ValidarSeModificavel();
            var item = _itens.FirstOrDefault(i => i.Id == itemOrcamentoId);
            if (item == null)
                throw new DomainException("Item do orçamento não encontrado.");

            // Em vez de remover, podemos marcar como cancelado se quisermos manter o histórico
            // _itens.Remove(item);
            item.Cancelar(); // Se o item tiver um método Cancelar()
            RecalcularValorTotal();
            AdicionarEvento(new ItemRemovidoDoOrcamentoEvent(Id, itemOrcamentoId));
        }

        public void AtualizarQuantidadeItem(Guid itemOrcamentoId, int novaQuantidade)
        {
            ValidarSeModificavel();
            if (novaQuantidade <= 0)
                throw new ArgumentException("Nova quantidade deve ser maior que zero.", nameof(novaQuantidade));
            var item = _itens.FirstOrDefault(i => i.Id == itemOrcamentoId);
            if (item == null)
                throw new DomainException("Item do orçamento não encontrado.");

            item.AtualizarQuantidade(novaQuantidade);
            RecalcularValorTotal();
            AdicionarEvento(new QuantidadeItemOrcamentoAtualizadaEvent(Id, itemOrcamentoId, novaQuantidade));
        }

        public void EnviarParaAnaliseDeCompra()
        {
            if (Status != StatusOrcamento.EmElaboracao)
                throw new DomainException($"Não é possível enviar para análise um orçamento com status '{Status}'.");
            if (!_itens.Any())
                throw new DomainException("Não é possível enviar um orçamento vazio para análise.");

            Status = StatusOrcamento.AguardandoAnaliseCompra;
            foreach (var item in _itens.Where(i => i.Status == StatusItemOrcamento.PendenteAnalise)) // Apenas os pendentes
            {
                // Se houver um fluxo de aprovação inicial para o vendedor, poderia ser aqui.
                // Por ora, eles já estão PendenteAnalise por padrão.
            }
            AdicionarEvento(new OrcamentoEnviadoParaAnaliseEvent(Id));
        }

        // Métodos para o processo de análise de compra (chamados pelo Financeiro/Compras)
        public void AprovarItemParaCompra(Guid itemOrcamentoId, Guid responsavelAprovacaoId)
        {
            if (Status != StatusOrcamento.AguardandoAnaliseCompra && Status != StatusOrcamento.AnaliseCompraConcluida)
                throw new DomainException($"Orçamento com status '{Status}' não permite aprovação de itens para compra.");

            var item = _itens.FirstOrDefault(i => i.Id == itemOrcamentoId);
            if (item == null)
                throw new DomainException("Item do orçamento não encontrado.");

            item.AprovarParaCompra(responsavelAprovacaoId); // Método na entidade ItemOrcamento
            VerificarConclusaoAnaliseCompra();
            AdicionarEvento(new ItemOrcamentoStatusAlteradoEvent(Id, item.Id, item.Status, responsavelAprovacaoId));
        }

        /// <summary>
        /// Rejeita um item específico do orçamento durante o processo de análise de compra.
        /// 
        /// Pré-condições:
        /// - O orçamento deve estar com status "AguardandoAnaliseCompra" ou "AnaliseCompraConcluida".
        /// - O item a ser rejeitado deve existir no orçamento.
        /// 
        /// Ações realizadas:
        /// - Chama o método <c>RejeitarParaCompra</c> do <see cref="ItemOrcamento"/>, informando o responsável e o motivo da rejeição.
        /// - Verifica se todos os itens do orçamento já foram analisados (aprovados ou rejeitados) e, se sim, atualiza o status do orçamento para "AnaliseCompraConcluida".
        /// - Adiciona um evento de alteração de status do item (<see cref="ItemOrcamentoStatusAlteradoEvent"/>).
        /// 
        /// Exceções:
        /// - <see cref="DomainException"/> se o orçamento não estiver em um status válido para rejeição de itens.
        /// - <see cref="DomainException"/> se o item não for encontrado no orçamento.
        /// </summary>
        /// <param name="itemOrcamentoId">ID do item do orçamento a ser rejeitado.</param>
        /// <param name="responsavelRejeicaoId">ID do usuário responsável pela rejeição.</param>
        /// <param name="motivo">Motivo da rejeição do item.</param>
        public void RejeitarItemParaCompra(Guid itemOrcamentoId, Guid responsavelRejeicaoId, string motivo)
        {
            if (Status != StatusOrcamento.AguardandoAnaliseCompra && Status != StatusOrcamento.AnaliseCompraConcluida)
                throw new DomainException($"Orçamento com status '{Status}' não permite rejeição de itens para compra.");

            var item = _itens.FirstOrDefault(i => i.Id == itemOrcamentoId);
            if (item == null)
                throw new DomainException("Item do orçamento não encontrado.");

            item.RejeitarParaCompra(responsavelRejeicaoId, motivo); // Método na entidade ItemOrcamento
            VerificarConclusaoAnaliseCompra();
            AdicionarEvento(new ItemOrcamentoStatusAlteradoEvent(Id, item.Id, item.Status, responsavelRejeicaoId, motivo));
        }

        /// <summary>
        /// Verifica se a análise de compra do orçamento foi concluída.
        /// A análise é considerada concluída quando todos os itens do orçamento que não foram cancelados
        /// possuem status "AprovadoParaCompra" ou "RejeitadoParaCompra".
        /// Caso isso ocorra, o status do orçamento é atualizado para "AnaliseCompraConcluida"
        /// e um evento de conclusão de análise é adicionado.
        /// </summary>
        private void VerificarConclusaoAnaliseCompra()
        {
            // Se todos os itens que não foram cancelados já foram analisados (Aprovados ou Rejeitados)
            if (_itens.Where(i => i.Status != StatusItemOrcamento.Cancelado)
                      .All(i => i.Status == StatusItemOrcamento.AprovadoParaCompra || i.Status == StatusItemOrcamento.RejeitadoParaCompra))
            {
                Status = StatusOrcamento.AnaliseCompraConcluida;
                AdicionarEvento(new AnaliseDeCompraDoOrcamentoConcluidaEvent(Id));
            }
        }

        // Outros métodos: CancelarOrcamento, MarcarComoConvertidoEmPedidoVenda, AtualizarObservacoes, etc.
        public void Cancelar(string motivo, Guid usuarioId)
        {
            if (Status == StatusOrcamento.ConvertidoEmPedidoVenda || Status == StatusOrcamento.Cancelado)
            {
                throw new DomainException($"Orçamento com status '{Status}' não pode ser cancelado.");
            }
            Status = StatusOrcamento.Cancelado;
            Observacoes = string.IsNullOrEmpty(Observacoes) ? $"Cancelado: {motivo}" : $"{Observacoes}\nCancelado: {motivo}";
            // AdicionarEvento(new OrcamentoCanceladoEvent(Id, usuarioId, motivo));
        }
    }
}
