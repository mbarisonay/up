using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using user_panel.Data;       // Your ApplicationUser model
using user_panel.ViewModels; // Your ViewModels
using System.Linq;
using System.Threading.Tasks;

namespace user_panel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // --- REGISTER ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreditBalance = 0 // Start with 0 credit
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- LOGIN ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = null;
                // Check if input contains only numbers (phone)
                if (model.EmailOrPhone.All(char.IsDigit))
                {
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.EmailOrPhone);
                }
                else // Otherwise, it's an email
                {
                    user = await _userManager.FindByEmailAsync(model.EmailOrPhone);
                }

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserPanel", "Account");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // --- LOGOUT ---
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // --- USER PANEL ---
        [Authorize] // Ensures only logged-in users can access this page
        [HttpGet]
        public async Task<IActionResult> UserPanel()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(user);
        }

        // --- CHANGE PASSWORD ---
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user.");

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction("UserPanel");
        }

        // --- UPDATE INFORMATION ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateInformation()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound($"Unable to load user.");
            var model = new UpdateInformationViewModel
            {
                NewEmail = user.Email,
                NewPhoneNumber = user.PhoneNumber
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateEmail(UpdateInformationViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.NewEmail != user.Email)
            {
                user.Email = model.NewEmail;
                user.UserName = model.NewEmail;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["StatusMessage"] = "Your E-mail has been updated.";
                }
            }
            return RedirectToAction("UserPanel");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePhoneNumber(UpdateInformationViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.NewPhoneNumber != user.PhoneNumber)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, model.NewPhoneNumber);
                if (result.Succeeded)
                {
                    TempData["StatusMessage"] = "Your Phone Number has been updated.";
                }
            }
            return RedirectToAction("UserPanel");
        }
    }
}