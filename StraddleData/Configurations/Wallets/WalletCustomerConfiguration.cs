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
    public class WalletCustomerConfiguration : IEntityTypeConfiguration<WalletCustomer>
    {
        public void Configure(EntityTypeBuilder<WalletCustomer> entity)
        {
            entity.HasKey(e => e.CustomerId);

            entity.Property(e => e.CustomerId).HasDefaultValueSql("(newid())");

            entity.Property(e => e.CustomerType).HasDefaultValueSql("((1))");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
        }
    }
}
