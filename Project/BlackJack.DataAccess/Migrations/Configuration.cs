namespace BlackJack.DataAccess.Migrations
{
    using BlackJack.Entities.Entities;
    using BlackJack.Entities.Enums;
    using BlackJack.Shared.Constants;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BlackJack.DataAccess.DataAccept.GameContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlackJack.DataAccess.DataAccept.GameContext context)
        {
            if (context.Games.Any() || context.PlayerGames.Any() || context.Players.Any() || context.Rounds.Any() || context.Hands.Any() || context.Cards.Any())
            {
                return;
            }

            var game1 = new Game { MaxCountRounds = 2, GameStart = DateTime.Now, CountBots = 3, CoinsAtStart = 1000, Reward = 1000 };
            var game2 = new Game { MaxCountRounds = 3, GameStart = DateTime.Now, CountBots = 4, CoinsAtStart = 1000, Reward = 1000 };

            context.Games.AddRange(new List<Game> { game1, game2 });

            context.SaveChanges();

            var player1 = new Player() { NickName = "Dealler", PlayerRole = PlayerRole.Dealler, Coins = GameConstants.BOTS_COINS };
            var player2 = new Player() { NickName = "Vlad", PlayerRole = PlayerRole.Player, Coins = 200 };

            var player4 = new Player() { NickName = "Maria", PlayerRole = PlayerRole.Dealler, Coins = 200 };
            var player5 = new Player() { NickName = "Drake", PlayerRole = PlayerRole.Bot, Coins = GameConstants.BOTS_COINS };
            var player6 = new Player() { NickName = "Martin", PlayerRole = PlayerRole.Bot, Coins = GameConstants.BOTS_COINS };

            context.Players.AddRange(new List<Player> { player1, player2, player4, player5, player6 });

            context.SaveChanges();

            var playerGames = new List<PlayerGame>()
            {
                new PlayerGame() { PlayerId = player1.Id, GameId = game1.Id },
                new PlayerGame() { PlayerId = player2.Id, GameId = game1.Id },
                new PlayerGame() { PlayerId = player1.Id, GameId = game2.Id },
                new PlayerGame() { PlayerId = player4.Id, GameId = game2.Id },
                new PlayerGame() { PlayerId = player5.Id, GameId = game2.Id },
                new PlayerGame() { PlayerId = player6.Id, GameId = game2.Id }
            };

            context.PlayerGames.AddRange(playerGames);

            context.SaveChanges();

            var rounds = new List<Round>()
            {
                new Round() { GameId = game1.Id, NumberOfRound = 1 },
                new Round() { GameId = game1.Id, NumberOfRound = 2 },
                new Round() { GameId = game2.Id, NumberOfRound = 1 },
                new Round() { GameId = game2.Id, NumberOfRound = 2 },
                new Round() { GameId = game2.Id, NumberOfRound = 3 },
                new Round() { GameId = game2.Id, NumberOfRound = 4 }
            };

            context.Rounds.AddRange(rounds);

            context.SaveChanges();

            var hands = new List<Hand>()
            {
                new Hand() { Summary = 18, PlayerId = player1.Id, RoundId = 1, Deal = 40 },
                new Hand() { Summary = 20, PlayerId = player2.Id, RoundId = 1, Deal = 40 },
                new Hand() { Summary = 19, PlayerId = player1.Id, RoundId = 2, Deal = 40 },
                new Hand() { Summary = 21, PlayerId = player2.Id, RoundId = 2, Deal = 40 },

                new Hand() { Summary = 17, PlayerId = player1.Id, RoundId = 3, Deal = 40 },
                new Hand() { Summary = 18, PlayerId = player4.Id, RoundId = 3, Deal = 40 },
                new Hand() { Summary = 19, PlayerId = player5.Id, RoundId = 3, Deal = 40 },
                new Hand() { Summary = 20, PlayerId = player6.Id, RoundId = 3, Deal = 40 },

                new Hand() { Summary = 21, PlayerId = player1.Id, RoundId = 4, Deal = 100 },
                new Hand() { Summary = 20, PlayerId = player4.Id, RoundId = 4, Deal = 40 },
                new Hand() { Summary = 18, PlayerId = player5.Id, RoundId = 4, Deal = 40 },
                new Hand() { Summary = 19, PlayerId = player6.Id, RoundId = 4, Deal = 40 },

                new Hand() { Summary = 20, PlayerId = player1.Id, RoundId = 5, Deal = 40 },
                new Hand() { Summary = 18, PlayerId = player4.Id, RoundId = 5, Deal = 40 },
                new Hand() { Summary = 16, PlayerId = player5.Id, RoundId = 5, Deal = 40 },
                new Hand() { Summary = 25, PlayerId = player6.Id, RoundId = 5, Deal = 40 },

                new Hand() { Summary = 21, PlayerId = player1.Id, RoundId = 6, Deal = 100 },
                new Hand() { Summary = 27, PlayerId = player4.Id, RoundId = 6, Deal = 40 },
                new Hand() { Summary = 20, PlayerId = player5.Id, RoundId = 6, Deal = 40 },
                new Hand() { Summary = 19, PlayerId = player6.Id, RoundId = 6, Deal = 40 }
            };

            context.Hands.AddRange(hands);

            context.SaveChanges();

            var cards = new List<Card>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                if (suit == CardSuit.None)
                {
                    continue;
                }

                foreach (CardFace face in Enum.GetValues(typeof(CardFace)))
                {
                    if (face == CardFace.None)
                    {
                        continue;
                    }

                    var card = new Card();

                    card.Face = face;
                    card.Suit = suit;

                    cards.Add(card);
                }
            }

            context.Cards.AddRange(cards);

            context.SaveChanges();

        }
    }
}