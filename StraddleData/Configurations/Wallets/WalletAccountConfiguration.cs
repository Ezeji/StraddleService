using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StraddleData.Models.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleData.Configurations.Wallets
{
    public class WalletAccountConfiguration : IEntityTypeConfiguration<WalletAccount>
    {
        public void Configure(EntityTypeBuilder<WalletAccount> entity)
        {
            entity.HasKey(e => e.AccountId);

            entity.Property(e => e.AccountId).HasDefaultValueSql("(newid())");

            entity.Property(e => e.AccountStatus).HasDefaultValueSql("((1))");

            entity.Property(e => e.BankAccountCurrency).HasDefaultValueSql("((1))");

            entity.Property(e => e.BankAccountTierLevel).HasDefaultValueSql("((1))");

            entity.Property(e => e.BankAccountType).HasDefaultValueSql("((1))");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Discount)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.LedgerBalance).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.LienAmount).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TotalCredits).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TotalDebits).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TotalLedgerCredits).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TotalLedgerDebits).HasColumnType("decimal(19, 2)");

            entity.Property(e => e.WalletBalance).HasColumnType("decimal(19, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.WalletAccounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WalletAcc__Custo__32E0915F");
        }
    }
}
