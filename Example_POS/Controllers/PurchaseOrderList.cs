using Microsoft.AspNetCore.Mvc;

namespace Example_POS.Controllers
{
    public class PurchaseOrderList : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
