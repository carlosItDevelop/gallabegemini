using Microsoft.AspNetCore.Mvc;

namespace VelzonModerna.Controllers
{
    public class CRMGalLabController : Controller
    {

        [ActionName("CRM")]
        public IActionResult CRM()
        {
            return View();
        }


        [ActionName("Contacts")]
        public IActionResult Contacts()
        {
            return View();
        }

        [ActionName("Companies")]
        public IActionResult Companies()
        {
            return View();
        }

        [ActionName("Deals")]
        public IActionResult Deals()
        {
            return View();
        }

        [ActionName("Leads")]
        public IActionResult Leads()
        {
            return View();
        }
    }
}
