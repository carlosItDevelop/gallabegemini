using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using GeneralLabSolutions.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            // Chave primária
            builder.HasKey(x => x.Id);

            // Configuração da propriedade DataPedido
            builder.Property(x => x.DataPedido)
                .IsRequired()
                .HasColumnName("DataPedido")
                .HasColumnType("datetime");

            builder.Property(x => x.VoucherId)
                .IsRequired(false);

            // Configuração do enum StatusDoPedido como string
            builder.Property(x => x.StatusDoPedido)
                .HasEnumConversion()
                .IsRequired()
                .HasDefaultValue(StatusDoPedido.Orcamento);

            // Configuração do relacionamento Pedido 1:N ItensPedido
            builder.HasMany(x => x.Itens)
                .WithOne(x => x.Pedido)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento com Cliente
            builder.HasOne(x => x.Cliente)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento com Vendedor
            builder.HasOne(x => x.Vendedor)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento 1:N com HistoricoPedido
            builder.HasMany(p => p.Historico)
                .WithOne(h => h.Pedido)
                .HasForeignKey(h => h.PedidoId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade faz sentido aqui

            // Nome da tabela
            builder.ToTable("Pedido");
        }
    }
}