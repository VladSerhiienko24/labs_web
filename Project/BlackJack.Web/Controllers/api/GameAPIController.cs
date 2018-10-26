using BlackJack.BusinessLogic.Interfaces;
using BlackJack.ViewModels.GameViewModels;
using System.Threading.Tasks;
using System.Web.Http;

namespace BlackJack.Web.Controllers.api
{
    [RoutePrefix("api/GameAPI")]
    public class GameAPIController : ApiController
    {
        private IGameLogicService _gameLogicService;

        public GameAPIController(IGameLogicService gameLogicService)
        {
            _gameLogicService = gameLogicService;
        }

        [HttpPost]
        [Route("CreateNewGame")]
        public async Task<int> CreateNewGame(CreateNewGameGameView startGameViewModel)
        {
            int gameId = await _gameLogicService.CreateNewGame(startGameViewModel);

            return gameId;
        }

        [HttpGet]
        [Route("GetGame")]
        public async Task<GetGameGameView> GetGame(int gameId)
        {
            GetGameGameView getGameGameView = await _gameLogicService.GetGame(gameId);

            return getGameGameView;
        }

        [HttpGet]
        [Route("CheckRound")]
        public async Task<bool> CheckRound(int gameId)
        {
            bool response = await _gameLogicService.CheckGameHasAnyRound(gameId);

            return response;
        }

        [HttpGet]
        [Route("CheckGameIsFinished")]
        public async Task<bool> CheckGameIsFinished(int gameId)
        {
            bool response = await _gameLogicService.CheckGameIsFinished(gameId);

            return response;
        }

        [HttpPost]
        [Route("CheckDealPlayer")]
        public async Task<bool> CheckDealPlayer([FromBody]CheckDealPlayerGameView checkDealPlayerGameView)
        {
            bool response = await _gameLogicService.CheckDealPlayer(checkDealPlayerGameView);

            return response;
        }

        [HttpPost]
        [Route("SetDealPlayer")]
        public async Task SetDealPlayer([FromBody]SetDealPlayerGameView setDealPlayerGameView)
        {
            await _gameLogicService.SetDealPlayer(setDealPlayerGameView);
        }

        [HttpPost]
        [Route("SetDealBotAndDealler")]
        public async Task SetDealBotAndDealler([FromBody]SetDealBotAndDeallerGameView setDealBotAndDeallerGameView)
        {
            await _gameLogicService.SetDealBotAndDealler(setDealBotAndDeallerGameView);
        }

        [HttpPost]
        [Route("GetPlayersTwoCards")]
        public async Task<ResponseGetPlayersTwoCardsGameView> GetPlayersTwoCards([FromBody]RequestGetPlayersTwoCardsGameView requestGetPlayersTwoCardsGameView)
        {
            ResponseGetPlayersTwoCardsGameView responseGetPlayersTwoCardsGameView = await _gameLogicService.GetPlayersTwoCards(requestGetPlayersTwoCardsGameView);

            return responseGetPlayersTwoCardsGameView;
        }

        [HttpPost]
        [Route("GetDeallerCard")]
        public async Task<ResponseGetDeallerCardGameView> GetDeallerCard([FromBody]RequestGetDeallerCardGameView requestGetDeallerCardGameView)
        {
            ResponseGetDeallerCardGameView responseGetDeallerCardGameView = await _gameLogicService.GetDeallerCard(requestGetDeallerCardGameView);

            return responseGetDeallerCardGameView;
        }

        [HttpPost]
        [Route("CheckDeallerFirstCard")]
        public async Task<bool> CheckDeallerFirstCard([FromBody]CheckDeallerFirstCardGameView checkDeallerFirstCardGameView)
        {
            bool IsAce = await _gameLogicService.CheckDeallerFirstCard(checkDeallerFirstCardGameView);

            return IsAce;
        }

        [HttpPost]
        [Route("CheckAbilityToInsure")]
        public async Task<bool> CheckAbilityToInsure([FromBody]CheckAbilityToInsureGameView checkAbilityToInsureGameView)
        {
            bool response = await _gameLogicService.CheckAbilityToInsure(checkAbilityToInsureGameView);

            return response;
        }

        [HttpPost]
        [Route("GetInsureCoins")]
        public async Task<int> GetInsureCoins([FromBody]GetInsureCoinsGameView getInsureCoinsGameView)
        {
            int coins = await _gameLogicService.GetInsureCoins(getInsureCoinsGameView);

            return coins;
        }

        [HttpPost]
        [Route("SetInsure")]
        public async Task<bool> SetInsure([FromBody]SetPlayerInsureGameView setPlayerInsureGameView)
        {
            bool response = await _gameLogicService.SetInsure(setPlayerInsureGameView);
            
            return response;
        }

        [HttpPost]
        [Route("CheckPointsPlayer")]
        public async Task<bool> CheckPointsPlayer([FromBody]CheckPointsPlayerGameView checkPointsPlayerGameView)
        {
            bool response = await _gameLogicService.CheckPointsPlayer(checkPointsPlayerGameView);

            return response;
        }

        [HttpPost]
        [Route("GetExtraCard")]
        public async Task<ResponseGetExtraCardGameView> GetExtraCard([FromBody]RequestGetExtraCardGameView requestGetExtraCardView)
        {
            ResponseGetExtraCardGameView responseGetExtraCardView = await _gameLogicService.GetExtraCard(requestGetExtraCardView);

            return responseGetExtraCardView;
        }

        [HttpPost]
        [Route("GetBotExtraCard")]
        public async Task<ResponseGetBotExtraCardGameView> GetBotExtraCard([FromBody]RequestGetBotExtraCardGameView getBotExtraCardView)
        {
            ResponseGetBotExtraCardGameView responseGetBotExtraCardView = await _gameLogicService.GetBotExtraCard(getBotExtraCardView);

            return responseGetBotExtraCardView;
        }

        [HttpPost]
        [Route("GetDeallerExtraCard")]
        public async Task<ResponseGetDeallerExtraCardGameView> GetDeallerExtraCard([FromBody]RequestGetDeallerExtraCardGameView getDeallerExtraCardView)
        {
            ResponseGetDeallerExtraCardGameView responseGetDeallerExtraCardView = await _gameLogicService.GetDeallerExtraCard(getDeallerExtraCardView);

            return responseGetDeallerExtraCardView;
        }

        [HttpGet]
        [Route("GetWinners")]
        public async Task<GetWinnersGameView> GetWinners(int roundId)
        {
            GetWinnersGameView getWinnersGameView = await _gameLogicService.GetWinners(roundId);

            return getWinnersGameView;
        }

        [HttpGet]
        [Route("UpdateFiledCoins")]
        public async Task<int> UpdateFiledCoins(int playerId)
        {
            int coins = await _gameLogicService.UpdateFiledCoins(playerId);

            return coins;
        }

        [HttpGet]
        [Route("GetRound")]
        public async Task<GetRoundGameView> GetRound(int gameId)
        {
            GetRoundGameView getRoundGameView = await _gameLogicService.GetRound(gameId);

            return getRoundGameView;
        }

        [HttpGet]
        [Route("GetHistoryOfWins")]
        public async Task<GetHistoryOfWins> GetHistoryOfWins(int gameId)
        {
            GetHistoryOfWins getHistoryOfWins = await _gameLogicService.GetHistoryOfWins(gameId);

            return getHistoryOfWins;
        }

        [HttpGet]
        [Route("FinishGame")]
        public async Task<bool> FinishGame(int gameId)
        {
            bool response = await _gameLogicService.FinishGame(gameId);

            return response;
        }
    }
}
