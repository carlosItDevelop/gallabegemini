using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class FornecedorMap : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {

            builder.HasKey(x => x.Id);

            // Configuração de índices para melhorar o desempenho de consultas em campos frequentemente utilizados
            builder.HasIndex(x => x.Nome).HasDatabaseName("IX_Fornecedor_Nome");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Fornecedor_Email");
            builder.HasIndex(x => x.Documento).IsUnique().HasDatabaseName("IX_Fornecedor_Documento");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Documento)
                .IsRequired()
                .HasColumnType("varchar(14)"); // CPF ou CNPJ

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(254)");

            builder.Property(x => x.TipoDePessoa)
                .HasEnumConversion()
                .IsRequired();


            builder.Property(x => x.StatusDoFornecedor)
                .HasEnumConversion()
                .IsRequired();


            builder.HasMany(x => x.Produtos)
                .WithOne(x => x.Fornecedor)
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Pessoa)
                   .WithOne() //  Fornecedor  tem  uma  Pessoa,  mas  Pessoa  não  tem  Fornecedor  diretamente
                   .HasForeignKey<Fornecedor>(c => c.PessoaId);

            builder.ToTable("Fornecedor");
        }
    }
}
