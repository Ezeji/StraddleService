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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> entity)
        {
            entity.HasKey(e => e.TransactionId);

            entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");

            entity.Property(e => e.ConvertedTransactionAmount)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.ExchangeRate)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.GrossTransactionAmount)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TransactionAmount)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TransactionFee)
                .HasDefaultValueSql("((0.00))")
                .HasColumnType("decimal(19, 2)");

            entity.Property(e => e.TransactionStatus).HasDefaultValueSql("((1))");

            entity.Property(e => e.TransactionType).HasDefaultValueSql("((1))");
        }
    }
}
