using BlackJack.DataAccess.DataAccept;
using BlackJack.DataAccess.Generics;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJack.DataAccess.Repositories.EFRepositories
{
    public class HandRepository : EFBaseRepository<Hand>, IHandRepository
    {
        public HandRepository(GameContext context) : base(context)
        {
        }

        public async Task<Hand> GetWithCards(int id)
        {
            Hand hand = await _context.Hands
                .Include(h => h.HandCards.Select(hc => hc.Card))
                .FirstOrDefaultAsync(h => h.Id == id);
            
            return hand;
        }

        public async Task<Hand> GetHandByRoundAndPlayerId(int roundId, int playerId)
        {
            Hand hand = await _context.Hands
                .FirstOrDefaultAsync(h => h.RoundId == roundId && h.PlayerId == playerId);

            return hand;
        }

        public async Task<Hand> GetHandWithCardsByRoundAndPlayerId(int roundId, int playerId)
        {
            Hand hand = await _context.Hands
               .Include(h => h.HandCards.Select(hc => hc.Card))
               .FirstOrDefaultAsync(h => h.RoundId == roundId && h.PlayerId == playerId);
            
            return hand;
        }

        public async Task<List<Hand>> GetListHandsWithCardsWithoutDeallerHandByRoundId(int roundId)
        {
            List<Hand> hands = await _context.Hands
                .Include(h => h.Player)
                .Include(h => h.HandCards.Select(hc => hc.Card))
                .Where(h => h.RoundId == roundId
                && (h.Player.PlayerRole == PlayerRole.Player
                || h.Player.PlayerRole == PlayerRole.Bot))
                .ToListAsync();
            
            return hands;
        }

        public async Task<Dictionary<Player, int>> GetListWithCountsOfWinsForAllGame(int gameId, List<Player> players)
        {
            var playerWins = new Dictionary<Player, int>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (Player player in players)
                    {
                        int count = await _context.Hands
                            .CountAsync(h => h.Round.GameId == gameId
                            && h.Situation == Situation.Win && h.PlayerId == player.Id);

                        playerWins.Add(player, count);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            return playerWins;
        }
    }
}