using AutoMapper;
using BlackJack.Entities.Entities;
using BlackJack.ViewModels.GameViewModels;

namespace BlackJack.BusinessLogic.Automapper
{
    public class GameControllerProfile : Profile
    {
        public GameControllerProfile()
        {
            CreateMap<Card, CardGetPlayersTwoCardsGameViewItemItem>();

            CreateMap<Card, CardGetDeallerCardGameViewItemItem>();

            CreateMap<CreateNewGameGameView, Game>();

            CreateMap<Game, GameGetGameGameViewItem>();

            CreateMap<Player, PlayerGetGameGameViewItem>();

            CreateMap<RequestGetBotExtraCardGameView, RequestGetExtraCardGameView>();

            CreateMap<RequestGetDeallerExtraCardGameView, RequestGetExtraCardGameView>();

            CreateMap<ResponseGetExtraCardGameView, ResponseGetBotExtraCardGameViewItem>();

            CreateMap<ResponseGetExtraCardGameViewItem, CardGetBotExtraCardGameViewItem>();

            CreateMap<ResponseGetExtraCardGameView, ResponseGetDeallerExtraCardGameViewItem>();

            CreateMap<ResponseGetExtraCardGameViewItem, CardGetDeallerExtraCardGameViewItem>();

            CreateMap<Card, ResponseGetExtraCardGameViewItem>();
        }
    }
}
