using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;

namespace BlackJack.DataAccess.Repositories.DapperRepositories
{
    public class CardRepositoryDapper : DapperBaseRepository<Card>, ICardRepository
    {
        public CardRepositoryDapper(string connectionString) : base(connectionString)
        {
        }
    }
}