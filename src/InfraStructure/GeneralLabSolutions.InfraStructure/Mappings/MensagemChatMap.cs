using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class MensagemChatMap : IEntityTypeConfiguration<MensagemChat>
    {
        public void Configure(EntityTypeBuilder<MensagemChat> builder)
        {
            builder.ToTable("MensagensChat");

            builder.Property(p => p.Conteudo)
                   .IsRequired()
                   .HasColumnType("nvarchar(max)");   // 🔑 aqui está a cura do truncamento
        }
    }
}