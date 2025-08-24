using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
