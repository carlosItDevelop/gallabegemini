using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Data.CRM
{
    public class LogMap : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Logs");
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Description).HasColumnType("text");
            builder.Property(x => x.Timestamp).IsRequired();
            builder.Property(x => x.UserId).HasMaxLength(255).HasColumnType("varchar(255)");
        }
    }
}
