using GeneralLabSolutions.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Data.CRM
{
    public class CrmTaskCommentMap : IEntityTypeConfiguration<CrmTaskComment>
    {
        public void Configure(EntityTypeBuilder<CrmTaskComment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("CrmTaskComments");
            builder.Property(x => x.Content).IsRequired().HasColumnType("text");
            builder.Property(x => x.UserId).HasMaxLength(255).HasColumnType("varchar(255)");
        }
    }
}
