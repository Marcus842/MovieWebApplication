using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MovieWebApplication.Models;
using MovieWebApplication.Services;

namespace MovieWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public OmdbResponseModel responseModel { get; set; }
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
                responseModel = await _movieService.GetAndDeserializeAsync<OmdbResponseModel>($"&s={title}&page={pageindex}");

                var homeViewModel = new HomeViewModel
                {
                    ResponseModel = responseModel,
                    PageIndex = pageindex,
                    MovieTitle = title
                };
                return View("Index", homeViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to serach for title. Error message: {Message} Stack trace: {StackTrace}", ex.Message, ex.StackTrace);
                return View("Index");
            }
        }
    }
}
