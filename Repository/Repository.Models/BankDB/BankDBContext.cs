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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(ConfigManage.AppSetting["ConnectionStrings:DefaultConnection"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.ToTable("CustomerAccount");

                entity.HasIndex(e => e.AccountNo, "IX_CustomerAccount_AccountNo");

                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(34);

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.IbanNo)
                    .IsRequired()
                    .HasMaxLength(34);

                entity.Property(e => e.IdCardPassport)
                    .IsRequired()
                    .HasMaxLength(20);

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
                entity.HasKey(e => new { e.EffectiveDate, e.FeeType });

                entity.ToTable("MasterFee");

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.Property(e => e.FeeType).HasMaxLength(1);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.FeePercent).HasColumnType("decimal(9, 2)");

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

                entity.Property(e => e.ActionBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.ActionType)
                    .IsRequired()
                    .HasMaxLength(1);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.FeeAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FeePercent).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.NetAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReferenceNo)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.ActionByNavigation)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.ActionBy)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Destination)
                    .WithMany(p => p.TransactionDestinations)
                    .HasForeignKey(d => d.DestinationId);

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.TransactionSources)
                    .HasForeignKey(d => d.SourceId);
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
