using BlackJack.BusinessLogic.Interfaces;
using BlackJack.ViewModels.PlayerViewModels;
using System.Threading.Tasks;
using System.Web.Http;

namespace BlackJack.Web.Controllers.api
{
    [RoutePrefix("api/PlayerAPI")]
    public class PlayerAPIController : ApiController
    {
        private IPlayerService _playerService;

        public PlayerAPIController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        [Route("GetAllPlayers")]
        public async Task<GetAllPlayersPlayerView> GetAllPlayers()
        {
            GetAllPlayersPlayerView getAllPlayersPlayerView = await _playerService.GetAllPlayers();

            return getAllPlayersPlayerView;
        }
    }
}