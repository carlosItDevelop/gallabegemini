using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(x => x.Id);


            builder.HasIndex(x => x.Nome).HasDatabaseName("IX_Cliente_Nome");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Cliente_Email");
            builder.HasIndex(x => x.Documento).IsUnique().HasDatabaseName("IX_Cliente_Documento");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Documento)
                .IsRequired()
                .HasColumnType("varchar(14)"); // CPF ou CNPJ

            builder.Property(x => x.TipoDePessoa)
                .HasEnumConversion()
                .IsRequired();


            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("varchar(254)");

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


            builder.Property(x => x.InscricaoEstadual)
                .HasColumnType("varchar(20)")
                .IsRequired(false);

            builder.Property(x => x.Observacao)
                .HasColumnType("varchar(4000)")
                .IsRequired(false);


            builder.Property(x => x.StatusDoCliente)
                .HasEnumConversion()
                .IsRequired();


            builder.Property(x => x.TipoDeCliente)
                .HasEnumConversion()
                .IsRequired();

            builder.Property(x => x.TelefonePrincipal)
                .HasColumnType("varchar(15)")
                .IsRequired();


            builder.Property(x => x.ContatoRepresentante)
                   .HasColumnType("varchar(50)")
                   .IsRequired(false); // Explicitamente definido como não obrigatório no banco



            builder.HasMany(x => x.Pedidos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Pessoa) // PONTO DE ATENÇÃO!
                   .WithOne() //  Cliente  tem  uma  Pessoa,  mas  Pessoa  não  tem  Cliente  diretamente
                   .HasForeignKey<Cliente>(c => c.PessoaId);

            builder.ToTable("Cliente");
        }
    }
}
