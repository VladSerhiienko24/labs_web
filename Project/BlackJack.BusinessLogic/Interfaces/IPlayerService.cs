using BlackJack.ViewModels.PlayerViewModels;
using System.Threading.Tasks;

namespace BlackJack.BusinessLogic.Interfaces
{
    public interface IPlayerService
    {
        Task<GetAllPlayersPlayerView> GetAllPlayers();
    }
}