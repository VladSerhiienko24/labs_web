using BlackJack.Entities.Base;
using BlackJack.Entities.Enums;
using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlackJack.Entities.Entities
{
    public class Card : BaseEntity
    {
        [Required]
        public CardSuit Suit { get; set; }
        [Required]
        public CardFace Face { get; set; }

        [Write(false)]
        [Computed]
        public ICollection<HandCard> HandCards { get; set; }

        public Card()
        {
            HandCards = new List<HandCard>();
        }
    }
}