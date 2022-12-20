using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab8.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            // Check the user's role
            if (User.IsInRole("Manager"))
            {
                // Redirect to the Manager view
                return RedirectToAction("UnapprovedListings", "Manager");
            }
            else
            {
                // Redirect to the Customer view
                return RedirectToAction("Index", "Customer");
            }
        }
    }
}
