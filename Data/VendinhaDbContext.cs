using Microsoft.EntityFrameworkCore;
using VendinhaBackend.Models;

namespace VendinhaBackend.Data
{
    public class VendinhaDbContext : DbContext
    {
        public VendinhaDbContext(DbContextOptions<VendinhaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Divida> Dividas => Set<Divida>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var modelCliente = modelBuilder.Entity<Cliente>();
            modelCliente.ToTable("Clientes");
            modelCliente.Property(e => e.Id).HasColumnName("Id");
            modelCliente.Property(e => e.NomeCompleto).HasColumnName("NomeCompleto");
            modelCliente.Property(e => e.Cpf).HasColumnName("Cpf");
            modelCliente.Property(e => e.DataNascimento).HasColumnName("DataNascimento");
            modelCliente.Property(e => e.Email).HasColumnName("Email");
            modelCliente.HasKey(e => e.Id);
            modelCliente.HasIndex(e => e.Cpf).IsUnique();

            var modelDivida = modelBuilder.Entity<Divida>();
            modelDivida.ToTable("Dividas");
            modelDivida.Property(e => e.Id).HasColumnName("Id");
            modelDivida.Property(e => e.ClienteId).HasColumnName("ClienteId");
            modelDivida.Property(e => e.Valor).HasColumnName("Valor");
            modelDivida.Property(e => e.Situacao).HasColumnName("Situacao");
            modelDivida.Property(e => e.DataCriacao).HasColumnName("DataCriacao");
            modelDivida.Property(e => e.DataPagamento).HasColumnName("DataPagamento");
            modelDivida.HasKey(e => e.Id);
            modelDivida
                .HasOne(e => e.Cliente)
                .WithMany(e => e.Dividas)
                .HasForeignKey(e => e.ClienteId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
