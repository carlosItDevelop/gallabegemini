using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class KanbanTaskMap : IEntityTypeConfiguration<KanbanTask>
    {
        public void Configure(EntityTypeBuilder<KanbanTask> builder)
        {
            // Chave primária
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Title).HasDatabaseName("IX_KanbanTask_Title");
            builder.HasIndex(x => x.Description).HasDatabaseName("IX_KanbanTask_Description");


            builder.Property(x => x.Title)
                .IsRequired()
                .HasColumnType("varchar(80)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasColumnType("varchar(150)");



            builder.Property(x => x.Column)
                .HasEnumConversion()
                .IsRequired();

            builder.Property(x => x.Priority)
                .HasEnumConversion()
                .IsRequired();


            builder.HasMany(p => p.Participantes)
                .WithMany(k => k.Tasks);

            // Nome da tabela
            builder.ToTable("KanbanTask");
        }
    }
}

