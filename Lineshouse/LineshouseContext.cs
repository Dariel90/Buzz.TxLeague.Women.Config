using Buzz.TxLeague.Women.Config.Lineshouse.Models;
using Buzz.TxLeague.Women.Config.Utils;
using Microsoft.EntityFrameworkCore;

namespace Buzz.TxLeague.Women.Config.Lineshouse
{
    public class LineshouseContext : DbContext
    {
        public virtual DbSet<League> Leagues { get; set; }
        public virtual DbSet<Sport> Sports { get; set; }
        public virtual DbSet<Event> Events { get; set; }

        public virtual DbSet<Fixture> Fixtures { get; set; }
        public virtual DbSet<EventMap> BetradarEventMaps { get; set; }

        public virtual DbSet<TxLeagueMap> TxLeagueMaps { get; set; }

        public DbSet<TournamentEvent> TournamentEvents { get; set; }

        public LineshouseContext() : base(GetOptions())
        {
        }

        private static DbContextOptions<LineshouseContext> GetOptions()
        {
            return new DbContextOptionsBuilder<LineshouseContext>()
            //.UseMySQL("server=10.0.0.186;port=3306;user=warehouse;password=JW+G_w9E;database=lineshouse-dev").Options;
            .UseMySQL("server=10.0.0.186;port=3306;user=warehouse;password=JW+G_w9E;database=lineshouse-prod").Options;
            //.UseMySQL("Server=localhost;User Id=root;Password=admin123*;Database=lineshouse-prod;Port=3306").Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fixture>().Property(t => t.Name).IsRequired().HasMaxLength(255);

            modelBuilder.Entity<TxLeagueMap>().HasKey(o => new { o.PgId });
            modelBuilder.Entity<TxLeagueMap>()
                .HasOne(t => t.League)
                .WithMany()
                .HasForeignKey(a => a.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TxLeagueMap>().Property(t => t.Name).IsRequired().HasMaxLength(255);

            modelBuilder.Entity<Sport>().Property(t => t.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Sport>().HasMany(t => t.Leagues).WithOne(t => t.Sport).OnDelete(DeleteBehavior.Cascade).IsRequired();

            //League model additional configuration
            modelBuilder.Entity<League>().Property(t => t.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<League>().Property(t => t.IsDefault).IsRequired();
            modelBuilder.Entity<League>().HasMany(t => t.Events).WithOne(t => t.League).OnDelete(DeleteBehavior.Cascade).IsRequired();
            //modelBuilder.Entity<League>().HasOne(t => t.Sport).WithMany().HasForeignKey(a => a.SportId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<League>().Property(t => t.SportId).HasColumnName("SportId");

            //Event model additional configuration
            modelBuilder.Entity<Event>().Property(t => t.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Event>().HasOne(t => t.League).WithMany().HasForeignKey(l => l.LeagueId).IsRequired();
            modelBuilder.Entity<Event>().HasOne(t => t.League).WithMany(t => t.Events).OnDelete(DeleteBehavior.Cascade).IsRequired();
            modelBuilder.Entity<Event>().HasMany(t => t.Fixtures).WithOne(t => t.Event).OnDelete(DeleteBehavior.Cascade).IsRequired();

            modelBuilder.Entity<EventMap>()
                .HasOne(t => t.ExtEvent)
                .WithMany()
                .HasForeignKey(a => a.ExtEventId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EventMap>().Property(t => t.SportId).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<EventMap>().Property(t => t.TournamentId).IsRequired().HasMaxLength(255);

            modelBuilder.Entity<TournamentEvent>(entity =>
            {
                entity.HasNoKey();
            });
        }
    }
}