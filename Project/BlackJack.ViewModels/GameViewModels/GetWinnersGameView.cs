using System.Collections.Generic;

namespace BlackJack.ViewModels.GameViewModels
{
    public class GetWinnersGameView
    {
        public string NickName { get; set; }
        public int Points { get; set; }
        public List<GetWinnersGameViewItem> WinnersOfGame { get; set; }

        public GetWinnersGameView()
        {
            WinnersOfGame = new List<GetWinnersGameViewItem>();
        }
    }

    public class GetWinnersGameViewItem
    {
        public string NickName { get; set; }
        public int Cash { get; set; }
    }
}