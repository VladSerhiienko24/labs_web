using System.Collections.Generic;

namespace BlackJack.ViewModels.GameViewModels
{
    public class GetHistoryOfWins
    {
        public List<GetHistoryOfWinsItem> Winners { get; set; }

        public GetHistoryOfWins()
        {
            Winners = new List<GetHistoryOfWinsItem>();
        }
    }

    public class GetHistoryOfWinsItem
    {
        public int RoundNumber { get; set; }
        public List<GetHistoryOfWinsItemItem> Players { get; set; }

        public GetHistoryOfWinsItem()
        {
            Players = new List<GetHistoryOfWinsItemItem>();
        }
    }

    public class GetHistoryOfWinsItemItem
    {
        public string NickName { get; set; }
        public int Points { get; set; }
    }
}