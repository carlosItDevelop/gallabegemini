using GeneralLabSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralLabSolutions.InfraStructure.Mappings
{
    public class CalendarEventMap : IEntityTypeConfiguration<CalendarEvent>
    {
        public void Configure(EntityTypeBuilder<CalendarEvent> builder)
        {
            builder.ToTable("CalendarEvents"); // Nome da tabela

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Start)
                .IsRequired()
                .HasColumnType("datetime2"); // Usar datetime2 para maior precisão

            builder.Property(e => e.End)
                .HasColumnType("datetime2"); // Nullable por padrão

            builder.Property(e => e.AllDay)
                .IsRequired();

            builder.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar(50)"); // varchar é apropriado aqui

            builder.Property(e => e.Location)
                .HasMaxLength(250);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            // --- Se for Multi-Tenant (descomente e ajuste se necessário) ---
            // builder.Property(e => e.UserId)
            //    .HasMaxLength(450); // Ou o tamanho do seu campo de ID de usuário
            //
            // builder.HasIndex(e => e.UserId);
            //
            // builder.HasOne<ApplicationUser>() // Substitua ApplicationUser pelo seu tipo de usuário
            //    .WithMany() // Ou .WithMany(u => u.CalendarEvents) se tiver coleção no usuário
            //    .HasForeignKey(e => e.UserId)
            //    .OnDelete(DeleteBehavior.Cascade); // Ou Restrict, dependendo da regra de negócio
            // ---------------------------------------------------------------
        }
    }
}