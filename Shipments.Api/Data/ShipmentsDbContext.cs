using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shipments.Shared.Domain.Entities;
using Shipments.Api.Models;
namespace Shipments.Api.Data
{
    public class ShipmentsDbContext : IdentityDbContext<AppUser>
    {
        public ShipmentsDbContext(DbContextOptions<ShipmentsDbContext> options) : base(options)
        {

        }
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentEvent> ShipmentEvents => Set<ShipmentEvent>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {     
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Shipment>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.Status)
                    .HasConversion<int>();

                b.HasMany(x => x.Events)
                    .WithOne(x => x.Shipment!)
                    .HasForeignKey(x => x.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ShipmentEvent>(b =>
            {
                b.HasKey(x => x.Id);

                b.Property(x => x.OldStatus).HasConversion<int?>();
                b.Property(x => x.NewStatus).HasConversion<int?>();
            });
        }
    }
}
