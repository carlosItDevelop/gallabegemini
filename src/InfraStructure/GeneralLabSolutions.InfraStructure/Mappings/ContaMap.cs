using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class ContaMap : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.DataVencimento).HasDatabaseName("IX_Conta_DataVencimento");
            builder.HasIndex(c => c.Documento).IsUnique().HasDatabaseName("IX_Conta_Documento");

            builder.Property(c => c.Instituicao)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Documento)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.DataVencimento)
                .IsRequired();

            builder.Property(c => c.Valor)
                .IsRequired()
                .HasColumnType("decimal(18, 2)"); // decimal para valores monetários

            builder.Property(c => c.TipoDeConta)
                .HasEnumConversion(); // Usa o extension method!

            builder.Property(c => c.EstaPaga)
                .IsRequired();

            builder.Property(c => c.DataPagamento);

            builder.Property(c => c.Observacao)
                .HasColumnType("varchar(500)");

            builder.Property(c => c.Inativa)
                .IsRequired();

            builder.ToTable("Contas");
        }
    }
}