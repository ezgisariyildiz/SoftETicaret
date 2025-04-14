using ETicaretData.Identity;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var user = new List<AppUser>();
            foreach (var item in admins)
            {
                user = _userManager.Users.Where(i => i.Id != item.Id).ToList();
            }
            return View(user);
        }


        [HttpGet]
        public async Task<IActionResult> RoleAssign(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            var roles = _roleManager.Roles.Where(i => i.Name != "Admin").ToList();
            var userRoles = await _userManager.GetRolesAsync(user);
            var RoleAssigns = new List<RoleAssignModels>();

            roles.ForEach(role => RoleAssigns.Add(new RoleAssignModels
            {
                HasAssign = userRoles.Contains(role.Name),
                Id = role.Id,
                Name = role.Name
            }));

            ViewBag.name = user.Name;
            return View(RoleAssigns);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignModels> models, int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var model in models)
            {
                if (model.HasAssign)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _userManager.AddToRoleAsync(user, model.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, model.Name);
                }
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            var sonuc = await _userManager.DeleteAsync(user);

            if (sonuc.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound("Silme işlemi başarısız");
            }
        }

    }
}
