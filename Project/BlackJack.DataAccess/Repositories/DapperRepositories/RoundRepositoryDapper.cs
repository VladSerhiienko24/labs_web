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
    public class RoundRepositoryDapper : DapperBaseRepository<Round>, IRoundRepository
    {
        public RoundRepositoryDapper(string connectionString) : base(connectionString)
        {
        }

        public async Task<List<Round>> GetAllRoundsByGameId(int gameId)
        {
            var roundLookup = new Dictionary<int, Round>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Rounds r
                                INNER JOIN Hands h ON h.RoundId = r.Id
                                INNER JOIN Players p ON p.Id = h.PlayerId
                                WHERE r.GameId = @gameId";

                await connection.QueryAsync<Round, Hand, Player, Round>(
                   sqlQuery,
                   (round, hand, player) =>
                   {
                       Round lookupRound = null;

                       if (!roundLookup.TryGetValue(round.Id, out lookupRound))
                       {
                           lookupRound = round;

                           roundLookup.Add(lookupRound.Id, lookupRound);
                       }

                       if (player != null)
                       {
                           hand.Player = player;
                       }

                       if (hand != null && !lookupRound.Hands.Any(item => item.Id == hand.Id))
                       {
                           lookupRound.Hands.Add(hand);
                       }

                       return lookupRound;
                   },
                   param: new { gameId },
                   splitOn: "Id, Id, Id");

                List<Round> roundsList = roundLookup.Values.ToList();

                return roundsList;
            }
        }

        public async Task<Round> GetLastRound(int gameId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Rounds 
                                WHERE GameId = @gameId
                                ORDER BY NumberOfRound DESC";

                Round round = await connection.QueryFirstOrDefaultAsync<Round>(sqlQuery, new { gameId });

                return round;
            }
        }

        public async Task<Round> GetRoundIncludeHandsAndPlayers(int roundId)
        {
            var roundLookup = new Dictionary<int, Round>();
            var handLookup = new Dictionary<int, Hand>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Rounds r
                                INNER JOIN Hands h ON h.RoundId = r.Id
                                INNER JOIN Players p ON p.Id = h.PlayerId
                                WHERE r.Id = @roundId";

                await connection.QueryAsync<Round, Hand, Player, Round>(
                   sqlQuery,
                   (round, hand, player) =>
                   {
                       Round lookupRound = null;
                       Hand lookupHand = null;

                       if (!roundLookup.TryGetValue(round.Id, out lookupRound))
                       {
                           lookupRound = round;

                           roundLookup.Add(lookupRound.Id, lookupRound);
                       }

                       if (!handLookup.TryGetValue(hand.Id, out lookupHand))
                       {
                           lookupHand = hand;

                           handLookup.Add(lookupHand.Id, lookupHand);
                       }

                       if (player != null)
                       {
                           lookupHand.Player = player;
                       }

                       if (lookupHand != null && !lookupRound.Hands.Any(item => item.Id == hand.Id))
                       {
                           lookupRound.Hands.Add(lookupHand);
                       }

                       return lookupRound;
                   },
                   param: new { roundId },
                   splitOn: "Id, Id, Id");

                Round responseRoundObject = roundLookup.Values.FirstOrDefault();

                return responseRoundObject;
            }
        }

    }
}