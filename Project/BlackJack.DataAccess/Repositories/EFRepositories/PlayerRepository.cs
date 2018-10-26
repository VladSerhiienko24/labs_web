using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.Entities.Enums;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class PlayerRepository : EFBaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(GameContext context) : base(context)
        {
        }

        public async Task<List<Player>> GetAllPlayers()
        {
            List<Player> players = await _context.Players
                .Where(p => p.PlayerRole == PlayerRole.Player)
                .ToListAsync();

            return players;
        }

        public async Task<List<Player>> GetAllFreeBots(int countNeededBots)
        {
            List<Player> bots = await _context.Players
                .Include(p => p.PlayerGames.Select(pg => pg.Game))
                .Where(g => !g.PlayerGames.Any(pg => !pg.Game.IsFinished) && g.PlayerRole == PlayerRole.Bot)
                .Take(countNeededBots)
                .ToListAsync();

            return bots;
        }

        public async Task<Player> GetFreeDealler()
        {
            Player dealler = await _context.Players
                .Include(p => p.PlayerGames.Select(pg => pg.Game))
                .FirstOrDefaultAsync(p => !p.PlayerGames.Any(pg => !pg.Game.IsFinished) && p.PlayerRole == PlayerRole.Dealler);

            return dealler;
        }

        public async Task<List<Player>> GetAllPlayersWithoutDeallerByGameId(int gameId)
        {
            List<Player> players = await _context.Players
                .Where(p => p.PlayerGames.Any(pg => pg.GameId == gameId)
                && (p.PlayerRole == PlayerRole.Player || p.PlayerRole == PlayerRole.Bot))
                .ToListAsync();

            return players;
        }

        public async Task CreateMultiplePlayersAndReturnTheirIds(List<Player> players)
        {
            _dbSet.AddRange(players);

            await _context.SaveChangesAsync();
        }

    }
}