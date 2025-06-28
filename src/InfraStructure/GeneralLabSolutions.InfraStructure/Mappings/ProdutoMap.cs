using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(x => x.Id);

            // Configuração de índices para melhorar o desempenho de consultas em campos frequentemente utilizados
            builder.HasIndex(x => x.Descricao).IsUnique().HasDatabaseName("IX_Produto_Descricao");
            builder.HasIndex(x => x.Codigo).IsUnique().HasDatabaseName("IX_Produto_Codigo");
            builder.HasIndex(x => x.Ncm).IsUnique().HasDatabaseName("IX_Produto_Ncm");

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasColumnType("varchar(600)")
                .HasComment("Descrição do produto")
                .HasMaxLength(600);

            builder.Property(x => x.Codigo)
                .IsRequired()
                .HasColumnType("varchar(20)");

            builder.Property(x => x.Ncm)
                .IsRequired()
                .HasColumnName("NCM")
                .HasColumnType("varchar(15)");

            builder.Property(x => x.ValorUnitario)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasColumnName("ValorUnitario");

            builder.Property(x => x.DataDeValidade)
                .IsRequired();

            builder.Property(x => x.Imagem)
                .IsRequired()
                .HasColumnType("varchar")
                .HasMaxLength(300);

            builder.Property(x => x.StatusDoProduto)
                .HasEnumConversion()
                .HasDefaultValue(StatusDoProduto.Dropshipping)
                .IsRequired();

            // Relacionamento Produto N:1 Categoria
            builder.HasOne(x => x.CategoriaProduto)
                .WithMany(x => x.Produtos)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Produto N:1 Fornecedor
            builder.HasOne(x => x.Fornecedor)
                .WithMany(x => x.Produtos)
                .HasForeignKey(x => x.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Produto");
        }
    }
}
