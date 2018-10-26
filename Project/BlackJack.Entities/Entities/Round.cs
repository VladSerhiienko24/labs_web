using BlackJack.Entities.Base;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackJack.Entities.Entities
{
    public class Round : BaseEntity
    {
        [Required]
        public int NumberOfRound { get; set; }
        [Required]
        public int GameId { get; set; }
        [ForeignKey("GameId")]
        [Write(false)]
        [Computed]
        public Game Game { get; set; }

        [Write(false)]
        [Computed]
        public ICollection<Hand> Hands { get; set; }

        public Round()
        {
            Hands = new List<Hand>();
        }
    }
}