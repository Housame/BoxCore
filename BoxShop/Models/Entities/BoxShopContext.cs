using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BoxShop.Models.Entities
{
    public partial class BoxShopContext : DbContext
    {
        public virtual DbSet<Box> Box { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Box>(entity =>
            {
                entity.ToTable("Box", "web");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order", "web");

                entity.Property(e => e.DateOfPurchase).HasColumnType("date");

                entity.Property(e => e.IsPaid).HasDefaultValueSql("0");

                entity.HasOne(d => d.Box)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.BoxId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Order_ToBox");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Order_ToCustomer");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItem", "web");

                entity.Property(e => e.DateOfPurchase)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(2017)-(6))-(22");

                entity.Property(e => e.Price).HasColumnType("decimal");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Vat).HasColumnType("decimal");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItem)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrderItem_ToOrder");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "web");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal");

                entity.Property(e => e.Vat).HasColumnType("decimal");

                entity.HasOne(d => d.Box)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.BoxId)
                    .HasConstraintName("FK_Product_ToBox");

               
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "web");

                entity.Property(e => e.Email).HasMaxLength(450);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HashId).HasMaxLength(450);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNr).HasMaxLength(50);

                entity.HasOne(d => d.Box)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.BoxId)
                    .HasConstraintName("FK_Users_ToBoxes");
            });
        }

    }
}