using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MovieWebApplication.Models;
using MovieWebApplication.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public OmdbResponseModel _responseModel;
        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _movieService = movieService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchTitle(string title, string pageindex)
        {
            try
            {
                if (pageindex == null)
                {
                    pageindex = "1";
                }
                _responseModel = await _movieService.GetAndDeserializeAsync<OmdbResponseModel>($"&s={title}&page={pageindex}");

                if (_responseModel.Response == "False")
                {
                    _logger.LogError("Unable to serach for title. Error message: {Message}", _responseModel.Error);
                    return ShowErrorView("Error", _responseModel.Error);
                }

                var homeViewModel = new HomeViewModel
                {
                    ResponseModel = _responseModel,
                    PageIndex = pageindex,
                    MovieTitle = title
                };
                return View("Index", homeViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to serach for title. Error message: {Message} Stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return ShowErrorView("Error", ex.Message, ex.StackTrace);
            }
        }

        private IActionResult ShowErrorView(string title, string? message, string? stacktrace = null)
        {
            var viewModel = new ErrorViewModel
            {
                Title = title,
                Message = message,
                StackTrace = stacktrace
            };
            return RedirectToAction("Index", "Error", viewModel);
        }
    }
}
