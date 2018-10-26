using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class CardRepository : EFBaseRepository<Card>, ICardRepository
    {
        public CardRepository(GameContext context) : base(context)
        {
        }
    }
}