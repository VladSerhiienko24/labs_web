using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class HandCardRepository : EFBaseRepository<HandCard>, IHandCardRepository
    {
        public HandCardRepository(GameContext context) : base(context)
        {
        }
    }
}