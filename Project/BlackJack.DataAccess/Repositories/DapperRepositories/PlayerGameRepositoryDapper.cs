using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.DapperRepositories
{
    public class PlayerGameRepositoryDapper : DapperBaseRepository<PlayerGame>, IPlayerGameRepository
    {
        public PlayerGameRepositoryDapper(string connectionString) : base(connectionString)
        {
        }
    }
}