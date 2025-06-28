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
    public class CrmTaskAttachmentMap : IEntityTypeConfiguration<CrmTaskAttachment>
    {
        public void Configure(EntityTypeBuilder<CrmTaskAttachment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("CrmTaskAttachments");
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.FileUrl).IsRequired().HasColumnType("text");
            builder.Property(x => x.MimeType).IsRequired().HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.UploadedBy).HasMaxLength(255).HasColumnType("varchar(255)");
        }
    }
}
