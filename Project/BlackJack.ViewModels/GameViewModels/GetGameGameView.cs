using BlackJack.ViewModels.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace BlackJack.ViewModels.GameViewModels
{
    public class GetGameGameView
    {
        public GameGetGameGameViewItem Game { get; set; }
        public List<PlayerGetGameGameViewItem> Players { get; set; }

        public GetGameGameView()
        {
            Players = new List<PlayerGetGameGameViewItem>();
        }
    }

    public class GameGetGameGameViewItem
    {
        public int Id { get; set; }

        public int MaxCountRounds { get; set; }
        public int CoinsAtStart { get; set; }
        public int CountBots { get; set; }
        public int Reward { get; set; }
    }

    public class PlayerGetGameGameViewItem
    {
        public int Id { get; set; }

        public string NickName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerRoleEnumView PlayerRole { get; set; }
        public int Coins { get; set; }
    }
}