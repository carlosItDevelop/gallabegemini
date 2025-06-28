using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities.CRM;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GeneralLabSolutions.InfraStructure.Extensions;

namespace GeneralLabSolutions.InfraStructure.Data.CRM
{
    public class LeadMap : IEntityTypeConfiguration<Lead>
    {
        public void Configure(EntityTypeBuilder<Lead> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Leads"); // Nome da tabela

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Company).HasMaxLength(255);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Phone).HasMaxLength(50);
            builder.Property(x => x.Position).HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Source).HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.Responsible).HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Score).IsRequired();
            builder.Property(x => x.Value).IsRequired().HasColumnType("decimal(10, 2)");
            builder.Property(x => x.Notes).HasColumnType("text"); // Adequado para textos longos
            builder.Property(x => x.LastContact).HasColumnType("date");

            // Mapeamento dos Enums usando seu helper
            builder.Property(x => x.Status).HasEnumConversion().HasMaxLength(50).HasColumnType("varchar(50)");
            builder.Property(x => x.Temperature).HasEnumConversion().HasMaxLength(20).HasColumnType("varchar(20)");

            // Relacionamentos: Um Lead tem muitas Tasks, Activities, etc.
            builder.HasMany(l => l.Tasks).WithOne(t => t.Lead).HasForeignKey(t => t.LeadId);
            builder.HasMany(l => l.Activities).WithOne(a => a.Lead).HasForeignKey(a => a.LeadId).IsRequired(false); // Permite nulo
            builder.HasMany(l => l.LeadNotes).WithOne(n => n.Lead).HasForeignKey(n => n.LeadId);
            builder.HasMany(l => l.Logs).WithOne(log => log.Lead).HasForeignKey(log => log.LeadId).IsRequired(false); // Permite nulo
        }
    }
}
