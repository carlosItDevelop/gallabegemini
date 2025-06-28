using GeneralLabSolutions.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings.Data.CRM
{
    public class ActivityMap : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Activities");

            builder.Property(x => x.Type).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Description).HasColumnType("text");
            builder.Property(x => x.ScheduledDate).IsRequired();
        }
    }
}