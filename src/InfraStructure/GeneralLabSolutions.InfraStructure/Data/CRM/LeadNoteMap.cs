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
    public class LeadNoteMap : IEntityTypeConfiguration<LeadNote>
    {
        public void Configure(EntityTypeBuilder<LeadNote> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("LeadNotes");
            builder.Property(x => x.Content).IsRequired().HasColumnType("text");
            builder.Property(x => x.Color).IsRequired().HasMaxLength(20).HasColumnType("varchar(20)");
            builder.Property(x => x.UserId).HasMaxLength(255).HasColumnType("varchar(255)");
        }
    }
}
