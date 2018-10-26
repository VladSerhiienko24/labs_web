using BlackJack.Entities.Base;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlackJack.Entities.Entities
{
    public class Game : BaseEntity
    {
        [Required]
        public int MaxCountRounds { get; set; }
        [Required]
        public int CoinsAtStart { get; set; }
        [Required]
        public int CountBots { get; set; }
        [Required]
        public int Reward { get; set; }
        [Required]
        public DateTime GameStart { get; set; }
        public bool IsFinished { get; set; }

        [Write(false)]
        [Computed]
        public ICollection<Round> Rounds { get; set; }
        [Write(false)]
        [Computed]
        public ICollection<PlayerGame> PlayerGames { get; set; }

        public Game()
        {
            Rounds = new List<Round>();
            PlayerGames = new List<PlayerGame>();
        }
    }
}