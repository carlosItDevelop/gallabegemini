using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ItemPedidoMap : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.HasKey(x => x.Id);

        // Configuração de índices para melhorar o desempenho
        // de consultas em campos frequentemente utilizados
        builder.HasIndex(x => x.NomeDoProduto)
            .HasDatabaseName("IX_ItemPedido_NomeDoProduto");

        builder.Property(x => x.NomeDoProduto) // Descrição em Produto
                .IsRequired()
                .HasColumnType("varchar(600)")
                .HasComment("Descrição do produto")
                .HasMaxLength(600);

        builder.Property(x => x.Quantidade)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(x => x.ValorUnitario)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.PedidoId)
            .IsRequired();

        builder.Property(x => x.ProdutoId)
            .IsRequired();

        // Configurando a FK para Pedido
        builder.HasOne(x => x.Pedido)
            .WithMany(p => p.Itens)
            .HasForeignKey(x => x.PedidoId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configurando a FK para Produto
        builder.HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento com EstadoDoItem (1:N)
        builder.HasMany(i => i.Estados)
               .WithOne(e => e.ItemPedido)
               .HasForeignKey(e => e.ItemPedidoId)
               .OnDelete(DeleteBehavior.Restrict); // Evitar exclusão em cascata de ItemPedido se houver Estados associados

        // Relacionamento com HistoricoItem (1:N)
        builder.HasMany(i => i.Historico)
               .WithOne(h => h.ItemPedido)
               .HasForeignKey(h => h.ItemPedidoId)
               .OnDelete(DeleteBehavior.Cascade); // Cascade pode ser apropriado aqui, para excluir o histórico se o item for excluído

        builder.ToTable("ItemPedido");
    }
}