namespace BlackJack.ViewModels.GameViewModels
{
    public class RequestGetDeallerCardGameView
    {
        public int GameId { get; set; }
        public int RoundId { get; set; }
        public int PlayerId { get; set; }
    }
}