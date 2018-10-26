using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BlackJack.ViewModels.Enums;

namespace BlackJack.ViewModels.GameViewModels
{
    public class ResponseGetDeallerCardGameView
    {
        public CardGetDeallerCardGameViewItemItem Card { get; set; }
        public int HandId { get; set; }
        public int Summary { get; set; }
    }
    
    public class CardGetDeallerCardGameViewItemItem
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardSuitEnumView Suit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CardFaceEnumView Face { get; set; }

        public int? HandId { get; set; }
    }
}