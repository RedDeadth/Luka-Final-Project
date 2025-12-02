using FinalProject.Domain.Entities;
using System;
using System.Collections.Generic;
using FinalProject.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace FinalProject.Infrastructure.Data;

public partial class LukitasDbContext : DbContext
{
    public LukitasDbContext()
    {
    }

    public LukitasDbContext(DbContextOptions<LukitasDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<MissionTemplate> MissionTemplates { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierType> SupplierTypes { get; set; }

    public virtual DbSet<Transfer> Transfers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserMission> UserMissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback - should use configuration from appsettings.json
            optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=lukitas_db;user=lukitas_user;password=lukitas123;AllowPublicKeyRetrieval=True;SslMode=Preferred", 
                new MySqlServerVersion(new Version(9, 2, 0)));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("accounts");

            entity.HasIndex(e => e.AccountNumber, "account_number").IsUnique();

            entity.HasIndex(e => e.CampaignId, "idx_accounts_campaign");

            entity.HasIndex(e => e.UserId, "idx_accounts_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .HasColumnName("account_number");
            entity.Property(e => e.Balance)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("balance");
            entity.Property(e => e.CampaignId).HasColumnName("campaign_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("fk_accounts_campaigns");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_accounts_users");
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("achievements", tb => tb.HasComment("Achievements obtained by completing missions"));

            entity.HasIndex(e => e.UserMissionId, "fk_achievements_user_missions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AchievementDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("achievement_date");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.UserMissionId).HasColumnName("user_mission_id");

            entity.HasOne(d => d.UserMission).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.UserMissionId)
                .HasConstraintName("fk_achievements_user_missions");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("campaigns", tb => tb.HasComment("Campaigns promoted by coordinators"));

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_campaigns_dates");

            entity.HasIndex(e => e.UserId, "idx_campaigns_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Budget)
                .HasPrecision(15, 2)
                .HasColumnName("budget");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(10)
                .HasColumnName("contact_number");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Location)
                .HasMaxLength(200)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Schedule)
                .HasMaxLength(100)
                .HasColumnName("schedule");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_campaigns_users");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("coupons");

            entity.HasIndex(e => e.Code, "code").IsUnique();

            entity.HasIndex(e => e.CampaignId, "fk_coupons_campaigns");

            entity.HasIndex(e => e.SupplierId, "fk_coupons_suppliers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CampaignId).HasColumnName("campaign_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.DiscountType)
                .HasColumnType("enum('percentage','fixed_amount')")
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(10, 2)
                .HasColumnName("discount_value");
            entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.Campaign).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.CampaignId)
                .HasConstraintName("fk_coupons_campaigns");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_coupons_suppliers");
        });

        modelBuilder.Entity<MissionTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mission_templates", tb => tb.HasComment("Master catalog of system missions"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.RewardPoints)
                .HasDefaultValueSql("'0'")
                .HasColumnName("reward_points");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.Code, "code").IsUnique();

            entity.HasIndex(e => e.SupplierId, "idx_products_supplier");

            entity.HasIndex(e => e.ProductTypeId, "idx_products_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(12, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductTypeId).HasColumnName("product_type_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");
            entity.Property(e => e.Stock)
                .HasDefaultValueSql("'0'")
                .HasColumnName("stock");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .HasConstraintName("fk_products_types");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_products_suppliers");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles", tb => tb.HasComment("System roles: student and coordinator"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Permissions)
                .HasMaxLength(500)
                .HasColumnName("permissions");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sales", tb => tb.HasComment("sale_id serves as voucher code"));

            entity.HasIndex(e => e.AccountId, "idx_sales_account");

            entity.HasIndex(e => e.SaleDate, "idx_sales_date");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("sale_date");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValueSql("'pending'")
                .HasColumnName("status");
            entity.Property(e => e.Total)
                .HasPrecision(15, 2)
                .HasColumnName("total");

            entity.HasOne(d => d.Account).WithMany(p => p.Sales)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("fk_sales_accounts");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sale_details");

            entity.HasIndex(e => e.ProductId, "fk_sale_details_products");

            entity.HasIndex(e => e.SaleId, "fk_sale_details_sales");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.Subtotal)
                .HasPrecision(15, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(12, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_sale_details_products");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("fk_sale_details_sales");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("suppliers");

            entity.HasIndex(e => e.SupplierTypeId, "fk_suppliers_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");
            entity.Property(e => e.SupplierTypeId).HasColumnName("supplier_type_id");

            entity.HasOne(d => d.SupplierType).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.SupplierTypeId)
                .HasConstraintName("fk_suppliers_types");
        });

        modelBuilder.Entity<SupplierType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("supplier_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transfers");

            entity.HasIndex(e => e.DestinationAccountId, "fk_transfers_destination");

            entity.HasIndex(e => e.SourceAccountId, "fk_transfers_source");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2)
                .HasColumnName("amount");
            entity.Property(e => e.DestinationAccountId).HasColumnName("destination_account_id");
            entity.Property(e => e.SourceAccountId).HasColumnName("source_account_id");
            entity.Property(e => e.Status)
                .HasMaxLength(15)
                .HasDefaultValueSql("'pending'")
                .HasColumnName("status");
            entity.Property(e => e.TransferDate).HasColumnName("transfer_date");

            entity.HasOne(d => d.DestinationAccount).WithMany(p => p.TransferDestinationAccounts)
                .HasForeignKey(d => d.DestinationAccountId)
                .HasConstraintName("fk_transfers_destination");

            entity.HasOne(d => d.SourceAccount).WithMany(p => p.TransferSourceAccounts)
                .HasForeignKey(d => d.SourceAccountId)
                .HasConstraintName("fk_transfers_source");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users", tb => tb.HasComment("Educational system users"));

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.RoleId, "idx_users_role");

            entity.HasIndex(e => e.StudentCode, "idx_users_student_code").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Company)
                .HasMaxLength(100)
                .HasColumnName("company");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StudentCode)
                .HasMaxLength(20)
                .HasColumnName("student_code");
            entity.Property(e => e.University)
                .HasMaxLength(100)
                .HasColumnName("university");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_users_roles");
        });

        modelBuilder.Entity<UserMission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_missions", tb => tb.HasComment("Missions assigned to users with their status"));

            entity.HasIndex(e => e.SaleId, "fk_user_missions_sales");

            entity.HasIndex(e => e.Completed, "idx_user_missions_completed");

            entity.HasIndex(e => e.MissionId, "idx_user_missions_template");

            entity.HasIndex(e => e.UserId, "idx_user_missions_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignmentDate)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("assignment_date");
            entity.Property(e => e.Completed)
                .HasDefaultValueSql("'0'")
                .HasColumnName("completed");
            entity.Property(e => e.CompletionDate).HasColumnName("completion_date");
            entity.Property(e => e.MissionId).HasColumnName("mission_id");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Mission).WithMany(p => p.UserMissions)
                .HasForeignKey(d => d.MissionId)
                .HasConstraintName("fk_user_missions_templates");

            entity.HasOne(d => d.Sale).WithMany(p => p.UserMissions)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("fk_user_missions_sales");

            entity.HasOne(d => d.User).WithMany(p => p.UserMissions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_missions_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
