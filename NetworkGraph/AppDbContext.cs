using Microsoft.EntityFrameworkCore;
using NetworkGraph.models;

namespace NetworkGraph
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\учеба\\NetworkGraph\\Database1.mdf;Integrated Security=True");
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\учеба\\NetworkGraph\\Database2.mdf;Integrated Security=True");

        }

        // Таблицы
        public DbSet<Product> Product { get; set; }
        public DbSet<OperationType> OperationType { get; set; }
        public DbSet<ProductLink> ProductLink { get; set; }
        public DbSet<Operation> Operation { get; set; }
        public DbSet<OperationVariant> OperationVariant { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<OperationVariantComponent> OperationVariantComponent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация Equipment
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InventoryNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                // Раскомментируйте, когда добавите соответствующие модели
                //entity.HasOne(e => e.Area)
                //      .WithMany()
                //      .HasForeignKey(e => e.TerritorialAreaId);

                //entity.HasOne(e => e.Type)
                //      .WithMany()
                //      .HasForeignKey(e => e.FunctionalAreaId);
            });

            // Конфигурация OperationType
            modelBuilder.Entity<OperationType>(entity =>
            {
                entity.HasKey(ot => ot.Id);
                entity.Property(ot => ot.Name).IsRequired().HasMaxLength(100);
                entity.Property(ot => ot.ShortName).IsRequired().HasMaxLength(20);
            });

            // Конфигурация Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);

                // Раскомментируйте, когда добавите соответствующие модели
                //entity.HasOne(p => p.CoopStatus)
                //      .WithMany()
                //      .HasForeignKey(p => p.CoopStatusId);

                //entity.HasOne(p => p.Type)
                //      .WithMany()
                //      .HasForeignKey(p => p.TypeId);
            });

            // Конфигурация ProductLink (связь между продуктами)
            modelBuilder.Entity<ProductLink>().ToTable("Product_Link");

            modelBuilder.Entity<ProductLink>(entity =>
            {
                entity.HasKey(pl => new { pl.ParentProductId, pl.IncludedProductId });

                entity.HasOne(pl => pl.ParentProduct)
                      .WithMany(p => p.ProductLinkParentProducts)
                      .HasForeignKey(pl => pl.ParentProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pl => pl.IncludedProduct)
                      .WithMany(p => p.ProductLinkIncludedProducts)
                      .HasForeignKey(pl => pl.IncludedProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация Operation
            modelBuilder.Entity<Operation>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.HasOne(o => o.Type)
                      .WithMany(ot => ot.Operations)
                      .HasForeignKey(o => o.TypeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Product)
                      .WithMany(p => p.Operations)
                      .HasForeignKey(o => o.ProductId);

                // Раскомментируйте, когда добавите CoopStatus
                //entity.HasOne(o => o.CoopStatus)
                //      .WithMany()
                //      .HasForeignKey(o => o.CoopStatusId);
            });

            modelBuilder.Entity<OperationVariant>().ToTable("Operation_Variant");

            // Конфигурация OperationVariant
            modelBuilder.Entity<OperationVariant>(entity =>
            {
                entity.HasKey(ov => ov.Id);

                entity.HasOne(ov => ov.Operation)
                      .WithMany(o => o.OperationVariants)
                      .HasForeignKey(ov => ov.OperationId);
            });

            modelBuilder.Entity<OperationVariantComponent>().ToTable("Operation_Variant_Component");

            // Конфигурация OperationVariantComponent
            modelBuilder.Entity<OperationVariantComponent>(entity =>
            {
                entity.ToTable("Operation_Variant_Component"); // Точное имя таблицы в БД
                entity.HasKey(ovc => ovc.Id);

                // Минимальная настройка связей
                entity.HasOne(ovc => ovc.OperationVariant)
                      .WithMany()
                      .HasForeignKey(ovc => ovc.OperationVariantId);

                entity.HasOne(ovc => ovc.Equipment)
                      .WithMany()
                      .HasForeignKey(ovc => ovc.EquipmentId)
                      .IsRequired(false);
            });
        }
    }
}