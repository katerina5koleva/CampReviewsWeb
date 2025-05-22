using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;
using CampRating.Data.ViewModels;
using CampRating.Repostories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampRating.Controllers.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICampRepository _campRepository;

        public RatingController(IRatingRepository ratingRepository, IHttpContextAccessor httpContextAccessor, ICampRepository campRepository)
        {
            _ratingRepository = ratingRepository;
            _httpContextAccessor = httpContextAccessor;
            _campRepository = campRepository;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var ratings = await _ratingRepository.GetAllAsync();
            return View(ratings);
        }

        [Authorize(Roles = "BasicUser")]
        public async Task<IActionResult> MyRatings()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRatings = await _ratingRepository.GetAllByUserAsync(currentUserId);
            return View(userRatings);
        }

        public IActionResult Create(int? campId)
        {
            if (!campId.HasValue)
            {
                return BadRequest("Camp ID is required.");
            }

            // Get current user ID from claims
            var currentUserId = _httpContextAccessor.HttpContext
                                                     .User
                                                     .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            return View(new CreateRatingVM
            {
                CampID = campId.Value,
                UserID = currentUserId
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRatingVM ratingVM)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ratingVM.UserID = currentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }
            var camp = await _campRepository.GetByIdAsync(ratingVM.CampID);
            if (camp == null)
            {
                ModelState.AddModelError("", "The specified barber doesn't exist");
                return View(ratingVM);
            }

            if (!ModelState.IsValid)
            {
                // Re-populate UserID if model is invalid
                ratingVM.UserID = currentUserId;
                return View(ratingVM);
            }

            Rating rating = new Rating
            {
                Review = ratingVM.Review,
                CampRating = ratingVM.CampRating,
                DateOfRequest = DateTime.Now,
                UserID = ratingVM.UserID,
                CampID = ratingVM.CampID
            };

            await _ratingRepository.AddAsync(rating);

            return RedirectToAction(User.IsInRole("Administrator") ? "Index" : "MyRatings");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = _httpContextAccessor.HttpContext
                                                     .User
                                                     .FindFirstValue(ClaimTypes.NameIdentifier);
            Rating rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null || rating.UserID != currentUserId && !User.IsInRole("Administrator"))
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            EditRatingVM editRatingVM = new EditRatingVM
            {
                ID = rating.Id,
                Review = rating.Review,
                CampRating = rating.CampRating,
                UserID = currentUserId,
                CampID = rating.CampID
            };
            return View(editRatingVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRatingVM editRatingVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit rating.");
                return View(editRatingVM);
            }
            Rating rating = await _ratingRepository.GetByIdAsync(editRatingVM.ID);

            rating.Id = editRatingVM.ID;
            rating.Review = editRatingVM.Review;
            rating.CampRating = editRatingVM.CampRating;
            rating.CampID = editRatingVM.CampID;
            rating.UserID = editRatingVM.UserID;

            await _ratingRepository.UpdateAsync(rating);
            return RedirectToAction(User.IsInRole("Administrator") ? "Index" : "MyRatings");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = _httpContextAccessor.HttpContext
                                                     .User
                                                     .FindFirstValue(ClaimTypes.NameIdentifier);
            Rating rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null || rating.UserID != currentUserId && !User.IsInRole("Administrator"))
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            return View(rating);
        }

        [HttpPost, ActionName("DeleteRating")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var currentUserId = _httpContextAccessor.HttpContext
                                                     .User
                                                     .FindFirstValue(ClaimTypes.NameIdentifier);
            Rating rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null || rating.UserID != currentUserId && !User.IsInRole("Administrator"))
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            await _ratingRepository.DeleteAsync(rating);
            return RedirectToAction(User.IsInRole("Administrator") ? "Index" : "MyRatings");
        }
    }
}
