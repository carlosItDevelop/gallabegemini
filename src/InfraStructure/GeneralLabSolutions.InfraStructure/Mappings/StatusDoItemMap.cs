using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class StatusDoItemMap : IEntityTypeConfiguration<StatusDoItem>
    {
        public void Configure(EntityTypeBuilder<StatusDoItem> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Descricao)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(100); // Defina um tamanho máximo adequado

            builder.Property(s => s.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            // Configuração do relacionamento 1:N com StatusDoItemIncompativel
            builder.HasMany(s => s.Incompatibilidades)
                .WithOne(i => i.StatusDoItem)
                .HasForeignKey(i => i.StatusDoItemId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar exclusão em cascata

            builder.ToTable("StatusDoItem");
        }
    }
}