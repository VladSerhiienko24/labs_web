using AutoMapper;
using BlackJack.BusinessLogic.Interfaces;
using BlackJack.DataAccess.Interfaces;
using BlackJack.Entities.Entities;
using BlackJack.ViewModels.PlayerViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Services
{
    public class PlayerService : IPlayerService
    {
        #region Properties

        private IPlayerRepository _playerRepository;
        private IMapper _mapper;

        #endregion

        #region Constructors

        public PlayerService(IPlayerRepository playerRepository, IMapper mapper)
        {
            _playerRepository = playerRepository;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        public async Task<GetAllPlayersPlayerView> GetAllPlayers()
        {
            List<Player> players = await _playerRepository.GetAllPlayers();

            GetAllPlayersPlayerView responseView = _mapper.Map<List<Player>, GetAllPlayersPlayerView>(players);

            return responseView;
        }

        #endregion
    }
}