using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class GameRepository : EFBaseRepository<Game>, IGameRepository
    {
        public GameRepository(GameContext context) : base(context)
        {
        }

        public async Task<Game> GetGameWithPlayerGames(int id)
        {
            Game game = await _context.Games
                .Include(g => g.PlayerGames.Select(pg => pg.Player))
                .FirstOrDefaultAsync(g => g.Id == id);
            
            return game;
        }
    }
}