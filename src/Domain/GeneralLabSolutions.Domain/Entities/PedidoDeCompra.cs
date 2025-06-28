using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.SharedKernel.Enums;

public class PedidoDeCompra : EntityAudit, IAggregateRoot
{
    public string NumeroPedidoCompra { get; private set; } // Gerado automaticamente ou manualmente?
    public Guid FornecedorId { get; private set; }
    public virtual Fornecedor Fornecedor { get; private set; }

    public Guid ResponsavelCompraId { get; private set; } // Id do usuário (ApplicationUser) que criou/gerenciou
    // public virtual ApplicationUser ResponsavelCompra { get; private set; }

    public DateTime DataEmissao { get; private set; }
    public DateTime? DataPrevisaoEntregaFornecedor { get; private set; }
    public DateTime? DataEfetivaEntregaTotal { get; private set; } // Quando o último item foi entregue

    public StatusPedidoCompra Status { get; private set; }
    public string? CondicoesPagamentoNegociadas { get; private set; }
    public string? Observacoes { get; private set; }
    public decimal ValorTotalEstimado { get; private set; } // Soma dos (QuantSol * ValorUnitNeg)
    public decimal ValorTotalRecebido { get; private set; } // Soma dos itens efetivamente recebidos e conferidos

    private readonly List<ItemPedidoDeCompra> _itens = new();
    public IReadOnlyCollection<ItemPedidoDeCompra> Itens => _itens.AsReadOnly();

    // Construtor para EF Core
    public PedidoDeCompra() { }

    public PedidoDeCompra(Guid fornecedorId, Guid responsavelCompraId, string? numeroPedidoCompra = null)
    {
        FornecedorId = fornecedorId;
        ResponsavelCompraId = responsavelCompraId;
        DataEmissao = DateTime.UtcNow;
        Status = StatusPedidoCompra.EmElaboracao;
        // Lógica para gerar NumeroPedidoCompra (ex: ANO-MES-SEQUENCIAL ou via serviço)
        NumeroPedidoCompra = numeroPedidoCompra ?? GerarNumeroPedidoCompraTemporario();
        ValorTotalEstimado = 0;
        ValorTotalRecebido = 0;

        AdicionarEvento(new PedidoDeCompraCriadoEvent(Id, NumeroPedidoCompra, FornecedorId, ResponsavelCompraId, DataEmissao));
    }

    // Método temporário/exemplo para número do pedido
    private string GerarNumeroPedidoCompraTemporario() => $"PC-{DateTime.UtcNow:yyyyMMddHHmmss}-{Id.ToString().Substring(0, 4)}";

    private void ValidarSeModificavel()
    {
        // Permite modificar enquanto EmElaboracao ou AguardandoAprovacaoInterna.
        // Outros status podem ter regras mais rígidas.
        if (Status != StatusPedidoCompra.EmElaboracao && Status != StatusPedidoCompra.AguardandoAprovacaoInterna)
        {
            throw new DomainException($"Pedido de Compra com status '{Status}' não pode ser modificado (itens/valores).");
        }
    }

    private void RecalcularValorTotalEstimado()
    {
        ValorTotalEstimado = _itens.Where(i => i.Status != StatusItemPedidoCompra.CanceladoPeloComprador && i.Status != StatusItemPedidoCompra.CanceladoPeloFornecedor)
                                   .Sum(i => i.ValorTotalItemSolicitado);
    }

    private void RecalcularValorTotalRecebido()
    {
        // Considera apenas itens recebidos (com ou sem divergência, mas o valor é baseado no que foi negociado pela quantidade recebida)
        ValorTotalRecebido = _itens.Where(i => i.Status == StatusItemPedidoCompra.Recebido || i.Status == StatusItemPedidoCompra.RecebidoComDivergencia)
                                   .Sum(i => i.QuantidadeRecebida * i.ValorUnitarioNegociado);
    }

    public void AdicionarItem(Produto produto, int quantidade, decimal valorUnitarioNegociado, Guid? itemOrcamentoOrigemId = null)
    {
        ValidarSeModificavel();
        if (produto == null)
            throw new ArgumentNullException(nameof(produto));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));
        if (valorUnitarioNegociado < 0)
            throw new ArgumentException("Valor unitário negociado não pode ser negativo.", nameof(valorUnitarioNegociado));

        // Poderia verificar se o item já existe e, em vez de adicionar, incrementar quantidade, mas para Pedido de Compra,
        // cada linha costuma ser distinta, especialmente se vinculada a um ItemOrcamentoOrigemId.
        // Se for o caso de agrupar, a lógica seria similar à do Orcamento.
        var novoItem = new ItemPedidoDeCompra(Id, produto, produto.Descricao, quantidade, valorUnitarioNegociado, itemOrcamentoOrigemId);
        _itens.Add(novoItem);
        RecalcularValorTotalEstimado();

        AdicionarEvento(new ItemAdicionadoAoPedidoDeCompraEvent(Id, novoItem.Id, produto.Id, quantidade, valorUnitarioNegociado));
    }

    public void RemoverItem(Guid itemPedidoDeCompraId)
    {
        ValidarSeModificavel();
        var item = _itens.FirstOrDefault(i => i.Id == itemPedidoDeCompraId);
        if (item == null)
            throw new DomainException("Item do pedido de compra não encontrado.");

        item.CancelarItemPeloComprador("Removido pelo comprador antes do envio."); // Ou uma lógica de remoção física se permitido
        RecalcularValorTotalEstimado();
        // Se o item foi fisicamente removido: _itens.Remove(item);
        AdicionarEvento(new ItemRemovidoDoPedidoDeCompraEvent(Id, itemPedidoDeCompraId));
    }

    // Outros métodos para atualizar quantidade/valor de item seriam similares, sempre chamando RecalcularValorTotalEstimado.

    public void SubmeterParaAprovacaoInterna()
    {
        if (Status != StatusPedidoCompra.EmElaboracao)
            throw new DomainException($"Pedido de Compra com status '{Status}' não pode ser submetido para aprovação.");
        if (!_itens.Any())
            throw new DomainException("Não é possível submeter um Pedido de Compra vazio para aprovação.");

        Status = StatusPedidoCompra.AguardandoAprovacaoInterna;
        AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status));
    }

    public void AprovarInternamente(Guid responsavelAprovacaoId)
    {
        if (Status != StatusPedidoCompra.AguardandoAprovacaoInterna)
            throw new DomainException($"Pedido de Compra com status '{Status}' não pode ser aprovado internamente.");

        // Atualizar ResponsavelAprovacaoId se tiver esse campo.
        Status = StatusPedidoCompra.AprovadoProntoParaEnvio;
        AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status, responsavelAprovacaoId));
    }

    public void EnviarAoFornecedor(DateTime dataPrevisaoEntrega)
    {
        if (Status != StatusPedidoCompra.AprovadoProntoParaEnvio)
            throw new DomainException($"Pedido de Compra com status '{Status}' não pode ser enviado ao fornecedor.");

        DataPrevisaoEntregaFornecedor = dataPrevisaoEntrega;
        Status = StatusPedidoCompra.EnviadoAoFornecedor;
        // Aqui poderia ter lógica para notificar o ItemOrcamento de origem que a compra está em andamento.
        foreach (var itemPc in _itens.Where(i => i.ItemOrcamentoOrigemId.HasValue))
        {
            // Disparar evento para que um handler possa atualizar o ItemOrcamento correspondente
            AdicionarEvento(new NotificarItemOrcamentoCompraIniciadaEvent(itemPc.ItemOrcamentoOrigemId.Value, Id, itemPc.Id));
        }
        AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status));
    }

    public void ConfirmarPeloFornecedor(string? confirmacaoFornecedorDetalhes = null)
    {
        if (Status != StatusPedidoCompra.EnviadoAoFornecedor)
            throw new DomainException($"Aguardando confirmação do fornecedor para pedido com status '{Status}'.");
        Status = StatusPedidoCompra.ConfirmadoPeloFornecedor;
        // Adicionar confirmacaoFornecedorDetalhes a um campo se necessário.
        AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status));
    }

    // Métodos para registrar recebimento (parcial/total) dos itens e do pedido.
    // Estes métodos seriam mais complexos, atualizando QuantidadeRecebida nos itens,
    // disparando eventos para o estoque, e potencialmente mudando o Status do PedidoDeCompra.

    public void RegistrarRecebimentoItem(Guid itemPedidoDeCompraId, int quantidadeRecebida, string? observacaoRecebimento = null, bool divergencia = false)
    {
        if (Status < StatusPedidoCompra.ConfirmadoPeloFornecedor || Status >= StatusPedidoCompra.RecebidoTotalmenteConcluido)
            throw new DomainException($"Não é possível registrar recebimento para pedido com status '{Status}'.");

        var item = _itens.FirstOrDefault(i => i.Id == itemPedidoDeCompraId);
        if (item == null)
            throw new DomainException("Item do pedido de compra não encontrado para registrar recebimento.");

        item.RegistrarRecebimento(quantidadeRecebida, observacaoRecebimento, divergencia);
        RecalcularValorTotalRecebido();

        // Verificar se todos os itens foram recebidos para mudar status do pedido principal
        if (_itens.All(i => i.Status == StatusItemPedidoCompra.Recebido || i.Status == StatusItemPedidoCompra.CanceladoPeloComprador || i.Status == StatusItemPedidoCompra.CanceladoPeloFornecedor))
        {
            Status = StatusPedidoCompra.RecebidoTotalmenteConcluido;
            DataEfetivaEntregaTotal = DateTime.UtcNow;
            AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status));
        } else if (Status != StatusPedidoCompra.RecebidoParcialmente && _itens.Any(i => i.Status == StatusItemPedidoCompra.Recebido || i.Status == StatusItemPedidoCompra.RecebidoComDivergencia))
        {
            Status = StatusPedidoCompra.RecebidoParcialmente;
            AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status));
        }
        // Se um item foi totalmente recebido, o evento ItemPedidoDeCompraRecebidoEvent já foi disparado por ele.
        // Se a compra do ItemOrçamento associado foi concluída, disparar evento.
        if (item.Status == StatusItemPedidoCompra.Recebido && item.ItemOrcamentoOrigemId.HasValue && item.QuantidadeRecebida >= item.QuantidadeSolicitada)
        {
            AdicionarEvento(new NotificarItemOrcamentoCompraConcluidaEvent(item.ItemOrcamentoOrigemId.Value, Id, item.Id));
        }
    }

    public void Cancelar(string motivo, Guid usuarioId)
    {
        if (Status >= StatusPedidoCompra.ConfirmadoPeloFornecedor && Status < StatusPedidoCompra.Cancelado)
        {
            // Regras mais complexas podem ser necessárias se o fornecedor já confirmou/enviou.
            // Pode exigir comunicação com o fornecedor.
            throw new DomainException($"Pedido de Compra com status '{Status}' pode ter restrições para cancelamento direto.");
        }
        if (Status == StatusPedidoCompra.Cancelado)
            return; // Já cancelado

        Status = StatusPedidoCompra.Cancelado;
        Observacoes = string.IsNullOrEmpty(Observacoes) ? $"Cancelado: {motivo}" : $"{Observacoes}\nCancelado: {motivo}";
        AdicionarEvento(new PedidoDeCompraStatusAlteradoEvent(Id, Status, usuarioId, motivo));
        // Notificar itens de orçamento para reverter status (se aplicável)
        foreach (var itemPc in _itens.Where(i => i.ItemOrcamentoOrigemId.HasValue && i.Status != StatusItemPedidoCompra.CanceladoPeloFornecedor))
        {
            AdicionarEvento(new NotificarItemOrcamentoCompraCanceladaEvent(itemPc.ItemOrcamentoOrigemId.Value, Id, itemPc.Id));
        }
    }
}