using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class StatusDoItemIncompativelMap : IEntityTypeConfiguration<StatusDoItemIncompativel>
    {
        public void Configure(EntityTypeBuilder<StatusDoItemIncompativel> builder)
        {
            builder.HasKey(i => i.Id);

            // Relacionamento com StatusDoItem (para o status "base")
            builder.HasOne(i => i.StatusDoItem)
                .WithMany(s => s.Incompatibilidades)
                .HasForeignKey(i => i.StatusDoItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com StatusDoItem (para o status "incompatível")
            builder.HasOne(i => i.StatusDoItemIncompativelNavigation)
                .WithMany() // Sem propriedade de navegação inversa em StatusDoItem
                .HasForeignKey(i => i.StatusDoItemIncompativelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("StatusDoItemIncompativel");
        }
    }
}