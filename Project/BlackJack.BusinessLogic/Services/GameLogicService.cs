using AutoMapper;
using BlackJack.BusinessLogic.Enums;
using BlackJack.BusinessLogic.Extensions;
using BlackJack.BusinessLogic.Interfaces;
using BlackJack.BusinessLogic.Models;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.Entities.Enums;
using BlackJack.Exceptions.BusinessLogicExceptions;
using BlackJack.Shared.Constants;
using BlackJack.ViewModels.GameViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Services
{
    public class GameLogicService : IGameLogicService
    {
        #region Properties

        private IGameRepository _gameRepository;
        private IPlayerRepository _playerRepository;
        private IRoundRepository _roundRepository;
        private IHandRepository _handRepository;
        private IPlayerGameRepository _playerGameRepository;
        private ICardRepository _cardRepository;
        private IHandCardRepository _handCardRepository;

        private IMapper _mapper;

        private IGameUtility _gameUtility;
        private IDeckProvider _deckProvider;

        #endregion

        #region Constructors

        public GameLogicService(IGameRepository gameRepository, IPlayerRepository playerRepository,
            ICardRepository cardRepository, IPlayerGameRepository playerGameRepository,
            IRoundRepository roundRepository, IHandRepository handRepository, IHandCardRepository handCardRepository,
            IMapper mapper, IGameUtility gameUtility, IDeckProvider deckProvider)
        {
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _playerGameRepository = playerGameRepository;
            _roundRepository = roundRepository;
            _handRepository = handRepository;
            _cardRepository = cardRepository;
            _handCardRepository = handCardRepository;

            _mapper = mapper;

            _gameUtility = gameUtility;
            _deckProvider = deckProvider;
        }

        #endregion 

        #region Public Methods

        public async Task<int> CreateNewGame(CreateNewGameGameView createNewGameGameView)
        {
            Game game = _mapper.Map<CreateNewGameGameView, Game>(createNewGameGameView);

            game.GameStart = DateTime.UtcNow;

            await _gameRepository.Create(game);

            await CreatePlayer(game.Id, createNewGameGameView);

            if (createNewGameGameView.CountBots > 0 && createNewGameGameView.CountBots <= GameConstants.MAX_COUNT_OF_BOTS)
            {
                await CreateBots(game.Id, createNewGameGameView);
            }

            await CreateDealler(createNewGameGameView.CoinsAtStart, game.Id);

            return game.Id;
        }

        public async Task<GetGameGameView> GetGame(int gameId)
        {
            var gameView = new GetGameGameView();

            gameView.Game = await GetGameById(gameId);

            gameView.Players = await GetListPlayers(gameId);

            return gameView;
        }

        public async Task<ResponseGetPlayersTwoCardsGameView> GetPlayersTwoCards(RequestGetPlayersTwoCardsGameView requestGetPlayersTwoCardsGameView)
        {
            Round round = await _roundRepository.Get(requestGetPlayersTwoCardsGameView.RoundId);

            Game game = await _gameRepository.GetGameWithPlayerGames(round.GameId);

            if (round == null || game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", requestGetPlayersTwoCardsGameView.RoundId.ToString()));

                for (int i = 0; i < requestGetPlayersTwoCardsGameView.Players.Count; i++)
                {
                    stringBuilder.AppendLine(string.Format("PlayerId: {0}", requestGetPlayersTwoCardsGameView.Players[i]));
                }

                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            await GiveOutTwoCards(game.Id, requestGetPlayersTwoCardsGameView);

            List<Hand> listHands = await _handRepository.GetListHandsWithCardsWithoutDeallerHandByRoundId(round.Id);

            if (listHands == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", round.Id));

                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            var responseView = new ResponseGetPlayersTwoCardsGameView();

            for (int i = 0; i < listHands.Count; i++)
            {
                List<Card> cards = listHands[i].HandCards.Select(item => item.Card).ToList();

                var responseViewItem = new GetPlayersTwoCardsGameViewItem();

                responseViewItem.PlayerId = listHands[i].PlayerId;
                responseViewItem.HandId = listHands[i].Id;
                responseViewItem.Cards = _mapper.Map<List<Card>, List<CardGetPlayersTwoCardsGameViewItemItem>>(cards);
                responseViewItem.Summary = listHands[i].Summary;

                responseView.ListPlayersWithCards.Add(responseViewItem);
            }

            return responseView;
        }

        public async Task<ResponseGetDeallerCardGameView> GetDeallerCard(RequestGetDeallerCardGameView requestGetDeallerCardGameView)
        {
            Game game = await _gameRepository.GetGameWithPlayerGames(requestGetDeallerCardGameView.GameId);

            Hand hand = await _handRepository.GetHandWithCardsByRoundAndPlayerId(requestGetDeallerCardGameView.RoundId, requestGetDeallerCardGameView.PlayerId);

            if (game == null || hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", requestGetDeallerCardGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", requestGetDeallerCardGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("GameId: {0}", requestGetDeallerCardGameView.GameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            Deck infinityDeck = await _deckProvider.GetAllDeckFromCache(game.Id);

            List<Card> handCards = hand.HandCards.Select(item => item.Card).ToList();

            Card card = DeckExtension.GetCard(infinityDeck);

            handCards.Add(card);

            hand.Summary += _gameUtility.GetCardValue(card.Face);

            if (handCards.Count == GameConstants.TWO_CARDS)
            {
                hand.VictoryType = _gameUtility.CheckTypeOfVictory(handCards);

                if (hand.VictoryType == VictoryType.GoldenPoint)
                {
                    hand.Summary = GameConstants.BLACKJACK;
                }
            }

            await _handRepository.Update(hand);

            var handCard = new HandCard();

            handCard.CardId = card.Id;
            handCard.HandId = hand.Id;

            await _handCardRepository.Create(handCard);

            _deckProvider.SetDeckInMemoryCashe(game.Id, infinityDeck);

            var responseView = new ResponseGetDeallerCardGameView();

            responseView.HandId = hand.Id;
            responseView.Card = _mapper.Map<Card, CardGetDeallerCardGameViewItemItem>(card);
            responseView.Summary = hand.Summary;

            return responseView;
        }

        public async Task<bool> CheckDeallerFirstCard(CheckDeallerFirstCardGameView checkDeallerFirstCardGameView)
        {
            Hand hand = await _handRepository.GetHandWithCardsByRoundAndPlayerId(checkDeallerFirstCardGameView.RoundId, checkDeallerFirstCardGameView.PlayerId);

            if (hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", checkDeallerFirstCardGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", checkDeallerFirstCardGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            Card card = hand.HandCards.Select(item => item.Card).First();

            bool result = card.Face == CardFace.Ace;

            return result;
        }

        public async Task<bool> CheckAbilityToInsure(CheckAbilityToInsureGameView checkAbilityToInsureGameView)
        {
            Player player = await _playerRepository.Get(checkAbilityToInsureGameView.PlayerId);

            Hand hand = await _handRepository.GetHandByRoundAndPlayerId(checkAbilityToInsureGameView.RoundId, checkAbilityToInsureGameView.PlayerId);

            if (player == null || hand == null)
            {
                return false;
            }

            var result = false;

            int insurance = hand.Deal / GameConstants.INSURE_DEVISION;

            if ((player.Coins - hand.Deal - insurance) >= 0 && insurance != 0)
            {
                result = true;
            }

            return result;
        }

        public async Task<int> GetInsureCoins(GetInsureCoinsGameView getInsureCoinsGameView)
        {
            Player player = await _playerRepository.Get(getInsureCoinsGameView.PlayerId);

            Hand hand = await _handRepository.GetHandByRoundAndPlayerId(getInsureCoinsGameView.RoundId, getInsureCoinsGameView.PlayerId);

            if (player == null || hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", getInsureCoinsGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", getInsureCoinsGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            int insurance = hand.Deal / GameConstants.INSURE_DEVISION;

            return insurance;
        }

        public async Task<bool> SetInsure(SetPlayerInsureGameView setPlayerInsureGameView)
        {
            Hand hand = await _handRepository.GetHandByRoundAndPlayerId(setPlayerInsureGameView.RoundId, setPlayerInsureGameView.PlayerId);

            if (hand == null)
            {
                return false;
            }

            int insuranceCoins = hand.Deal / GameConstants.INSURE_DEVISION;

            hand.InsuranceCoins = insuranceCoins;

            await _handRepository.Update(hand);

            return true;

        }

        public async Task<ResponseGetExtraCardGameView> GetExtraCard(RequestGetExtraCardGameView requestGetExtraCardGameView)
        {
            Game game = await _gameRepository.GetGameWithPlayerGames(requestGetExtraCardGameView.GameId);

            Hand hand = await _handRepository.GetWithCards(requestGetExtraCardGameView.HandId);

            if (game == null || hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", requestGetExtraCardGameView.GameId));
                stringBuilder.AppendLine(string.Format("HandId: {0}", requestGetExtraCardGameView.HandId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            Deck infinityDeck = await _deckProvider.GetAllDeckFromCache(game.Id);

            List<Card> handCards = hand.HandCards.Select(item => item.Card).ToList();

            Card card = DeckExtension.GetCard(infinityDeck);

            _deckProvider.SetDeckInMemoryCashe(game.Id, infinityDeck);

            handCards.Add(card);

            hand.Summary = _gameUtility.CalculateCardsSumm(handCards);

            hand.VictoryType = _gameUtility.CheckTypeOfVictory(handCards);

            var handCard = new HandCard();

            handCard.CardId = card.Id;
            handCard.HandId = hand.Id;

            await _handCardRepository.Create(handCard);

            await _handRepository.Update(hand);

            ResponseGetExtraCardGameViewItem responseViewItem = _mapper.Map<Card, ResponseGetExtraCardGameViewItem>(card);

            var responseView = new ResponseGetExtraCardGameView();

            responseView.Card = responseViewItem;
            responseView.Summary = hand.Summary;

            return responseView;
        }

        public async Task<ResponseGetBotExtraCardGameView> GetBotExtraCard(RequestGetBotExtraCardGameView requestGetBotExtraCardGameView)
        {
            Hand hand = await _handRepository.GetWithCards(requestGetBotExtraCardGameView.HandId);

            if (hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", requestGetBotExtraCardGameView.GameId));
                stringBuilder.AppendLine(string.Format("HandId: {0}", requestGetBotExtraCardGameView.HandId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            RequestGetExtraCardGameView requestExtraCardView = _mapper.Map<RequestGetBotExtraCardGameView, RequestGetExtraCardGameView>(requestGetBotExtraCardGameView);

            int chance = await CountChanceToGetNeededExtraCardForBot(requestGetBotExtraCardGameView.GameId);

            var extraCards = new List<ResponseGetBotExtraCardGameViewItem>();

            for (int i = hand.Summary; i < GameConstants.BLACKJACK;)
            {
                i = _gameUtility.CalculateCardsSumm(hand.HandCards.Select(item => item.Card).ToList());

                if (i <= GameConstants.REQUIRED_SUMM_CARDS_BOT)
                {
                    ResponseGetExtraCardGameView extraCardView = await GetExtraCard(requestExtraCardView);

                    ResponseGetBotExtraCardGameViewItem extraCardViewItem = _mapper.Map<ResponseGetExtraCardGameView, ResponseGetBotExtraCardGameViewItem>(extraCardView);

                    extraCards.Add(extraCardViewItem);

                    chance = await CountChanceToGetNeededExtraCardForBot(requestGetBotExtraCardGameView.GameId);

                    hand = await _handRepository.GetWithCards(hand.Id);

                    continue;
                }

                if (i <= GameConstants.DESIRED_SUMM_CARDS_BOT_AND_DEALLER && chance < 0)
                {
                    ResponseGetExtraCardGameView extraCardView = await GetExtraCard(requestExtraCardView);

                    ResponseGetBotExtraCardGameViewItem extraCardViewItem = _mapper.Map<ResponseGetExtraCardGameView, ResponseGetBotExtraCardGameViewItem>(extraCardView);

                    extraCards.Add(extraCardViewItem);

                    chance = await CountChanceToGetNeededExtraCardForBot(requestGetBotExtraCardGameView.GameId);

                    hand = await _handRepository.GetWithCards(hand.Id);

                    continue;
                }

                if (i <= GameConstants.DESIRED_SUMM_CARDS_BOT_AND_DEALLER && chance >= 0)
                {
                    var random = new Random();

                    int randValue = random.Next(GameConstants.DECISION_TO_TAKE_CARD, GameConstants.DECISION_NOT_TO_TAKE_CARD);

                    if (randValue.Equals(GameConstants.DECISION_TO_TAKE_CARD))
                    {
                        ResponseGetExtraCardGameView extraCardView = await GetExtraCard(requestExtraCardView);

                        ResponseGetBotExtraCardGameViewItem extraCardViewItem = _mapper.Map<ResponseGetExtraCardGameView, ResponseGetBotExtraCardGameViewItem>(extraCardView);

                        extraCards.Add(extraCardViewItem);

                        chance = await CountChanceToGetNeededExtraCardForBot(requestGetBotExtraCardGameView.GameId);

                        hand = await _handRepository.GetWithCards(hand.Id);

                        continue;
                    }

                    if (randValue.Equals(GameConstants.DECISION_NOT_TO_TAKE_CARD))
                    {
                        break;
                    }
                }

                break;
            }

            var responseView = new ResponseGetBotExtraCardGameView();

            responseView.ExtraCards = extraCards;

            return responseView;
        }

        public async Task<ResponseGetDeallerExtraCardGameView> GetDeallerExtraCard(RequestGetDeallerExtraCardGameView requestGetDeallerExtraCardGameView)
        {
            Hand hand = await _handRepository.GetWithCards(requestGetDeallerExtraCardGameView.HandId);

            if (hand == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", requestGetDeallerExtraCardGameView.GameId));
                stringBuilder.AppendLine(string.Format("HandId: {0}", requestGetDeallerExtraCardGameView.HandId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            int playerPoints = hand.Summary;

            var extraCards = new List<ResponseGetDeallerExtraCardGameViewItem>();
            
            var responseView = new ResponseGetDeallerExtraCardGameView();

            if (playerPoints >= GameConstants.DESIRED_SUMM_CARDS_BOT_AND_DEALLER)
            {
                responseView.ExtraCards = extraCards;

                return responseView;
            }

            RequestGetExtraCardGameView requestGetExtraCardView = _mapper.Map<RequestGetDeallerExtraCardGameView, RequestGetExtraCardGameView>(requestGetDeallerExtraCardGameView);

            for (int i = playerPoints; i < GameConstants.DESIRED_SUMM_CARDS_BOT_AND_DEALLER;)
            {
                ResponseGetExtraCardGameView extraCardView = await GetExtraCard(requestGetExtraCardView);

                ResponseGetDeallerExtraCardGameViewItem extraCardViewItem = _mapper.Map<ResponseGetExtraCardGameView, ResponseGetDeallerExtraCardGameViewItem>(extraCardView);

                extraCards.Add(extraCardViewItem);

                hand = await _handRepository.GetWithCards(hand.Id);

                i = _gameUtility.CalculateCardsSumm(hand.HandCards.Select(item => item.Card).ToList());
            }

            responseView.ExtraCards = extraCards;

            return responseView;
        }

        public async Task<bool> CheckDealPlayer(CheckDealPlayerGameView checkDealPlayerGameView)
        {
            Player player = await _playerRepository.Get(checkDealPlayerGameView.PlayerId);

            if (player == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", checkDealPlayerGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", checkDealPlayerGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            bool result = player.Coins >= checkDealPlayerGameView.Coins;

            return result;
        }

        public async Task<bool> CheckPointsPlayer(CheckPointsPlayerGameView checkPointsPlayerGameView)
        {
            Hand hand = await _handRepository.GetHandByRoundAndPlayerId(checkPointsPlayerGameView.RoundId, checkPointsPlayerGameView.PlayerId);

            bool result = hand != null && hand.Summary < GameConstants.BLACKJACK;

            return result;
        }

        public async Task SetDealPlayer(SetDealPlayerGameView setDealPlayerGameView)
        {
            Player player = await _playerRepository.Get(setDealPlayerGameView.PlayerId);
            Round round = await _roundRepository.Get(setDealPlayerGameView.RoundId);

            if (player == null || round == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", setDealPlayerGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", setDealPlayerGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            var hand = new Hand();

            hand.Summary = GameConstants.DEFAULT_SUMMARY_AT_START;
            hand.Situation = Situation.None;
            hand.Deal = setDealPlayerGameView.Coins;
            hand.PlayerId = player.Id;
            hand.RoundId = round.Id;

            await _handRepository.Create(hand);
        }

        public async Task SetDealBotAndDealler(SetDealBotAndDeallerGameView setDealBotAndDeallerGameView)
        {
            Player player = await _playerRepository.Get(setDealBotAndDeallerGameView.PlayerId);
            Round round = await _roundRepository.Get(setDealBotAndDeallerGameView.RoundId);

            if (player == null || round == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", setDealBotAndDeallerGameView.RoundId));
                stringBuilder.AppendLine(string.Format("PlayerId: {0}", setDealBotAndDeallerGameView.PlayerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            if (player.Coins < GameConstants.BOTS_MINIMUM_COINS)
            {
                player.Coins = GameConstants.BOTS_COINS;

                await _playerRepository.Update(player);
            }

            var random = new Random();

            int dealCoins = random.Next(GameConstants.MINIMUM_DEAL_COINS_FOR_BOT, GameConstants.MAXIMUM_DEAL_COINS_FOR_BOT);

            var hand = new Hand();

            hand.Summary = GameConstants.DEFAULT_SUMMARY_AT_START;
            hand.Situation = Situation.None;
            hand.Deal = dealCoins;
            hand.PlayerId = player.Id;
            hand.RoundId = round.Id;

            await _handRepository.Create(hand);
        }

        public async Task<bool> CheckGameIsFinished(int gameId)
        {
            Game game = await _gameRepository.Get(gameId);

            if (game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            bool result = game.IsFinished;

            return result;
        }

        public async Task<GetRoundGameView> GetRound(int gameId)
        {
            Game game = await _gameRepository.Get(gameId);

            if (game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            var responseView = new GetRoundGameView();

            if (!game.IsFinished)
            {
                Round round = await _roundRepository.GetLastRound(gameId);

                var newRound = new Round();

                newRound.GameId = gameId;

                newRound.NumberOfRound = round != null ? (round.NumberOfRound + GameConstants.NEXT_ROUND) : GameConstants.FIRST_ROUND;

                await SetFinishedGameIfEndedRounds(newRound.NumberOfRound, game);

                await _roundRepository.Create(newRound);

                responseView.RoundId = newRound.Id;
                responseView.NumberOfRound = newRound.NumberOfRound;
            }

            return responseView;
        }

        public async Task<bool> CheckGameHasAnyRound(int gameId)
        {
            Round round = await _roundRepository.GetLastRound(gameId);

            bool result = round != null;

            return result;
        }

        public async Task<int> UpdateFiledCoins(int playerId)
        {
            Player player = await _playerRepository.Get(playerId);

            if (player == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("PlayerId: {0}", playerId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            int coins = player.Coins;

            return coins;
        }

        public async Task<GetWinnersGameView> GetWinners(int roundId)
        {
            Round round = await _roundRepository.GetRoundIncludeHandsAndPlayers(roundId);

            if (round == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("RoundId: {0}", roundId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            List<Hand> allHands = round.Hands.ToList();

            Hand deallerHand = allHands.FirstOrDefault(item => item.Player.PlayerRole == PlayerRole.Dealler);

            List<Hand> playersHands = allHands.Where(item => item.Player.PlayerRole != PlayerRole.Dealler).ToList();

            bool checkGameIsFinished = await CheckGameIsFinished(round.GameId);

            foreach (Hand hand in playersHands)
            {
                SetSituation(hand, deallerHand);
            }

            await _handRepository.UpdateMultiple(playersHands);

            await CalculateCashFlow(deallerHand, playersHands);

            Deck infinityDeck = await _deckProvider.GetAllDeckFromCache(round.GameId);

            DeckExtension.AddUsedCardsToHangUp(infinityDeck);

            _deckProvider.SetDeckInMemoryCashe(round.GameId, infinityDeck);

            Hand playerHand = playersHands.FirstOrDefault(item => item.Player.PlayerRole == PlayerRole.Player);

            var responseView = new GetWinnersGameView();

            if (checkGameIsFinished)
            {
                List<GetWinnersGameViewItem> winnersViewItems = await DevideRewards(round.GameId);

                responseView.WinnersOfGame = winnersViewItems;
            }

            SetGetWinnersGameView(playerHand, deallerHand, responseView);

            return responseView;
        }

        public async Task<GetHistoryOfWins> GetHistoryOfWins(int gameId)
        {
            List<Round> rounds = await _roundRepository.GetAllRoundsByGameId(gameId);

            if (rounds == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            var responseView = new GetHistoryOfWins();

            foreach (Round round in rounds)
            {
                SetHistoryOfWinsViewItem(responseView, round);
            }

            return responseView;
        }

        public async Task<bool> FinishGame(int gameId)
        {
            Game game = await _gameRepository.Get(gameId);

            if (game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            game.IsFinished = true;

            await _gameRepository.Update(game);

            return true;
        }

        #endregion

        #region Private Methods

        private async Task GiveOutTwoCards(int gameId, RequestGetPlayersTwoCardsGameView requestView)
        {
            Deck infinityDeck = await _deckProvider.GetAllDeckFromCache(gameId);

            var hands = new List<Hand>();

            var handCards = new List<HandCard>();

            for (int i = 0; i < requestView.Players.Count; i++)
            {
                Hand hand = await _handRepository.GetHandByRoundAndPlayerId(requestView.RoundId, requestView.Players[i]);

                if (hand == null)
                {
                    continue;
                }

                int summary = 0;

                var cards = new List<Card>();

                for (int y = 0; y < GameConstants.TWO_CARDS; y++)
                {
                    Card card = DeckExtension.GetCard(infinityDeck);

                    cards.Add(card);

                    var handCard = new HandCard();

                    handCard.CardId = card.Id;
                    handCard.HandId = hand.Id;

                    handCards.Add(handCard);

                    summary += _gameUtility.GetCardValue(card.Face);
                }

                hand.Summary = summary;

                hand.VictoryType = _gameUtility.CheckTypeOfVictory(cards);

                if (hand.VictoryType == VictoryType.GoldenPoint)
                {
                    hand.Summary = GameConstants.BLACKJACK;
                }

                hands.Add(hand);
            }

            await _handRepository.UpdateMultiple(hands);

            await _handCardRepository.CreateMultiple(handCards);

            _deckProvider.SetDeckInMemoryCashe(gameId, infinityDeck);
        }

        private void SetSituation(Hand playerHand, Hand deallerHand)
        {
            if (deallerHand.VictoryType > playerHand.VictoryType
                    && deallerHand.VictoryType != VictoryType.None
                    && playerHand.VictoryType != VictoryType.None)
            {
                playerHand.Situation = Situation.Win;

                return;
            }

            if (deallerHand.VictoryType < playerHand.VictoryType
                && deallerHand.VictoryType != VictoryType.None
                && playerHand.VictoryType != VictoryType.None)
            {
                playerHand.Situation = Situation.Lose;

                return;
            }

            int deallerHandSumm = deallerHand.Summary;
            int playerHandSumm = playerHand.Summary;

            if (deallerHand.VictoryType == playerHand.VictoryType
                && (playerHand.VictoryType == VictoryType.BlackJackMoreTwoCards || deallerHandSumm == playerHandSumm))
            {
                playerHand.Situation = Situation.Drawn;
                return;
            }
            if (deallerHand.VictoryType == playerHand.VictoryType &&
                (playerHand.VictoryType == VictoryType.Shortfall && playerHandSumm > deallerHandSumm) ||
                    (playerHand.VictoryType == VictoryType.Bust && playerHandSumm < deallerHandSumm))
            {
                playerHand.Situation = Situation.Win;
                return;
            }

            if (deallerHand.VictoryType == playerHand.VictoryType &&
                (playerHand.VictoryType == VictoryType.Shortfall && playerHandSumm < deallerHandSumm) ||
                (playerHand.VictoryType == VictoryType.Bust && playerHandSumm > deallerHandSumm))
            {
                playerHand.Situation = Situation.Lose;
                return;
            }

        }

        private void SetHistoryOfWinsViewItem(GetHistoryOfWins responseView, Round round)
        {
            var viewItem = new GetHistoryOfWinsItem();

            viewItem.RoundNumber = round.NumberOfRound;

            foreach (Hand hand in round.Hands)
            {
                if (hand.Situation == Situation.Win)
                {
                    var viewItemItem = new GetHistoryOfWinsItemItem();

                    viewItemItem.NickName = hand.Player.NickName;
                    viewItemItem.Points = hand.Summary;

                    viewItem.Players.Add(viewItemItem);
                }
            }

            responseView.Winners.Add(viewItem);
        }

        private void SetGetWinnersGameView(Hand playerHand, Hand deallerHand, GetWinnersGameView responseView)
        {
            if (playerHand == null || playerHand.Situation == Situation.Drawn)
            {
                responseView.NickName = String.Empty;
                responseView.Points = 0;
            }

            if (playerHand != null && playerHand.Situation == Situation.Win)
            {
                responseView.NickName = playerHand.Player.NickName;
                responseView.Points = playerHand.Summary;
            }

            if (playerHand != null && playerHand.Situation == Situation.Lose)
            {
                responseView.NickName = deallerHand.Player.NickName;
                responseView.Points = deallerHand.Summary;
            }
        }

        private async Task SetFinishedGameIfEndedRounds(int currentNumberRound, Game game)
        {
            if (currentNumberRound >= game.MaxCountRounds)
            {
                game.IsFinished = true;

                await _gameRepository.Update(game);
            }
        }

        private async Task CreatePlayer(int gameId, CreateNewGameGameView createNewGameGameView)
        {
            var player = new Player();

            player.NickName = createNewGameGameView.NickName;
            player.PlayerRole = PlayerRole.Player;
            player.Coins = createNewGameGameView.CoinsAtStart;

            await _playerRepository.Create(player);

            var playerGame = new PlayerGame();

            playerGame.GameId = gameId;
            playerGame.PlayerId = player.Id;

            await _playerGameRepository.Create(playerGame);
        }

        private async Task CreateDealler(int coins, int gameId)
        {
            Player dealler = await _playerRepository.GetFreeDealler();

            if (dealler == null)
            {
                dealler = new Player();

                dealler.NickName = GameConstants.DEALLER_NICKNAME;
                dealler.PlayerRole = PlayerRole.Dealler;
                dealler.Coins = GameConstants.BOTS_COINS;

                await _playerRepository.Create(dealler);
            }

            var playerGame = new PlayerGame();

            playerGame.GameId = gameId;
            playerGame.PlayerId = dealler.Id;

            await _playerGameRepository.Create(playerGame);
        }

        private async Task CreateBots(int gameId, CreateNewGameGameView createNewGameGameView)
        {
            List<Player> bots = await _playerRepository.GetAllFreeBots(createNewGameGameView.CountBots);
            
            int countBots = bots.Count;

            if (createNewGameGameView.CountBots <= countBots)
            {
                var playerGames = new List<PlayerGame>();

                for (int i = 0; i < createNewGameGameView.CountBots; i++)
                {
                    var playerGame = new PlayerGame();

                    playerGame.GameId = gameId;
                    playerGame.PlayerId = bots[i].Id;

                    playerGames.Add(playerGame);
                }

                await _playerGameRepository.CreateMultiple(playerGames);
            }

            if (createNewGameGameView.CountBots > countBots)
            {
                var playerGames = new List<PlayerGame>();

                int difference = createNewGameGameView.CountBots - countBots;

                for (int i = 0; i < countBots; i++)
                {
                    var playerGame = new PlayerGame();

                    playerGame.GameId = gameId;
                    playerGame.PlayerId = bots[i].Id;

                    playerGames.Add(playerGame);
                }

                List<string> names = Enum.GetNames(typeof(BotName)).ToList();

                var random = new Random();

                var listBots = new List<Player>();

                for (int i = 0; i < difference; i++)
                {
                    string nickName = names[random.Next(GameConstants.FIRST_ELEMENT_IN_BOTS_NAME_ENUM, names.Count)];

                    var bot = new Player();

                    bot.NickName = nickName;
                    bot.PlayerRole = PlayerRole.Bot;
                    bot.Coins = GameConstants.BOTS_COINS;

                    listBots.Add(bot);
                }

                await _playerRepository.CreateMultiplePlayersAndReturnTheirIds(listBots);

                foreach (Player bot in listBots)
                {
                    var playerGame = new PlayerGame();

                    playerGame.GameId = gameId;
                    playerGame.PlayerId = bot.Id;

                    playerGames.Add(playerGame);
                }

                await _playerGameRepository.CreateMultiple(playerGames);
            }

        }

        private async Task<GameGetGameGameViewItem> GetGameById(int gameId)
        {
            Game game = await _gameRepository.Get(gameId);

            if (game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            GameGetGameGameViewItem viewItem = _mapper.Map<Game, GameGetGameGameViewItem>(game);

            return viewItem;
        }

        private async Task<List<PlayerGetGameGameViewItem>> GetListPlayers(int gameId)
        {
            Game game = await _gameRepository.GetGameWithPlayerGames(gameId);

            if (game == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            IEnumerable<Player> players = game.PlayerGames.Select(item => item.Player);

            List<Player> sortedPlayers = players.OrderBy(item => item.PlayerRole).ToList();

            List<PlayerGetGameGameViewItem> viewItems = _mapper.Map<List<Player>, List<PlayerGetGameGameViewItem>>(sortedPlayers);

            return viewItems;
        }

        private async Task<int> CountChanceToGetNeededExtraCardForBot(int gameId)
        {
            int chance = 0;

            Deck infinityDeck = await _deckProvider.GetAllDeckFromCache(gameId);

            for (int i = 0; i < infinityDeck.HangUpCards.Count; i++)
            {
                int valueCard = _gameUtility.GetCardValue(infinityDeck.HangUpCards[i].Face);

                if (valueCard <= GameConstants.MIN_CARD_VALUE_CHANCE)
                {
                    chance++;
                }

                if (valueCard >= GameConstants.MAX_CARD_VALUE_CHANCE)
                {
                    chance--;
                }
            }

            return chance;
        }

        private async Task CalculateCashFlow(Hand deallerHand, List<Hand> playerHands)
        {
            var players = new List<Player>();

            foreach (Hand playerHand in playerHands)
            {
                Player player = playerHand.Player;

                players.Add(player);

                if (playerHand.InsuranceCoins > 0 && deallerHand.VictoryType == VictoryType.BlackJack)
                {
                    player.Coins += playerHand.InsuranceCoins * GameConstants.INCREASE_VALUE;

                    continue;
                }

                if (playerHand.InsuranceCoins > 0)
                {
                    player.Coins -= playerHand.InsuranceCoins;
                }

                if (playerHand.Situation == Situation.Win)
                {
                    player.Coins += playerHand.Deal * GameConstants.INCREASE_VALUE;
                }

                if (playerHand.Situation == Situation.Lose)
                {
                    player.Coins -= playerHand.Deal;
                }

            }

            await _playerRepository.UpdateMultiple(players);
        }

        private async Task<List<GetWinnersGameViewItem>> DevideRewards(int gameId)
        {
            Game game = await _gameRepository.Get(gameId);

            List<Player> players = await _playerRepository.GetAllPlayersWithoutDeallerByGameId(gameId);

            if (game == null || players == null)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(string.Format("GameId: {0}", gameId));
                stringBuilder.AppendLine(string.Format("Message: {0}", SolutionConstants.BUSINESS_LOGIC_GET_ITEM_EXCEPTION_MESSAGE));

                string message = stringBuilder.ToString();

                throw new BusinessLogicGetItemException(message);
            }

            Dictionary<Player, int> playerWins = await _handRepository.GetListWithCountsOfWinsForAllGame(game.Id, players);

            int maxPoint = playerWins.Values.Max();

            var winners = new List<Player>();

            winners = playerWins.Where(item => item.Value == maxPoint).Select(item => item.Key).ToList();

            var viewItemList = new List<GetWinnersGameViewItem>();

            if (winners.Count > 0 && maxPoint > 0)
            {
                int reward = game.Reward / winners.Count;

                foreach (Player player in winners)
                {
                    player.Coins += reward;

                    var viewItem = new GetWinnersGameViewItem();

                    viewItem.NickName = player.NickName;
                    viewItem.Cash = reward;

                    viewItemList.Add(viewItem);
                }
            }

            await _playerRepository.UpdateMultiple(winners);

            return viewItemList;
        }

        #endregion
    }
}