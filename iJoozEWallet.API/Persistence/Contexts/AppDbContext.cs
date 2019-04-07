using System;
using iJoozEWallet.API.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace iJoozEWallet.API.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<EWallet> EWallet { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            initialBalanceTable(builder);

            initialTopUpHistoryTable(builder);

            initialDeductHistoryTable(builder);
        }


        private void initialBalanceTable(ModelBuilder builder)
        {
            builder.Entity<EWallet>().ToTable("EWallet");
            builder.Entity<EWallet>().HasKey(p => p.UserId);
            builder.Entity<EWallet>().Property(p => p.UserId).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<EWallet>().Property(p => p.Balance).IsRequired();
            builder.Entity<EWallet>().Property(p => p.LastUpdateDate).IsRequired();

            builder.Entity<EWallet>()
                .HasMany(p => p.TopUpHistories)
                .WithOne(p => p.EWallet)
                .HasForeignKey(p => p.UserId);

            builder.Entity<EWallet>()
                .HasMany(p => p.DeductHistories)
                .WithOne(p => p.EWallet)
                .HasForeignKey(p => p.UserId);

            builder.Entity<EWallet>().HasData
            (
                new EWallet {UserId = 100, Balance = 100, LastUpdateDate = DateTime.Now},
                new EWallet {UserId = 101, Balance = 10, LastUpdateDate = DateTime.Now}
            );
        }

        private void initialTopUpHistoryTable(ModelBuilder builder)
        {
            builder.Entity<TopUpHistory>().ToTable("TopUpHistory");
            builder.Entity<TopUpHistory>().HasKey(p => p.Id);
            builder.Entity<TopUpHistory>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<TopUpHistory>().Property(p => p.UserId).IsRequired();
            builder.Entity<TopUpHistory>().Property(p => p.TopUpCredit).IsRequired();
            builder.Entity<TopUpHistory>().Property(p => p.TopUpSource).IsRequired();
            builder.Entity<TopUpHistory>().Property(p => p.ActionDate).IsRequired();
        }

        private void initialDeductHistoryTable(ModelBuilder builder)
        {
            builder.Entity<DeductHistory>().ToTable("DeductHistory");
            builder.Entity<DeductHistory>().HasKey(p => p.Id);
            builder.Entity<DeductHistory>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<DeductHistory>().Property(p => p.UserId).IsRequired();
            builder.Entity<DeductHistory>().Property(p => p.DeductCredit).IsRequired();
            builder.Entity<DeductHistory>().Property(p => p.DeductSource).IsRequired();
            builder.Entity<DeductHistory>().Property(p => p.ActionDate).IsRequired();
        }
    }
}