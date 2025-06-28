using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.InfraStructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class DadosBancariosMap : IEntityTypeConfiguration<DadosBancarios>
    {
        public void Configure(EntityTypeBuilder<DadosBancarios> builder)
        {
            builder.HasKey(x => x.Id);
            // Configuração de índices para melhorar o desempenho de consultas em campos frequentemente utilizados
            builder.HasIndex(x => x.Banco).HasDatabaseName("IX_DadosBancarios_Banco");
            builder.HasIndex(x => x.Agencia).HasDatabaseName("IX_DadosBancarios_Agencia");
            builder.HasIndex(x => x.Conta).HasDatabaseName("IX_DadosBancarios_Conta");
            builder.HasIndex(x => x.PessoaId).HasDatabaseName("IX_DadosBancarios_PessoaId");
            builder.HasIndex(x => x.TipoDeContaBancaria).HasDatabaseName("IX_DadosBancarios_TipoDeContaBancaria");

            builder.Property(x => x.Banco)
                .IsRequired()
                .HasColumnType("varchar(100)");
            builder.Property(x => x.Agencia)
                .IsRequired()
                .HasColumnType("varchar(20)");
            builder.Property(x => x.Conta)
                .IsRequired()
                .HasColumnType("varchar(20)");
            builder.Property(x => x.TipoDeContaBancaria)
                .IsRequired()
                .HasEnumConversion();

            builder.HasOne(d => d.Pessoa)                   // DadosBancarios tem uma Pessoa
                   .WithMany(p => p.DadosBancarios)         // Pessoa tem muitos DadosBancarios
                   .HasForeignKey(d => d.PessoaId)          // A chave estrangeira é PessoaId em DadosBancarios
                   .IsRequired()                            // Uma pertencer a alguém.
                   .OnDelete(DeleteBehavior.Cascade);       // DECISÃO: Excluir DadosBancarios quando Pessoa for excluída? Cascade é comum aqui. Ou usar Restrict/NoAction se preferir.


            builder.ToTable("DadosBancarios");
        }
    }
}
