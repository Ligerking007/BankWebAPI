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
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_CI_AS");

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(e => e.Ibanno)
                    .HasName("PK_Account");

                entity.ToTable("CustomerAccount");

                entity.Property(e => e.Ibanno)
                    .HasMaxLength(34)
                    .HasColumnName("IBANNo");

                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(35);

                entity.Property(e => e.Idcard)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("IDCard");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CustomerAccounts)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Accounts_User_CreatedBy");
            });

            modelBuilder.Entity<MasterFee>(entity =>
            {
                entity.HasKey(e => e.EffectiveDate);

                entity.ToTable("MasterFee");

                entity.Property(e => e.EffectiveDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedTime).HasColumnType("datetime");

                entity.Property(e => e.FeePer)
                    .HasColumnType("decimal(9, 2)")
                    .HasColumnName("Fee_Per");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasMaxLength(20);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
