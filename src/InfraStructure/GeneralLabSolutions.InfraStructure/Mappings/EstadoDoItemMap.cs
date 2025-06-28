using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class EstadoDoItemMap : IEntityTypeConfiguration<EstadoDoItem>
    {
        public void Configure(EntityTypeBuilder<EstadoDoItem> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.StatusDoItemId)
                .IsRequired();

            builder.Property(e => e.DataAlteracao)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(e => e.Ativo)
                .IsRequired();

            builder.Property(e => e.DadosExtras)
                .HasColumnType("NVARCHAR(MAX)"); // Ou JSON, se o seu banco de dados suportar

            // Relacionamento com ItemPedido (agora 1:N)
            builder.HasOne(e => e.ItemPedido)
                .WithMany(i => i.Estados)
                .HasForeignKey(e => e.ItemPedidoId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar exclusão em cascata

            // Relacionamento com StatusDoItem
            builder.HasOne(e => e.StatusDoItem)
                .WithMany() // Pode adicionar uma propriedade de navegação em StatusDoItem se necessário
                .HasForeignKey(e => e.StatusDoItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("EstadoDoItem");
        }
    }
}