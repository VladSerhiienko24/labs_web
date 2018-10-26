using BlackJack.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Interfaces
{
    public interface IRoundRepository : IBaseRepository<Round>
    {
        Task<Round> GetLastRound(int gameId);
        Task<Round> GetRoundIncludeHandsAndPlayers(int stepId);
        Task<List<Round>> GetAllRoundsByGameId(int gameId);
    }
}