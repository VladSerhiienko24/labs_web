using System.Collections.Generic;

namespace BlackJack.ViewModels.GameViewModels
{
    public class RequestGetPlayersTwoCardsGameView
    {
        public int RoundId { get; set; }
        public List<int> Players { get; set; }

        public RequestGetPlayersTwoCardsGameView()
        {
            Players = new List<int>();
        }
    }
}