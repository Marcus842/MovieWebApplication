using Microsoft.AspNetCore.Mvc;
using MovieWebApplication.Models;
using MovieWebApplication.Services;

namespace MovieWebApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(ErrorViewModel viewModel)
        {
            return View("Index", viewModel);
        }
    }
}
