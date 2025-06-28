using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class ParticipanteMap : IEntityTypeConfiguration<Participante>
    {
        public void Configure(EntityTypeBuilder<Participante> builder)
        {
            // Chave primária
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("IX_Participante_Name");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Participante_Email");


            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(254)");


            builder.HasMany(p => p.Tasks)
                .WithMany(k => k.Participantes);

            // Relecionamento feito em AgendaEventos

            // Nome da tabela
            builder.ToTable("Participante");
        }
    }
}

