SELECT 
    YEAR(DataPedido) AS Ano,
    MONTH(DataPedido) AS Mes,
    StatusDoPedido,
    COUNT(*) AS QuantidadePedidos,
    SUM(ItemPedido.ValorUnitario * ItemPedido.Quantidade) AS ValorTotal
FROM 
    Pedido
JOIN 
    ItemPedido ON Pedido.Id = ItemPedido.PedidoId
GROUP BY 
    YEAR(DataPedido), 
    MONTH(DataPedido), 
    StatusDoPedido
ORDER BY 
    Ano DESC,
    Mes DESC,
    StatusDoPedido;
