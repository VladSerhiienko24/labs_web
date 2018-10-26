using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using BlackJack.ViewModels.Enums;

namespace BlackJack.ViewModels.GameViewModels
{
    public class ResponseGetBotExtraCardGameView
    {
        public List<ResponseGetBotExtraCardGameViewItem> ExtraCards { get; set; }

        public ResponseGetBotExtraCardGameView()
        {
            ExtraCards = new List<ResponseGetBotExtraCardGameViewItem>();
        }
    }

    public class ResponseGetBotExtraCardGameViewItem
    {
        public CardGetBotExtraCardGameViewItem Card { get; set; }
        public int Summary { get; set; }
        public int Chance { get; set; }
    }

    public class CardGetBotExtraCardGameViewItem
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardSuitEnumView Suit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CardFaceEnumView Face { get; set; }
    }
}