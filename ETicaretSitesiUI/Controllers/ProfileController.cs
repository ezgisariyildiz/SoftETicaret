using ETicaretData.Context;
using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    public class ProfileController : Controller
    {

        private readonly ETicaretContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(ETicaretContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Profil görüntüleme
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name; // Giriş yapan kullanıcının email adresini al
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userId);
            if (user == null)
            {
                return RedirectToAction("Edit", "Profile");
            }

            return View(user);
        }

        // Profil düzenleme formu
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Kullanıcının bilgilerini güncelle
            user.UserName = model.UserName;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;


            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                return RedirectToAction("Edit");
            }

            TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu.";
            return View(model);
        }



    }
}

