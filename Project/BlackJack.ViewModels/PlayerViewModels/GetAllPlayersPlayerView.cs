using BlackJack.ViewModels.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace BlackJack.ViewModels.PlayerViewModels
{
    public class GetAllPlayersPlayerView
    {
        public List<GetAllPlayersPlayerViewItem> Players { get; set; }

        public GetAllPlayersPlayerView()
        {
            Players = new List<GetAllPlayersPlayerViewItem>();
        }
    }

    public class GetAllPlayersPlayerViewItem
    {
        public int Id { get; set; }

        public string NickName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerRoleEnumView PlayerRole { get; set; }
        public int Coins { get; set; }

        public int PlayerInGameViewModelId { get; set; }
    }
}