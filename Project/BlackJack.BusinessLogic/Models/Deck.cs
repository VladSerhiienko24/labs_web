using BlackJack.Entities.Entities;
using System.Collections.Generic;

namespace BlackJack.BusinessLogic.Models
{
    public class Deck
    {
        public List<Card> Cards { get; set; }
        public List<Card> UsedCards { get; set; }
        public List<Card> HangUpCards { get; set; }
    }
}