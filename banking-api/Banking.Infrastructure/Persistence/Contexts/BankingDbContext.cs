using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Banking.Infrastructure.Persistence.Contexts
{
    public class BankingDbContext : DbContext
    {
        // El constructor recibe las opciones de configuración (cadena de conexión, etc.)
        public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options)
        {
        }

        // Representación de tus tablas en C#
        public DbSet<Person> People { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransactionBank> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí podemos forzar configuraciones extra si las Data Annotations no son suficientes.
            // Por ejemplo, asegurar la relación 1 a 1 entre Person y Customer.

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Person)
                .WithOne(p => p.Customer)
                .HasForeignKey<Customer>(c => c.PersonId);

            // Configuración para decimales (Importante para bancos)
            modelBuilder.Entity<Account>()
                .Property(a => a.InitialBalance)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TransactionBank>()
                .Property(t => t.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TransactionBank>()
                .Property(t => t.Balance)
                .HasPrecision(10, 2);
        }
    }
}
