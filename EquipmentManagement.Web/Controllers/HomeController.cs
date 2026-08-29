using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EquipmentManagement.Web.Models;

namespace EquipmentManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true &&
            User.IsInRole("Admin"))
        {
            return RedirectToAction(
                "Index",
                "Dashboard"
            );
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
