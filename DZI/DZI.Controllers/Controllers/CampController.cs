using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using CampRating.Data.Models;
using CampRating.Data.ViewModels;
using CampRating.Repostories.Interfaces;
using CampRating.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Controllers.Controllers
{
    [Authorize]
    public class CampController : Controller
    {
        private readonly ICampRepository _campRepository;
        private readonly IPhotoService _photoService;

        public CampController(ICampRepository campRepository, IPhotoService photoService)
        {
            _campRepository = campRepository;
            _photoService = photoService;
        }
        public async Task<IActionResult> Index(string searchQuery)
        {
            var camps = await _campRepository.GetAllAsync();

            // Filter camps based on search query if provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                camps = camps.Where(c => c.Name.ToUpper().Contains(searchQuery.ToUpper()) || c.Description.ToUpper().Contains(searchQuery.ToUpper())).ToList(); //Make sure to use ToUpper() for case-insensitive search
            }
            ViewData["SearchQuery"] = searchQuery;
            return View(camps);
        }

        public async Task<IActionResult> Details(int id)
        {
            var camp = await _campRepository.GetByIdAsync(id);
            return View(camp);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCampVM campVM)
        {
            if (!ModelState.IsValid)
            {
                return View(campVM);
            }
            var photoResult = await _photoService.AddPhotoAsync(campVM.Image);
            if (photoResult.Error != null)
            {
                ModelState.AddModelError("Image", "Photo upload failed");
                return View(campVM);
            }
            Camp camp = new Camp
            {
                Name = campVM.Name,
                Description = campVM.Description,
                Latitude = campVM.Latitude,
                Longitude = campVM.Longitude,
                Image = photoResult.Url.ToString()
            };

            await _campRepository.AddAsync(camp);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            Camp camp = await _campRepository.GetByIdAsync(id);
            if (camp == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            EditCampVM editCampVM = new EditCampVM
            {
                Id = camp.Id,
                Name = camp.Name,
                Description = camp.Description,
                Latitude = camp.Latitude,
                Longitude = camp.Longitude,
                URL = camp.Image
            };
            return View(editCampVM);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCampVM editCampVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit camp.");
                return View(editCampVM);
            }
            Camp camp = await _campRepository.GetByIdAsync(editCampVM.Id);
            if (camp == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            camp.Name = editCampVM.Name;
            camp.Description = editCampVM.Description;
            camp.Latitude = editCampVM.Latitude;
            camp.Longitude = editCampVM.Longitude;

            // Only process image if a new one was uploaded
            if (editCampVM.Image != null && editCampVM.Image.Length > 0)
            {
                try
                {
                    // Delete old photo only if it exists and we're replacing it
                    if (!string.IsNullOrEmpty(camp.Image))
                    {
                        await _photoService.DeletePhotoAsync(camp.Image);
                    }

                    var photoResult = await _photoService.AddPhotoAsync(editCampVM.Image);
                    camp.Image = photoResult.Url.ToString();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating photo: " + ex.Message);
                    return View(editCampVM);
                }
            }

            // Update the barber regardless of whether image was changed
            await _campRepository.UpdateAsync(camp);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            Camp camp = await _campRepository.GetByIdAsync(id);
            if (camp == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            return View(camp);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("DeleteCamp")]
        public async Task<IActionResult> DeleteCamp(int id)
        {
            Camp camp = await _campRepository.GetByIdAsync(id);
            if (camp == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            await _campRepository.DeleteAsync(camp);
            return RedirectToAction("Index");
        }
    }
}
