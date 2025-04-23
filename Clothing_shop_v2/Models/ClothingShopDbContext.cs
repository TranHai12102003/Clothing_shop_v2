using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Clothing_shop_v2.Models;

public partial class ClothingShopDbContext : DbContext
{
    public ClothingShopDbContext()
    {
    }

    public ClothingShopDbContext(DbContextOptions<ClothingShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CustomerType> CustomerTypes { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<VariantPromotion> VariantPromotions { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ClothingShopDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Banners_CategoryId");

            entity.HasIndex(e => e.ProductId, "IX_Banners_ProductId");

            entity.HasIndex(e => e.PromotionId, "IX_Banners_PromotionId");

            entity.Property(e => e.BannerName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.LinkUrl).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Banners).HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.Product).WithMany(p => p.Banners).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Promotion).WithMany(p => p.Banners).HasForeignKey(d => d.PromotionId);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Carts_UserId");

            entity.HasIndex(e => e.VariantId, "IX_Carts_VariantId");

            entity.HasOne(d => d.User).WithMany(p => p.Carts).HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Variant).WithMany(p => p.Carts).HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.ParentCategoryId, "IX_Categories_ParentCategoryId");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("imageUrl");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasForeignKey(d => d.ParentCategoryId);
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasIndex(e => e.ColorName, "IX_Colors_ColorName").IsUnique();

            entity.Property(e => e.ColorCode).HasMaxLength(10);
            entity.Property(e => e.ColorName).HasMaxLength(50);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_Comments_ProductId");

            entity.HasIndex(e => e.UserId, "IX_Comments_UserId");

            entity.HasOne(d => d.Product).WithMany(p => p.Comments).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.User).WithMany(p => p.Comments).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<CustomerType>(entity =>
        {
            entity.HasIndex(e => e.TypeName, "IX_CustomerTypes_TypeName").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.MinTotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.HasIndex(e => e.OrderId, "IX_InventoryTransactions_OrderId");

            entity.HasIndex(e => e.VariantId, "IX_InventoryTransactions_VariantId");

            entity.HasIndex(e => e.WarehouseId, "IX_InventoryTransactions_WarehouseId");

            entity.Property(e => e.Reason).HasMaxLength(255);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.InventoryTransactions).HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Variant).WithMany(p => p.InventoryTransactions).HasForeignKey(d => d.VariantId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryTransactions).HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_Notifications_OrderId");

            entity.HasIndex(e => e.PromotionId, "IX_Notifications_PromotionId");

            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Order).WithMany(p => p.Notifications).HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Promotion).WithMany(p => p.Notifications).HasForeignKey(d => d.PromotionId);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

            entity.HasIndex(e => e.VoucherId, "IX_Orders_VoucherId");

            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders).HasForeignKey(d => d.VoucherId);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

            entity.HasIndex(e => e.VariantId, "IX_OrderDetails_VariantId");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderDetails).HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => e.OrderId, "IX_Payments_OrderId").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentGateway).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithOne(p => p.Payment).HasForeignKey<Payment>(d => d.OrderId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Products_CategoryId");

            entity.Property(e => e.ProductName).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasForeignKey(d => d.CategoryId);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasIndex(e => e.ProductId, "IX_ProductImages_ProductId");

            entity.HasIndex(e => e.VariantId, "IX_ProductImages_VariantId");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Variant).WithMany(p => p.ProductImages).HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PromotionName).HasMaxLength(255);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.RoleName, "IX_Roles_RoleName").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasIndex(e => e.SizeName, "IX_Sizes_SizeName").IsUnique();

            entity.Property(e => e.SizeName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.CustomerTypeId, "IX_Users_CustomerTypeId");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.CustomerType).WithMany(p => p.Users).HasForeignKey(d => d.CustomerTypeId);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasIndex(e => e.ColorId, "IX_Variants_ColorId");

            entity.HasIndex(e => new { e.ProductId, e.SizeId, e.ColorId }, "IX_Variants_ProductId_SizeId_ColorId").IsUnique();

            entity.HasIndex(e => e.SizeId, "IX_Variants_SizeId");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Color).WithMany(p => p.Variants).HasForeignKey(d => d.ColorId);

            entity.HasOne(d => d.Product).WithMany(p => p.Variants).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Size).WithMany(p => p.Variants).HasForeignKey(d => d.SizeId);
        });

        modelBuilder.Entity<VariantPromotion>(entity =>
        {
            entity.HasIndex(e => e.PromotionId, "IX_VariantPromotions_PromotionId");

            entity.HasIndex(e => e.VariantId, "IX_VariantPromotions_VariantId");

            entity.Property(e => e.AppliedDiscount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Promotion).WithMany(p => p.VariantPromotions).HasForeignKey(d => d.PromotionId);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantPromotions).HasForeignKey(d => d.VariantId);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasIndex(e => e.CustomerTypeId, "IX_Vouchers_CustomerTypeId");

            entity.HasIndex(e => e.VoucherCode, "IX_Vouchers_VoucherCode").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountType).HasMaxLength(50);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinOrderAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);

            entity.HasOne(d => d.CustomerType).WithMany(p => p.Vouchers).HasForeignKey(d => d.CustomerTypeId);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(e => e.WarehouseName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
