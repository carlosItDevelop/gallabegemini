using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class VendedorMap : IEntityTypeConfiguration<Vendedor>
    {
        public void Configure(EntityTypeBuilder<Vendedor> builder)
        {
            builder.HasKey(x => x.Id);

            // Configuração de índices para melhorar o desempenho de consultas em campos frequentemente utilizados
            builder.HasIndex(x => x.Nome).HasDatabaseName("IX_Vendedor_Nome");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Vendedor_Email");
            builder.HasIndex(x => x.Documento).IsUnique().HasDatabaseName("IX_Vendedor_Documento");

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

            builder.Property(x => x.StatusDoVendedor)
                .HasEnumConversion()
                .IsRequired();

            #region: Auditoria

            builder.Property(x => x.DataInclusao)
                .HasColumnName("DataInclusao")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.UsuarioInclusao)
                .HasColumnType("varchar(120)")
                .IsRequired();


            builder.Property(x => x.DataUltimaModificacao)
                .HasColumnName("DataUltimaModificacao")
                .HasColumnType("datetime")
                .IsRequired();


            builder.Property(x => x.UsuarioUltimaModificacao)
                .HasColumnType("varchar(120)")
                .IsRequired();

            #endregion


            builder.HasMany(x => x.Pedidos)
                .WithOne(x => x.Vendedor)
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Pessoa)
                   .WithOne() //  Vendedor  tem  uma  Pessoa,  mas  Pessoa  não  tem  Vendedor  diretamente
                   .HasForeignKey<Vendedor>(c => c.PessoaId);

            builder.ToTable("Vendedor");
        }
    }

}
