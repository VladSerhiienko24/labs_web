using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.DapperRepositories
{
    public class HandCardRepositoryDapper : DapperBaseRepository<HandCard>, IHandCardRepository
    {
        public HandCardRepositoryDapper(string connectionString) : base(connectionString)
        {
        }
    }
}