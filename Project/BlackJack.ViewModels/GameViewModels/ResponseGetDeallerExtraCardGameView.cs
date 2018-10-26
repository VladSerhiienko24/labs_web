using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using BlackJack.ViewModels.Enums;

namespace BlackJack.ViewModels.GameViewModels
{
    public class ResponseGetDeallerExtraCardGameView
    {
        public List<ResponseGetDeallerExtraCardGameViewItem> ExtraCards { get; set; }

        public ResponseGetDeallerExtraCardGameView()
        {
            ExtraCards = new List<ResponseGetDeallerExtraCardGameViewItem>();
        }
    }

    public class ResponseGetDeallerExtraCardGameViewItem
    {
        public CardGetDeallerExtraCardGameViewItem Card { get; set; }
        public int Summary { get; set; }
        public int Chance { get; set; }
    }

    public class CardGetDeallerExtraCardGameViewItem
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardSuitEnumView Suit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CardFaceEnumView Face { get; set; }
    }
}