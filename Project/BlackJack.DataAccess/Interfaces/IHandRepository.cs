using BlackJack.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Interfaces
{
    public interface IHandRepository : IBaseRepository<Hand>
    {
        Task<Hand> GetWithCards(int id);
        Task<Hand> GetHandByRoundAndPlayerId(int stepId, int playerId);
        Task<Hand> GetHandWithCardsByRoundAndPlayerId(int stepId, int playerId);
        Task<List<Hand>> GetListHandsWithCardsWithoutDeallerHandByRoundId(int stepId);
        Task<Dictionary<Player, int>> GetListWithCountsOfWinsForAllGame(int gameId, List<Player> players);
    }
}