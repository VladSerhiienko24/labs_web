using BlackJack.BusinessLogic.Models;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Interfaces
{
    public interface IDeckProvider
    {
        void SetDeckInMemoryCashe(int gameId, Deck infinityDeck);
        Task<Deck> GetAllDeckFromCache(int cacheKey);
    }
}