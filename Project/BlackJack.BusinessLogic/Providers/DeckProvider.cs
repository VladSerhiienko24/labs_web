using BlackJack.BusinessLogic.Extensions;
using BlackJack.BusinessLogic.Interfaces;
using BlackJack.BusinessLogic.Models;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Shared.Constants;
using System;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Providers
{
    public class DeckProvider : IDeckProvider
    {
        private ObjectCache _memoryCache;

        private ICardRepository _cardRepository;

        public DeckProvider(ICardRepository cardRepository)
        {
            _memoryCache = MemoryCache.Default;

            _cardRepository = cardRepository;
        }

        public void SetDeckInMemoryCashe(int gameId, Deck infinityDeck)
        {
            _memoryCache.Set(gameId.ToString(), infinityDeck, DateTime.Now.AddHours(GameConstants.HOURS_FOR_SAVE_DECK_IN_MEMORY_CACHE));
        }

        public async Task<Deck> GetAllDeckFromCache(int gameId)
        {
            Deck infinityDeck = null;

            if (_memoryCache.Contains(gameId.ToString()))
            {
                object deckObjeck = _memoryCache.Get(gameId.ToString()); 

                infinityDeck = deckObjeck as Deck;
            }

            if (!_memoryCache.Contains(gameId.ToString()))
            {
                infinityDeck = new Deck();

                var listCards = (await _cardRepository.GetAll()).ToList();

                DeckExtension.SetCards(infinityDeck, listCards);

                _memoryCache.Add(gameId.ToString(), infinityDeck, DateTime.Now.AddHours(GameConstants.HOURS_FOR_SAVE_DECK_IN_MEMORY_CACHE));
            }

            return infinityDeck;
        }
    }
}