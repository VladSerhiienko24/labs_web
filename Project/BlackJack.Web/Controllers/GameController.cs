using System.Web.Mvc;

namespace BlackJack.Web.Controllers
{
    public class GameController : Controller
    {
        public ActionResult NewGame()
        {
            return View();
        }

        public ActionResult Table(int gameId)
        {
            return View(gameId);
        }
    }
}