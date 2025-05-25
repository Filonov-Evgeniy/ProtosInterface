using Microsoft.EntityFrameworkCore;
using ProtosInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ProtosInterface
{
    class AppDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductLink> ProductLinks { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<OperationVariant> OperationVariants { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<OperationVariantComponent> OperationVariantComponents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(250)
                      .HasColumnName("name");

                entity.Property(p => p.TypeId)
                      .HasColumnName("type_id");

                entity.Property(p => p.CoopStatusId)
                      .HasColumnName("coop_status_id");

                entity.Property(p => p.Description)
                      .HasMaxLength(4000)
                      .HasColumnName("description");
            });

            // Конфигурация ProductLink
            modelBuilder.Entity<ProductLink>(entity =>
            {
                entity.ToTable("Product_Link");

                // Составной первичный ключ
                entity.HasKey(pl => new { pl.ParentProductId, pl.IncludedProductId });

                // Настройка связи с родительским продуктом
                entity.HasOne(pl => pl.ParentProduct)
                      .WithMany(p => p.ChildLinks)
                      .HasForeignKey(pl => pl.ParentProductId)
                      .OnDelete(DeleteBehavior.Restrict); // Или Cascade по необходимости

                // Настройка связи с включаемым продуктом
                entity.HasOne(pl => pl.IncludedProduct)
                      .WithMany(p => p.ParentLinks)
                      .HasForeignKey(pl => pl.IncludedProductId);

                entity.Property(pl => pl.Amount)
                      .HasColumnName("amount");
            });

            modelBuilder.Entity<OperationType>(entity =>
            {
                entity.ToTable("Operation_Type");
                entity.HasKey(ot => ot.Id);

                entity.Property(ot => ot.Name)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasColumnName("name");

                entity.Property(ot => ot.Description)
                      .HasMaxLength(4000)
                      .HasColumnName("description");
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.ToTable("Operation");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Code)
                      .HasColumnName("code");

                entity.Property(o => o.TypeId)
                      .IsRequired()
                      .HasColumnName("type_id");

                entity.Property(o => o.ProductId)
                      .HasColumnName("product_id");

                entity.Property(o => o.CoopStatusId)
                      .HasColumnName("coop_status_id");

                entity.Property(o => o.Description)
                      .HasMaxLength(4000)
                      .HasColumnName("description");

                entity.HasOne(o => o.Product)
                      .WithMany()
                      .HasForeignKey(o => o.ProductId);

                entity.HasOne(o => o.OperationType)
                      .WithMany()
                      .HasForeignKey(o => o.TypeId);
            });
            modelBuilder.Entity<OperationVariant>(entity =>
            {
                entity.ToTable("Operation_Variant");
                entity.HasKey(ov => ov.Id);

                entity.Property(ov => ov.Id)
                      .HasColumnName("id");

                entity.Property(ov => ov.OperationId)
                      .IsRequired()
                      .HasColumnName("operation_id");

                entity.Property(ov => ov.Duration)
                      .IsRequired()
                      .HasColumnName("duration");

                entity.Property(ov => ov.Description)
                      .HasColumnType("VARCHAR(MAX)")
                      .HasColumnName("description");

                entity.HasOne(ov => ov.Operation)
                      .WithMany()
                      .HasForeignKey(ov => ov.OperationId);
            });
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id");

                entity.Property(e => e.InventoryNumber)
                      .IsRequired()
                      .HasColumnName("inventory_number");

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(10)
                      .HasColumnName("name");

                entity.Property(e => e.AreaId)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasColumnName("area_id");

                entity.Property(e => e.TypeId)
                      .IsRequired()
                      .HasColumnName("type_id");

                entity.Property(e => e.LoadFactor)
                      .IsRequired()
                      .HasColumnName("load_factor");

                entity.Property(e => e.Description)
                      .HasColumnType("VARCHAR(MAX)")
                      .HasColumnName("description");

                entity.Property(e => e.ShortName)
                      .HasMaxLength(10)
                      .HasColumnName("short_name");

                //entity.HasOne(e => e.Equipment_Type)
                //      .WithMany()
                //      .HasForeignKey(e => e.TypeId);
            });
            modelBuilder.Entity<OperationVariantComponent>(entity =>
            {
                entity.ToTable("Operation_Variant_Component");
                entity.HasKey(ovc => ovc.Id);

                entity.Property(ovc => ovc.Id)
                      .HasColumnName("id");

                entity.Property(ovc => ovc.OperationVariantId)
                      .IsRequired()
                      .HasColumnName("operation_variant_id");

                entity.Property(ovc => ovc.EquipmentId)
                      .IsRequired()
                      .HasColumnName("equipment_id");

                entity.Property(ovc => ovc.ProfessionId)
                      .HasColumnName("profession_id");

                entity.Property(ovc => ovc.WorkersAmount)
                      .HasColumnName("workers_amount");

                entity.HasOne(ovc => ovc.OperationVariant)
                      .WithMany()
                      .HasForeignKey(ovc => ovc.OperationVariantId);

                entity.HasOne(ovc => ovc.Equipment)
                      .WithMany()
                      .HasForeignKey(ovc => ovc.EquipmentId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DBConnection connect = new DBConnection();
            optionsBuilder.UseSqlServer(connect.ConnectionString);
            
        }
    }
}
