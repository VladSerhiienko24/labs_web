using BlackJack.Entities.Entities;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Interfaces
{
    public interface IGameRepository : IBaseRepository<Game>
    {
        Task<Game> GetGameWithPlayerGames(int id);
    }
}