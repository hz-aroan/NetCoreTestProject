#nullable disable

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Main;

public class MainDbContext : DbContext
{
    public virtual DbSet<BasketHead> BasketHeads { get; set; }

    public virtual DbSet<BasketItem> BasketItems { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Product> Products { get; set; }



    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<BasketHead>(entity =>
        {
            entity.ToTable("BasketHead");
            entity.Property(e => e.BasketHeadId).UseIdentityColumn();
            entity.HasIndex(e => e.Uid, "IDX_BasketHead_UID").IsUnique();
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.HasOne(d => d.Event).WithMany(p => p.BasketHeads)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BasketHead_Event");
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.ToTable("BasketItem");
            entity.HasIndex(e => e.BasketHeadId, "IDX_BasketItem_HEAD");
            entity.HasOne(d => d.BasketHead).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.BasketHeadId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BasketItem_BasketHead");

            entity.HasOne(d => d.Product).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BasketItem_Product");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");
            entity.Property(e => e.EventId).UseIdentityColumn();
            entity.HasIndex(e => e.Name, "IDX_Event_Name");
            entity.Property(e => e.FeeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FeeCurrency).HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
            entity.HasIndex(e => e.Name, "IDX_Product_Name").IsUnique();
            entity.Property(e => e.FeeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FeeCurrency).HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}