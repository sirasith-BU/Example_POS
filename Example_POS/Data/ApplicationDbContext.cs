using Example_POS.Models;
using Microsoft.EntityFrameworkCore;

namespace Example_POS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PurPurchaseOrder> PurPurchaseOrder { get; set; }
        public DbSet<PurPurchaseOrderItem> PurPurchaseOrderItem { get; set; }
        public DbSet<SysFlex> SysFlex { get; set; }
        public DbSet<SysFlexItem> SysFlexItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SysFlex>()
                .HasIndex(f => f.FlexCode)
                .IsUnique();

            modelBuilder.Entity<SysFlexItem>()
                .HasOne(item => item.SysFlex)
                .WithMany(flex => flex.SysFlexItem)
                .HasForeignKey(item => item.FlexId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SysFlex>()
                .Property(f => f.FlexCode)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<SysFlexItem>()
                .Property(i => i.FlexItemCode)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
