using BlackJack.Entities.Base;
using BlackJack.Entities.Enums;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlackJack.Entities.Entities
{
    public class Player : BaseEntity
    {
        [Required]
        [StringLength(20, MinimumLength = 2)]
        public string NickName { get; set; }
        [Required]
        public PlayerRole PlayerRole { get; set; }
        [Required]
        public int Coins { get; set; }

        [Write(false)]
        [Computed]
        public ICollection<Hand> Hands { get; set; }
        [Write(false)]
        [Computed]
        public ICollection<PlayerGame> PlayerGames { get; set; }

        public Player()
        {
            Hands = new List<Hand>();
            PlayerGames = new List<PlayerGame>();
        }
    }
}