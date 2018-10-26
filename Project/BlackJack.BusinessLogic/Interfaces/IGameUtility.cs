using BlackJack.Entities.Entities;
using BlackJack.Entities.Enums;
using System.Collections.Generic;

namespace BlackJack.BusinessLogic.Interfaces
{
    public interface IGameUtility
    {
        int GetCardValue(CardFace face);
        int CalculateCardsSumm(List<Card> cards);

        VictoryType CheckTypeOfVictory(List<Card> cards);
    }
}