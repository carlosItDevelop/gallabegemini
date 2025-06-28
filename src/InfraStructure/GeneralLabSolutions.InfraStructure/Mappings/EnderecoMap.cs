using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions; // Para HasEnumConversion, se aplicável
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static System.Formats.Asn1.AsnWriter;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class EnderecoMap : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PessoaId).IsRequired();

            builder.Property(e => e.PaisCodigoIso)
                .IsRequired()
                .HasMaxLength(2) // ISO 3166-1 alpha-2
                .HasColumnType("char(2)"); // char(2) é eficiente para códigos fixos de 2 letras

            builder.Property(e => e.LinhaEndereco1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.LinhaEndereco2)
                .HasMaxLength(200); // Opcional, então não IsRequired

            builder.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EstadoOuProvincia)
                .HasMaxLength(100); // Opcional

            builder.Property(e => e.CodigoPostal)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.InformacoesAdicionais)
                .HasMaxLength(500);

            // Mapeamento do Enum interno
            builder.Property(e => e.TipoDeEndereco)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(), // Converte enum para string para salvar no banco
                    v => (Endereco.TipoDeEnderecoEnum)Enum.Parse(typeof(Endereco.TipoDeEnderecoEnum), v) // Converte string do banco para enum
                )
                .HasMaxLength(20) // Ajuste o tamanho conforme o nome mais longo do seu enum
                .HasColumnType("varchar(20)"); // Ou use sua extensão HasEnumConversion se ela funcionar com enums internos

            // Relação 1:N com Pessoa
            builder.HasOne(e => e.Pessoa)
                   .WithMany(p => p.Enderecos)
                   .HasForeignKey(e => e.PessoaId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(e => e.PessoaId).HasDatabaseName("IX_Endereco_PessoaId");
            builder.HasIndex(e => e.PaisCodigoIso).HasDatabaseName("IX_Endereco_PaisCodigoIso");
            builder.HasIndex(e => e.CodigoPostal).HasDatabaseName("IX_Endereco_CodigoPostal");

            builder.ToTable("Endereco");
        }
    }
}