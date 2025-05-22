using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;
using CampRating.Data.ViewModels;
using CampRating.Repostories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CampRating.Controllers.Controllers
{
    [Authorize(Roles = "Administrator")]

    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(
            IUserRepository userRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            var userVMs = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userVM = new UserVM
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Ratings = user.RatingsByUser,
                    Role = roles.FirstOrDefault()
                };
                userVMs.Add(userVM);
            }

            return View(userVMs);
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Ratings = user.RatingsByUser,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            return View(vm);
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View(new CreateUserVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM vm)
        {

            if (await _userManager.FindByNameAsync(vm.UserName) != null) //Username should be unique
            {
                ModelState.AddModelError("UserName", "Username is taken");
            }
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = vm.UserName,
                    FirstName = vm.Firstname,
                    LastName = vm.LastName
                };
                var result = await _userRepository.AddAsync(user, vm.Password);
                await _userManager.AddToRoleAsync(user, "BasicUser");

                if (result)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(loginVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var vm = new EditUserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Firstname = user.FirstName,
                LastName = user.LastName
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword),
                        "Current password is required when changing password.");
                }

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    ModelState.AddModelError(nameof(model.ConfirmPassword),
                        "Please confirm your new password.");
                }
                else if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError(nameof(model.ConfirmPassword),
                        "The new password and confirmation password do not match.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.FirstName = model.Firstname;
            user.LastName = model.LastName;
            user.UserName = model.UserName;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {

                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword),
                        "The current password is incorrect.");
                    return View(model);
                }

                
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            await _userRepository.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new UserVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            return View(vm);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userRepository.DeleteAsync(user);
            if (result)
            {
                TempData["SuccessMessage"] = "User deleted successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to delete user";
            return View(await _userRepository.GetByIdAsync(id));
        }
    }
}
