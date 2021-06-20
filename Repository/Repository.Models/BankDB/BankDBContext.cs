using System;
using Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models.BankDB
{
    public partial class BankDBContext : DbContext
    {
        public BankDBContext()
        {
        }

        public BankDBContext(DbContextOptions<BankDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public virtual DbSet<MasterFee> MasterFees { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigManage.AppSetting["ConnectionStrings:DefaultConnection"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_CI_AS");

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(e => e.AccountNo);

                entity.ToTable("CustomerAccount");

                entity.HasIndex(e => e.AccountNo, "IX_CustomerAccount_AccountNo");

                entity.Property(e => e.AccountNo).HasMaxLength(34);

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Ibanno)
                    .IsRequired()
                    .HasMaxLength(34)
                    .HasColumnName("IBANNo");

                entity.Property(e => e.IsActived)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModifiedBy).HasMaxLength(20);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CustomerAccounts)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MasterFee>(entity =>
            {
                entity.HasKey(e => e.EffectiveDate);

                entity.ToTable("MasterFee");

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.FeePercent).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.FeeType)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.ModifiedBy).HasMaxLength(20);

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.MasterFees)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.HasIndex(e => e.ActionType, "IX_Transaction_ActionType");

                entity.HasIndex(e => e.ReferenceNo, "IX_Transaction_ReferenceNo");

                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(34);

                entity.Property(e => e.ActionBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.ActionType)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.DestinationNo).HasMaxLength(34);

                entity.Property(e => e.FeeAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FeePercent).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReferenceNo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.AccountNoNavigation)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AccountNo)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ActionByNavigation)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.ActionBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasMaxLength(20);

                entity.Property(e => e.CreatedBy).HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ModifiedBy).HasMaxLength(20);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
