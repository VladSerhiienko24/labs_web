using System.Web.Mvc;

namespace BlackJack.Web.Controllers
{
    public class PlayerController : Controller
    {
        public ActionResult AllPlayers()
        {
            return View();
        }
    }
}