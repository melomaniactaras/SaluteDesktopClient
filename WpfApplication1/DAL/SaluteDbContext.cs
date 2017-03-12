using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WpfApplication1.Models;

namespace WpfApplication1.DAL
{
    public class SaluteDbContext : DbContext
    {
        public DbSet<SeasonRating> SeasonRatingDbSet { get; set; }

        public DbSet<SeriesRating> SeriesRatingDbSet { get; set; }

        public DbSet<AllTimeRating> AllTimeRatingDbSet { get; set; }

        public DbSet<WeekRating> WeekRatingDbSet { get; set; }

        public DbSet<SeasonRatingFiim> SeasonRatingFiimDbSet { get; set; }
        
        public DbSet<SeriesRatingFiim> SeriesRatingFiimDbSet { get; set; }

        public DbSet<WeekRatingFiim> WeekRatingFiimDbSet { get; set; }

        public DbSet<AllTimeRatingFiim> AllTimeRatingFiimDbSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
