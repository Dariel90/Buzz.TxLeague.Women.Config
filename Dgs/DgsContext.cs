using Buzz.TxLeague.Women.Config.Dgs.Models;
using Microsoft.EntityFrameworkCore;

using CR_DTO = Buzz.TxLeague.Women.Config.Dgs.Models;

using DGS_DTO = Buzz.TxLeague.Women.Config.Dgs.Models.DGS;

namespace Buzz.TxLeague.Women.Config.Dgs
{
    public class DgsContext : DbContext
    {
        public virtual DbSet<CR_DTO::Sport> Sport { get; set; }
        public virtual DbSet<CR_DTO::League> League { get; set; }
        public virtual DbSet<CR_DTO::Config> Config { get; set; }
        public virtual DbSet<DGS_DTO::League> DLeague { get; set; }
        public virtual DbSet<CR_DTO::CR_Event> Events { get; set; }
        public virtual DbSet<CR_DTO::CR_Fixture> Fixtures { get; set; }
        public virtual DbSet<CR_DTO::CR_Period> Periods { get; set; }

        public DgsContext() : base(GetOptions())
        {
        }

        private static DbContextOptions<DgsContext> GetOptions()
        {
            return new DbContextOptionsBuilder<DgsContext>()
                //.UseSqlServer("Server=172.16.200.58;User Id=sa;Password=Bo0ki3SAAuth1;Initial Catalog=DGSData;")
                .UseSqlServer("Server=10.0.0.13;User Id=sportbookdba;Password=lumalu;Initial Catalog=DGSData;")
                .Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DGS_DTO::League>(entity =>
            {
                entity.HasKey(e => e.IdLeague);

                entity.ToTable("LEAGUE");

                entity.HasIndex(e => e.Description)
                    .HasName("IX_LEAGUE")
                    .IsUnique();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdLeagueRegion).HasDefaultValueSql("(1)");

                entity.Property(e => e.IdSport)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.LastModification).HasColumnType("datetime");

                entity.Property(e => e.ShortDescription)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.TeamFkrequired)
                    .IsRequired()
                    .HasColumnName("TeamFKRequired")
                    .HasDefaultValueSql("(0)");
            });

            modelBuilder.Entity<CR_DTO::Sport>(entity =>
            {
                entity.ToTable("CR_Sport");
                entity.Property(t => t.Name).IsRequired().HasMaxLength(255).IsUnicode(false);
                entity.HasIndex(t => t.Name).IsUnique();
                entity.HasMany(t => t.Leagues).WithOne(t => t.Sport).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity<CR_DTO::League>(entity =>
            {
                entity.ToTable("CR_League");
                entity.Property(t => t.Name).IsRequired().HasMaxLength(255).IsUnicode(false);
                entity.HasOne(t => t.Config).WithOne(t => t.League).HasForeignKey<CR_DTO::Config>(m => m.LeagueId).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity<CR_DTO::Config>(entity =>
            {
                entity.ToTable("CR_Config");
                entity.HasIndex("LeagueId").IsUnique();
            });

            modelBuilder.Entity<CR_Event>(entity =>
            {
                entity.ToTable("CR_Event");
                entity.HasMany(t => t.Fixtures).WithOne(t => t.Event).OnDelete(DeleteBehavior.Cascade).IsRequired();
                entity.HasMany(t => t.Periods).WithOne(t => t.Event).OnDelete(DeleteBehavior.Cascade).IsRequired();
            });

            modelBuilder.Entity<CR_Period>().ToTable("CR_Period");

            modelBuilder.Entity<CR_Fixture>(entity =>
            {
                entity.ToTable("CR_Fixture");

                entity.HasIndex(m => m.Type);
            });
        }
    }
}