using Microsoft.AspNetCore.Mvc;

namespace Example_POS.Controllers
{
    public class PurchaseOrderInfo : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
