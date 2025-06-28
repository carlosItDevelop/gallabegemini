using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.InfraStructure.Mappings.Data.CRM
{
    public class CrmTaskMap : IEntityTypeConfiguration<CrmTask>
    {
        public void Configure(EntityTypeBuilder<CrmTask> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("CrmTasks");

            builder.Property(x => x.Title).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Description).HasColumnType("text");
            builder.Property(x => x.DueDate).HasColumnType("date");
            builder.Property(x => x.Assignee).HasMaxLength(255).HasColumnType("varchar(255)");

            builder.Property(x => x.Priority).HasEnumConversion().HasMaxLength(20).HasColumnType("varchar(20)");
            builder.Property(x => x.Status).HasEnumConversion().HasMaxLength(20).HasColumnType("varchar(20)");

            // Relacionamentos
            builder.HasMany(t => t.Comments).WithOne(c => c.CrmTask).HasForeignKey(c => c.CrmTaskId);
            builder.HasMany(t => t.Attachments).WithOne(a => a.CrmTask).HasForeignKey(a => a.CrmTaskId);
        }
    }
}