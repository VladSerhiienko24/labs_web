using BlackJack.ViewModels.GameViewModels;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Interfaces
{
    public interface IGameLogicService
    {
        Task<int> CreateNewGame(CreateNewGameGameView startGameViewModel);

        Task<GetGameGameView> GetGame(int gameId);
        Task<bool> CheckGameIsFinished(int gameId);
        Task<bool> CheckGameHasAnyRound(int gameId);

        Task<bool> CheckDealPlayer(CheckDealPlayerGameView checkDealPlayerGameView);
        Task SetDealPlayer(SetDealPlayerGameView setDealPlayerGameView);
        Task SetDealBotAndDealler(SetDealBotAndDeallerGameView setDealBotAndDeallerGameView);

        Task<GetRoundGameView> GetRound(int gameId);

        Task<ResponseGetPlayersTwoCardsGameView> GetPlayersTwoCards(RequestGetPlayersTwoCardsGameView requestGetPlayersTwoCardsGameView);
        Task<ResponseGetDeallerCardGameView> GetDeallerCard(RequestGetDeallerCardGameView requestGetDeallerTwoCardsGameView);

        Task<bool> CheckDeallerFirstCard(CheckDeallerFirstCardGameView checkDeallerFirstCardGameView);
        Task<bool> CheckAbilityToInsure(CheckAbilityToInsureGameView checkAbilityToInsureGameView);
        Task<int> GetInsureCoins(GetInsureCoinsGameView getInsureCoinsGameView);
        Task<bool> SetInsure(SetPlayerInsureGameView setPlayerInsureGameView);

        Task<bool> CheckPointsPlayer(CheckPointsPlayerGameView checkPointsPlayerGameView);
        Task<ResponseGetExtraCardGameView> GetExtraCard(RequestGetExtraCardGameView requestGetExtraCardView);
        Task<ResponseGetDeallerExtraCardGameView> GetDeallerExtraCard(RequestGetDeallerExtraCardGameView getDeallerExtraCardView);
        Task<ResponseGetBotExtraCardGameView> GetBotExtraCard(RequestGetBotExtraCardGameView getBotExtraCardView);

        Task<GetWinnersGameView> GetWinners(int gameId);

        Task<int> UpdateFiledCoins(int playerId);

        Task<GetHistoryOfWins> GetHistoryOfWins(int gameId);

        Task<bool> FinishGame(int gameId);
    }
}