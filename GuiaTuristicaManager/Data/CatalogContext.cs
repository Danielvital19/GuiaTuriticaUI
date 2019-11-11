using GuiaTuristicaManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GuiaTuristicaManager.Data
{
    public class CatalogContext : DbContext
    {
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Media> Media { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Model>().HasIndex(M => M.PlaceId).IsUnique();
        }

    }
}
