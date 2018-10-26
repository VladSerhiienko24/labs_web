using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class RoundRepository : EFBaseRepository<Round>, IRoundRepository
    {
        public RoundRepository(GameContext context) : base(context)
        {
        }

        public async Task<List<Round>> GetAllRoundsByGameId(int gameId)
        {
            List<Round> rounds = await _context.Rounds
                .Include(r => r.Hands.Select(h => h.Player))
                .Where(r => r.GameId.Equals(gameId))
                .ToListAsync();

            return rounds;
        }

        public async Task<Round> GetLastRound(int gameId)
        {
            Round round = await _context.Rounds
                .OrderByDescending(r => r.NumberOfRound)
                .FirstOrDefaultAsync(r => r.GameId.Equals(gameId));

            return round;
        }

        public async Task<Round> GetRoundIncludeHandsAndPlayers(int roundId)
        {
            Round round = await _context.Rounds
                .Include(r => r.Hands.Select(h => h.Player))
                .FirstOrDefaultAsync(r => r.Id.Equals(roundId));
            
            return round;
        }
    }
}