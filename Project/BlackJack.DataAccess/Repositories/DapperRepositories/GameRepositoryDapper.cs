using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.DapperRepositories
{
    public class GameRepositoryDapper : DapperBaseRepository<Game>, IGameRepository
    {
        public GameRepositoryDapper(string connectionString) : base(connectionString)
        {
        }

        public async Task<Game> GetGameWithPlayerGames(int id)
        {
            var lookup = new Dictionary<int, Game>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Games g
                    INNER JOIN PlayerGames pg ON g.Id = pg.GameId
                    INNER JOIN Players p ON pg.PlayerId = p.Id
                    WHERE g.Id = @id";

                await connection.QueryAsync<Game, PlayerGame, Player, Game>(
                   sqlQuery,
                   (game, playerGame, player) =>
                   {
                       Game lookupGame = null;

                       if (!lookup.TryGetValue(game.Id, out lookupGame))
                       {
                           lookupGame = game;

                           lookupGame.PlayerGames = new List<PlayerGame>();

                           lookup.Add(game.Id, lookupGame);
                       }

                       playerGame.Player = player;

                       lookupGame.PlayerGames.Add(playerGame);

                       return lookupGame;
                   },
                   param: new { id },
                   splitOn: "Id, Id, Id");

                Game responseGameObject = lookup.Values.FirstOrDefault();
                
                return responseGameObject;
            }
        }

    }
}