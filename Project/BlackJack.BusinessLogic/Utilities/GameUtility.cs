using BlackJack.BusinessLogic.Enums;
using BlackJack.BusinessLogic.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.Entities.Enums;
using BlackJack.Shared.Constants;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack.BusinessLogic.Utilities
{
    public class GameUtility : IGameUtility
    {
        #region Properties

        private Dictionary<CardFace, int> _defaultCardValues;
        private Dictionary<SpecialCase, int> _defaultSpecialCaseValues;

        #endregion

        #region Constructors

        public GameUtility()
        {
            _defaultCardValues = new Dictionary<CardFace, int>
            {
                { CardFace.Two, 2 },
                { CardFace.Three, 3 },
                { CardFace.Four, 4 },
                { CardFace.Five, 5 },
                { CardFace.Six, 6 },
                { CardFace.Seven, 7 },
                { CardFace.Eight, 8 },
                { CardFace.Nine, 9 },
                { CardFace.Ten, 10 },
                { CardFace.Jack, 10 },
                { CardFace.Queen, 10 },
                { CardFace.King, 10 },
                { CardFace.Ace, 11 }
            };

            _defaultSpecialCaseValues = new Dictionary<SpecialCase, int>
            {
                { SpecialCase.AceRevert, -10}
            };
        }

        #endregion

        #region Public methods

        public int GetCardValue(CardFace face)
        {
            int value  = _defaultCardValues.FirstOrDefault(item => item.Key == face).Value;

            return value;
        }

        public int CalculateCardsSumm(List<Card> cards)
        {
            int summary = 0;

            bool isAce = cards.Any(item => item.Face == CardFace.Ace);

            foreach (Card c in cards)
            {
                summary += GetCardValue(c.Face);
            }

            if (summary > GameConstants.BLACKJACK && isAce)
            {
                summary += ConvertSpecialCases(SpecialCase.AceRevert);
            }

            return summary;
        }

        public VictoryType CheckTypeOfVictory(List<Card> cards)
        {
            if (CheckGoldenPoint(cards))
            {
                return VictoryType.GoldenPoint;
            }

            if (CheckBlackJack(cards))
            {
                return VictoryType.BlackJack;
            }

            if (CheckBlackJackMoreTwoCards(cards))
            {
                return VictoryType.BlackJackMoreTwoCards;
            }

            if (CheckShortfall(cards))
            {
                return VictoryType.Shortfall;
            }

            return VictoryType.Bust;
        }

        #endregion

        #region Private methods

        private int ConvertSpecialCases(SpecialCase specialCase)
        {
            int value = _defaultSpecialCaseValues.FirstOrDefault(item => item.Key == specialCase).Value;

            return value;
        }

        private bool CheckGoldenPoint(List<Card> cards)
        {
            bool isGold = cards.Count(item => item.Face == CardFace.Ace) == GameConstants.TWO_CARDS && cards.Count == GameConstants.TWO_CARDS;

            return isGold;
        }

        private bool CheckBlackJack(List<Card> cards)
        {
            int summary = CalculateCardsSumm(cards);

            bool isBlackJack = cards.Count == GameConstants.TWO_CARDS && summary == GameConstants.BLACKJACK;

            return isBlackJack;
        }

        private bool CheckBlackJackMoreTwoCards(List<Card> cards)
        {
            int summary = CalculateCardsSumm(cards);

            bool isBlackJackMoreTwoCards = cards.Count > GameConstants.TWO_CARDS && summary == GameConstants.BLACKJACK;

            return isBlackJackMoreTwoCards;
        }

        private bool CheckShortfall(List<Card> cards)
        {
            int summary = CalculateCardsSumm(cards);

            bool isShortfall = summary < GameConstants.BLACKJACK;

            return isShortfall;
        }

        private bool CheckBust(List<Card> cards)
        {
            int summary = CalculateCardsSumm(cards);

            bool isBust = cards.Count > GameConstants.TWO_CARDS && summary > GameConstants.BLACKJACK;

            return isBust;
        }

        #endregion
    }
}