using Microsoft.AspNetCore.Mvc;

namespace SGBL.Web.Controllers
{
    public class NotificationController : Controller
    {
        public NotificationController()
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
