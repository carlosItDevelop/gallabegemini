using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums.OrcamentoEPedidos;

public class ItemPedidoDeCompra : EntityBase
{
    public Guid PedidoDeCompraId { get; private set; }
    // public virtual PedidoDeCompra PedidoDeCompra { get; private set; }

    public Guid ProdutoId { get; private set; }
    public virtual Produto Produto { get; private set; }

    public string NomeProdutoSnapshot { get; private set; }
    public int QuantidadeSolicitada { get; private set; }
    public decimal ValorUnitarioNegociado { get; private set; } // CUSTO
    public decimal ValorTotalItemSolicitado => QuantidadeSolicitada * ValorUnitarioNegociado;

    public Guid? ItemOrcamentoOrigemId { get; private set; }
    // public virtual ItemOrcamento? ItemOrcamentoOrigem { get; private set; }

    public int QuantidadeRecebida { get; private set; }
    public DateTime? DataUltimoRecebimento { get; private set; }
    public string? ObservacaoRecebimento { get; private set; } // Para notas sobre o recebimento

    public StatusItemPedidoCompra Status { get; private set; }

    // Construtor para EF Core
    protected ItemPedidoDeCompra() { }

    // Construtor chamado por PedidoDeCompra
    internal ItemPedidoDeCompra(Guid pedidoDeCompraId, Produto produto, string nomeProdutoSnapshot, int quantidadeSolicitada, decimal valorUnitarioNegociado, Guid? itemOrcamentoOrigemId)
    {
        PedidoDeCompraId = pedidoDeCompraId;
        ProdutoId = produto.Id;
        Produto = produto;
        NomeProdutoSnapshot = nomeProdutoSnapshot;
        QuantidadeSolicitada = quantidadeSolicitada;
        ValorUnitarioNegociado = valorUnitarioNegociado;
        ItemOrcamentoOrigemId = itemOrcamentoOrigemId;
        Status = StatusItemPedidoCompra.PendenteDeEnvio;
        QuantidadeRecebida = 0;
    }

    internal void RegistrarRecebimento(int quantidade, string? observacao, bool comDivergencia)
    {
        if (Status == StatusItemPedidoCompra.CanceladoPeloComprador || Status == StatusItemPedidoCompra.CanceladoPeloFornecedor)
            throw new DomainException("Não é possível registrar recebimento para um item cancelado.");
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade recebida deve ser maior que zero.");
        if (QuantidadeRecebida + quantidade > QuantidadeSolicitada)
            throw new DomainException("Quantidade recebida excede a quantidade solicitada.");

        QuantidadeRecebida += quantidade;
        DataUltimoRecebimento = DateTime.UtcNow;
        ObservacaoRecebimento = string.IsNullOrWhiteSpace(ObservacaoRecebimento) ? observacao : $"{ObservacaoRecebimento}\n{DateTime.UtcNow:g}: {observacao}";


        if (comDivergencia)
        {
            Status = StatusItemPedidoCompra.RecebidoComDivergencia;
        } else if (QuantidadeRecebida >= QuantidadeSolicitada)
        {
            Status = StatusItemPedidoCompra.Recebido;
        }
        // Se for parcial e sem divergência explícita, mantém o status anterior (ex: EnviadoPeloFornecedor)
        // ou poderia ter um status "Recebido Parcialmente" a nível de item também.
        // O status do PedidoDeCompra global tratará o recebimento parcial.

        // Disparar evento de recebimento para possível atualização de estoque etc.
        // O PedidoDeCompra é quem vai adicionar este evento à sua lista de eventos.
        // Aqui, o Item apenas se atualiza.
    }

    internal void CancelarItemPeloComprador(string motivo)
    {
        // Pode haver regras sobre quando um item pode ser cancelado
        if (Status >= StatusItemPedidoCompra.EnviadoPeloFornecedor)
        {
            throw new DomainException($"Item com status '{Status}' não pode ser cancelado pelo comprador diretamente neste ponto.");
        }
        Status = StatusItemPedidoCompra.CanceladoPeloComprador;
        ObservacaoRecebimento = string.IsNullOrEmpty(ObservacaoRecebimento) ? $"Cancelado Comprador: {motivo}" : $"{ObservacaoRecebimento}\nCancelado Comprador: {motivo}";
    }

    // Poderia ter um método para o Fornecedor cancelar, se aplicável.
}