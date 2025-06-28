using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class CategoriaProdutoMap : IEntityTypeConfiguration<CategoriaProduto>
    {
        public void Configure(EntityTypeBuilder<CategoriaProduto> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Descricao).IsUnique().HasDatabaseName("IX_Cliente_Descricao");

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasColumnType("varchar(200)")
                .HasComment("Família do produto");

            // ProdutoCategoria 1:N Produto
            builder.HasMany(x => x.Produtos)
                .WithOne(x => x.CategoriaProduto)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict); // Evita que a exclusão de uma categoria exclua todos os produtos relacionados			
        }
    }
}
