using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BlackJack.ViewModels.Enums;

namespace BlackJack.ViewModels.GameViewModels
{
    public class ResponseGetExtraCardGameView
    {
        public ResponseGetExtraCardGameViewItem Card { get; set; }
        public int Summary { get; set; }
    }

    public class ResponseGetExtraCardGameViewItem
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardSuitEnumView Suit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CardFaceEnumView Face { get; set; }
    }
}