using System.Diagnostics;
using CampRating.Data.Models;
using CampRating.Data.ViewModels;
using CampRating.Repostories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CampRating.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly ICampRepository _campRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository, IRatingRepository ratingRepository, ICampRepository campRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            _campRepository = campRepository;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminHomeIndex");
            }
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminHomeIndex()
        {
            AdminHomeIndexVM adminHomeIndexVM = new AdminHomeIndexVM
            {
                UserCount = await _userRepository.CountUsersAsync(),
                RatingCount = await _ratingRepository.CounRatingsAsync(),
                CampCount = await _campRepository.CountCampsAsync()
            };
            return View(adminHomeIndexVM);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
