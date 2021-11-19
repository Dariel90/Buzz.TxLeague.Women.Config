using Buzz.TxLeague.Women.Config.Lineshouse.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Buzz.TxLeague.Women.Config.Lineshouse
{
    public class LineshouseContext : DbContext
    {

        public virtual DbSet<League> Leagues { get; set; }

        public virtual DbSet<TxLeagueMap> TxLeagueMaps { get; set; }

        public LineshouseContext() : base(GetOptions())
        {

        }

        private static DbContextOptions<LineshouseContext> GetOptions()
        {
            return new DbContextOptionsBuilder<LineshouseContext>()
                .UseMySQL("server=10.0.0.186;port=3306;user=warehouse;password=JW+G_w9E;database=lineshouse-prod;").Options;
                //.UseMySQL("Server=localhost;User Id=root;Password=admin123*;Database=lineshouse-tx;Port=3306").Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}