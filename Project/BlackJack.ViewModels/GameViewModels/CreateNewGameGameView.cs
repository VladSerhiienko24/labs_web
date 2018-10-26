namespace BlackJack.ViewModels.GameViewModels
{
    public class CreateNewGameGameView
    {
        public string NickName { get; set; }
        public int MaxCountRounds { get; set; }
        public int CoinsAtStart { get; set; }
        public int CountBots { get; set; }
        public int Reward { get; set; }
    }
}