using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using BlackJack.ViewModels.Enums;

namespace BlackJack.ViewModels.GameViewModels
{
    public class ResponseGetPlayersTwoCardsGameView
    {
        public List<GetPlayersTwoCardsGameViewItem> ListPlayersWithCards { get; set; }

        public ResponseGetPlayersTwoCardsGameView()
        {
            ListPlayersWithCards = new List<GetPlayersTwoCardsGameViewItem>();
        }
    }

    public class GetPlayersTwoCardsGameViewItem
    {
        public int PlayerId { get; set; }
        public int HandId { get; set; }
        public List<CardGetPlayersTwoCardsGameViewItemItem> Cards { get; set; }
        public int Summary { get; set; }

        public GetPlayersTwoCardsGameViewItem()
        {
            Cards = new List<CardGetPlayersTwoCardsGameViewItemItem>();
        }
    }

    public class CardGetPlayersTwoCardsGameViewItemItem
    {
        public int Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardSuitEnumView Suit { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CardFaceEnumView Face { get; set; }

        public int? HandId { get; set; }
    }
}