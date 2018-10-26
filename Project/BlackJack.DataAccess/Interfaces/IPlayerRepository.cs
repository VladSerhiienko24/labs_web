using BlackJack.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Interfaces
{
    public interface IPlayerRepository : IBaseRepository<Player>
    {
        Task<List<Player>> GetAllPlayers();
        Task<List<Player>> GetAllFreeBots(int countNeededBots);
        Task<Player> GetFreeDealler();
        Task<List<Player>> GetAllPlayersWithoutDeallerByGameId(int gameId);
        Task CreateMultiplePlayersAndReturnTheirIds(List<Player> players);
    }
}