using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.Exceptions.DataAccessExceptions;
using BlackJack.Shared.Constants;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.DapperRepositories
{
    public class PlayerRepositoryDapper : DapperBaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepositoryDapper(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Player>> GetAllPlayers()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Players p
                                WHERE p.PlayerRole = 1";

                List<Player> players = (await connection.QueryAsync<Player>(sqlQuery)).AsList();

                return players;
            }
        }

        public async Task<List<Player>> GetAllFreeBots(int countNeededBots)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT TOP(@countNeededBots) p.* FROM Players p
                                WHERE p.PlayerRole = 2 AND 
                                (SELECT COUNT(*) FROM PlayerGames pg 
                                LEFT JOIN Games g ON g.Id = pg.GameId 
                                WHERE g.IsFinished = 0 AND pg.PlayerId = p.Id) = 0";

                List<Player> bots = (await connection.QueryAsync<Player>(sqlQuery, param: new { countNeededBots })).AsList();

                return bots;
            }
        }

        public async Task<Player> GetFreeDealler()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT p.* FROM Players p
                                WHERE p.PlayerRole = 3 AND 
                                (SELECT COUNT(*) FROM PlayerGames pg 
                                LEFT JOIN  Games g ON g.Id = pg.GameId 
                                WHERE g.IsFinished = 0 AND pg.PlayerId = p.Id) = 0";

                Player dealler = await connection.QueryFirstOrDefaultAsync<Player>(sqlQuery);

                return dealler;
            }
        }

        public async Task<List<Player>> GetAllPlayersWithoutDeallerByGameId(int gameId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT p.* FROM Players p
                            INNER JOIN PlayerGames pg ON p.Id = pg.PlayerId
                            WHERE pg.GameId = @gameId AND (p.PlayerRole = 1 OR p.PlayerRole = 2)";

                List<Player> players = (await connection.QueryAsync<Player>(sqlQuery, param: new { gameId })).AsList();

                return players;
            }
        }

        public async Task CreateMultiplePlayersAndReturnTheirIds(List<Player> players)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sqlQuery = @"INSERT INTO Players (NickName, PlayerRole, Coins) 
                                VALUES(@NickName, @PlayerRole, @Coins);
                                SELECT CAST(SCOPE_IDENTITY() as int)";

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (Player player in players)
                    {
                        int? itemId = await connection.QueryFirstOrDefaultAsync<int?>(sqlQuery, player, transaction: transaction);

                        if (itemId == null)
                        {
                            var stringBuilder = new StringBuilder();

                            stringBuilder.AppendLine(string.Format("PlayerNickName: {0}", player.NickName));
                            stringBuilder.AppendLine(string.Format("PlayerPlayerRole: {0}", player.PlayerRole));
                            stringBuilder.AppendLine(string.Format("PlayerCoins: {0}", player.Coins));
                            stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.DB_UPDATE_ITEM_EXCEPTION_MESSAGE));

                            string message = stringBuilder.ToString();

                            throw new DataBaseGetNullFromCreateItemException(message);
                        }

                        player.Id = itemId.Value;
                    }

                    transaction.Commit();
                }
            }
        }
    }
}