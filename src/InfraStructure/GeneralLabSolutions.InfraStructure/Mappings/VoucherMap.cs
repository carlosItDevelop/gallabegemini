using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VoucherMap : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        // Nome da tabela
        builder.ToTable("Voucher");

        // Chave primária (herdada de EntityBase)
        builder.HasKey(v => v.Id);

        builder.HasIndex(x => x.Codigo)
            .HasDatabaseName("IX_Voucher_Codigo");

        // Propriedade Codigo - string, pode ser nula
        builder.Property(v => v.Codigo)
            .HasColumnType("varchar(25)")
            .IsRequired();

        // Propriedade Percentual - decimal, pode ser nula
        builder.Property(v => v.Percentual)
            .HasColumnType("decimal(5, 2)")
            .IsRequired(false);

        // Propriedade ValorDesconto - decimal, pode ser nula
        builder.Property(v => v.ValorDesconto)
            .HasColumnType("decimal(18, 2)")
            .IsRequired(false);

        // Propriedade Quantidade - int, obrigatória
        builder.Property(v => v.Quantidade)
            .IsRequired();


        // Propriedade TipoDescontoVoucher - enum, gravar como string, obrigatória
        builder.Property(x => x.TipoDescontoVoucher)
            .HasEnumConversion()
            .IsRequired();

        // Propriedade DataCriacao - DateTime, obrigatória
        builder.Property(v => v.DataCriacao)
            .IsRequired();

        // Propriedade DataUtilizacao - DateTime, pode ser nula
        builder.Property(v => v.DataUtilizacao)
            .IsRequired(false);

        // Propriedade DataValidade - DateTime, obrigatória
        builder.Property(v => v.DataValidade)
            .IsRequired();

        // Propriedade Ativo - bool, obrigatória
        builder.Property(v => v.Ativo)
            .IsRequired();

        // Propriedade Utilizado - bool, obrigatória
        builder.Property(v => v.Utilizado)
            .IsRequired();

        // Relacionamento com a entidade Pedido (um Voucher pode ter vários Pedidos)
        builder.HasMany(v => v.Pedidos)
            .WithOne(p => p.Voucher)
            .HasForeignKey(p => p.VoucherId)
            .IsRequired(false) // Pode ser nulo, já que um pedido pode não ter voucher
            .OnDelete(DeleteBehavior.NoAction);


    }
}
