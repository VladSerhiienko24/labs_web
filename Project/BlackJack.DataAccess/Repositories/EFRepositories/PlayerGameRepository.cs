using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class PlayerGameRepository : EFBaseRepository<PlayerGame>, IPlayerGameRepository
    {
        public PlayerGameRepository(GameContext context) : base(context)
        {
        }
    }
}