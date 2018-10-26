using BlackJack.Entities.Entities;
using BlackJack.Shared.Configurations;
using System.Data.Entity;

namespace BlackJack.DataAccess.DataAccept
{
    public class GameContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<Hand> Hands { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerGame> PlayerGames { get; set; }
        public DbSet<HandCard> HandCards { get; set; }

        public GameContext() : base(ConfigSettings.ConnectionStringName)
        {
        }

        public GameContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerGame>()
            .HasRequired<Player>(s => s.Player)
            .WithMany(g => g.PlayerGames)
            .HasForeignKey<int>(s => s.PlayerId);

            modelBuilder.Entity<PlayerGame>()
            .HasRequired<Game>(s => s.Game)
            .WithMany(g => g.PlayerGames)
            .HasForeignKey<int>(s => s.GameId);

            modelBuilder.Entity<HandCard>()
            .HasRequired<Hand>(s => s.Hand)
            .WithMany(g => g.HandCards)
            .HasForeignKey<int>(s => s.HandId);

            modelBuilder.Entity<HandCard>()
            .HasRequired<Card>(s => s.Card)
            .WithMany(g => g.HandCards)
            .HasForeignKey<int>(s => s.CardId);
        }
    }
}