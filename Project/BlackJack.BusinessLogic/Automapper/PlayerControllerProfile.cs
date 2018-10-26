using AutoMapper;
using BlackJack.Entities.Entities;
using BlackJack.ViewModels.PlayerViewModels;
using System.Collections.Generic;

namespace BlackJack.BusinessLogic.Automapper
{
    public class PlayerControllerProfile : Profile
    {
        public PlayerControllerProfile()
        {
            CreateMap<List<Player>, GetAllPlayersPlayerView>()
               .ForMember(destination => destination.Players, opt => opt.MapFrom(src => src));

            CreateMap<Player, GetAllPlayersPlayerViewItem>();
        }
    }
}