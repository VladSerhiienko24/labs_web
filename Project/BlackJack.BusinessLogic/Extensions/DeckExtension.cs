using BlackJack.BusinessLogic.Models;
using BlackJack.Entities.Entities;
using BlackJack.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack.BusinessLogic.Extensions
{
    public static class DeckExtension
    {
        #region Public Methods

        public static void SetCards(this Deck deck, List<Card> cards)
        {
            deck.Cards = cards;

            deck.UsedCards = new List<Card>();

            deck.HangUpCards = new List<Card>();

            ShuffleDeck(deck);
        }

        public static Card GetCard(this Deck deck)
        {
            Card card = deck.Cards.FirstOrDefault();

            deck.UsedCards.Add(card);

            deck.Cards.Remove(card);

            if (deck.Cards.Count <= GameConstants.AVERAGE_NUMBER_OF_REQUIRED_CARDS)
            {
                AddHangUpToTheDeck(deck);
            }

            return card;
        }

        public static void AddUsedCardsToHangUp(this Deck deck)
        {
            deck.HangUpCards.AddRange(deck.UsedCards);

            deck.UsedCards.Clear();
        }

        #endregion

        #region Private Methods

        private static void AddHangUpToTheDeck(Deck deck)
        {
            deck.Cards.AddRange(deck.HangUpCards);

            deck.HangUpCards.Clear();

            ShuffleDeck(deck);
        }

        private static void ShuffleDeck(Deck deck)
        {
            var random = new Random();

            for (int i = 0; i < deck.Cards.Count; i++)
            {
                int r = random.Next(deck.Cards.Count);

                Card card = deck.Cards[i];

                deck.Cards[i] = deck.Cards[r];

                deck.Cards[r] = card;
            }
        }

        #endregion
    }
}