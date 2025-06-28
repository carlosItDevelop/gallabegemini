SELECT 
    StatusDoPedido AS Status, 
    COUNT(*) AS TotalDesteStatus
FROM 
    Pedido
GROUP BY 
    StatusDoPedido;
