using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace GeneralLabSolutions.InfraStructure.Mappings;
public class TelefoneMap : IEntityTypeConfiguration<Telefone>
{
    public void Configure(EntityTypeBuilder<Telefone> builder)
    {
        builder.HasKey(x => x.Id);

        // Configuração de índices para melhorar o desempenho de consultas em campos frequentemente utilizados
        builder.HasIndex(x => x.Numero).HasDatabaseName("IX_Telefone_Numero");
        builder.HasIndex(x => x.DDD).HasDatabaseName("IX_Telefone_DDD");
        builder.HasIndex(x => x.PessoaId).HasDatabaseName("IX_Telefone_PessoaId");

        builder.Property(x => x.PessoaId)
            .IsRequired();

        builder.Property(x => x.DDD)
            .IsRequired()
            .HasColumnType("varchar(3)");

        builder.Property(x => x.Numero)
            .IsRequired()
            .HasColumnType("varchar(15)");

        builder.Property(x => x.TipoDeTelefone)
            .HasEnumConversion()
            .IsRequired();

        builder.HasOne(t => t.Pessoa)
               .WithMany(p => p.Telefones) // Usa a nova coleção em Pessoa
               .HasForeignKey(t => t.PessoaId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade); // Excluir telefones se Pessoa for excluída

        builder.ToTable("Telefone");
    }
}

