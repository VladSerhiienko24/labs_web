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
    public class HandRepositoryDapper : DapperBaseRepository<Hand>, IHandRepository
    {
        public HandRepositoryDapper(string connectionString) : base(connectionString)
        {
        }

        public async Task<Hand> GetHandByRoundAndPlayerId(int roundId, int playerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Hands
                                WHERE RoundId = @roundId AND PlayerId = @playerId";

                Hand hand = await connection.QueryFirstAsync<Hand>(sqlQuery, new { roundId, playerId });

                return hand;
            }
        }

        public async Task<Hand> GetHandWithCardsByRoundAndPlayerId(int roundId, int playerId)
        {
            var lookup = new Dictionary<int, Hand>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Hands h
                                LEFT JOIN HandCards hc ON hc.HandId = h.Id
                                LEFT JOIN Cards c ON c.Id = hc.CardId
                                WHERE h.RoundId = @roundId AND h.PlayerId = @playerId";

                await connection.QueryAsync<Hand, HandCard, Card, Hand>(sqlQuery,
                    (hand, handCard, card) =>
                    {
                        Hand lookupHand = null;

                        if (!lookup.TryGetValue(hand.Id, out lookupHand))
                        {
                            lookupHand = hand;

                            lookupHand.HandCards = new List<HandCard>();

                            lookup.Add(hand.Id, lookupHand);
                        }

                        if (card != null)
                        {
                            handCard.Card = card;

                            lookupHand.HandCards.Add(handCard);
                        }

                        return lookupHand;
                    },
                    param: new { roundId, playerId },
                    splitOn: "Id, Id, Id");

                Hand responseHandObject = lookup.Values.FirstOrDefault();

                return responseHandObject;
            }
        }

        public async Task<List<Hand>> GetListHandsWithCardsWithoutDeallerHandByRoundId(int roundId)
        {
            var lookup = new Dictionary<int, Hand>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Hands h
                                INNER JOIN Players p ON p.Id = h.PlayerId
                                INNER JOIN HandCards hc ON hc.HandId = h.Id
                                INNER JOIN Cards c ON c.Id = hc.CardId
                                WHERE h.RoundId = @roundId AND (p.PlayerRole = 1 OR p.PlayerRole = 2)";

                await connection.QueryAsync<Hand, Player, HandCard, Card, Hand>(
                   sqlQuery,
                   (hand, player, handCard, card) =>
                   {
                       Hand lookupHand = null;

                       if (!lookup.TryGetValue(hand.Id, out lookupHand))
                       {
                           hand.Player = player;

                           lookupHand = hand;

                           lookupHand.HandCards = new List<HandCard>();

                           lookup.Add(hand.Id, lookupHand);
                       }

                       handCard.Card = card;

                       lookupHand.HandCards.Add(handCard);

                       return lookupHand;
                   },
                   param: new { roundId },
                   splitOn: "Id, Id, Id, Id");

                List<Hand> hands = lookup.Values.ToList();

                return hands;
            }
        }

        public async Task<Hand> GetWithCards(int id)
        {
            var lookup = new Dictionary<int, Hand>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"SELECT * FROM Hands h
                                LEFT JOIN HandCards hc ON hc.HandId = h.Id
                                LEFT JOIN Cards c ON c.Id = hc.CardId
                                WHERE h.Id = @id";

                await connection.QueryAsync<Hand, HandCard, Card, Hand>(sqlQuery,
                    (hand, handCard, card) =>
                    {
                        Hand lookupHand = null;

                        if (!lookup.TryGetValue(hand.Id, out lookupHand))
                        {
                            lookupHand = hand;

                            lookupHand.HandCards = new List<HandCard>();

                            lookup.Add(hand.Id, lookupHand);
                        }

                        handCard.Card = card;

                        lookupHand.HandCards.Add(handCard);

                        return lookupHand;
                    },
                    param: new { id },
                    splitOn: "Id, Id, Id");

                Hand responseHandObject = lookup.Values.FirstOrDefault();

                return responseHandObject;
            }
        }

        public async Task<Dictionary<Player, int>> GetListWithCountsOfWinsForAllGame(int gameId, List<Player> players)
        {
            var playerWins = new Dictionary<Player, int>();
            
            if (players != null)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var sqlQuery = @"SELECT Count(*) FROM Hands h
                                INNER JOIN Rounds r ON h.RoundId = r.Id
                                WHERE r.GameId = @gameId AND h.Situation = 1 AND h.PlayerId = @playerId";

                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (Player player in players)
                        {
                            int playerId = player.Id;

                            int count = await connection.QueryFirstAsync<int>(sqlQuery, new { gameId, playerId }, transaction: transaction);

                            playerWins.Add(player, count);
                        }

                        transaction.Commit();
                    }
                }
            }

            return playerWins;
        }
    }
}