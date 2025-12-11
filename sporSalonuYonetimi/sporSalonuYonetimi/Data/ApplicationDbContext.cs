using Microsoft.AspNetCore.Mvc;

namespace sporSalonuYonetimi.Data
{
    public class ApplicationDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
