using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class HistoricoItemMap : IEntityTypeConfiguration<HistoricoItem>
    {
        public void Configure(EntityTypeBuilder<HistoricoItem> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.ItemPedidoId)
                .IsRequired();

            builder.Property(h => h.DataHora)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(h => h.TipoEvento)
                .IsRequired()
                .HasColumnType("varchar(100)"); // Ajuste o tamanho conforme necessário

            builder.Property(h => h.StatusAnterior)
                .HasColumnType("varchar(50)"); // Ajuste o tamanho conforme necessário

            builder.Property(h => h.StatusNovo)
                .HasColumnType("varchar(50)"); // Ajuste o tamanho conforme necessário

            builder.Property(h => h.UsuarioId)
                .HasColumnType("varchar(255)"); // Ajuste o tamanho conforme necessário

            builder.Property(h => h.DadosExtras)
                .HasColumnType("NVARCHAR(MAX)"); // Ou JSON, se o seu banco de dados suportar

            // Relacionamento com ItemPedido
            builder.HasOne(h => h.ItemPedido)
                .WithMany(i => i.Historico)
                .HasForeignKey(h => h.ItemPedidoId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade é apropriado aqui, pois o histórico deve ser excluído se o item for excluído

            builder.ToTable("HistoricoItem");
        }
    }
}