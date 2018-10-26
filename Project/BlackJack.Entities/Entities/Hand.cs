using BlackJack.Entities.Base;
using BlackJack.Entities.Enums;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackJack.Entities.Entities
{
    public class Hand : BaseEntity
    {
        public int Summary { get; set; }
        public VictoryType VictoryType { get; set; }
        public Situation Situation { get; set; }
        public int Deal { get; set; }
        public int InsuranceCoins { get; set; }

        [Required]
        public int PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        [Write(false)]
        [Computed]
        public Player Player { get; set; }

        [Required]
        public int RoundId { get; set; }
        [ForeignKey("RoundId")]
        [Write(false)]
        [Computed]
        public Round Round { get; set; }

        [Write(false)]
        [Computed]
        public ICollection<HandCard> HandCards { get; set; }

        public Hand()
        {
            HandCards = new List<HandCard>();
        }
    }
}