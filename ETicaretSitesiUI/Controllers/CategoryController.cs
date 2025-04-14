using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryDal _categoryDal;
        private readonly ETicaretContext _contex;

        public CategoryController(ICategoryDal categoryDal, ETicaretContext contex)
        {
            _categoryDal = categoryDal;
            _contex = contex;
        }

        public async Task<IActionResult> Index()
        {
            var category = _contex.Categories.ToList();
            return View(category);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDal.Add(category);
                await _contex.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _contex.Categories == null)
            {
                return NotFound();
            }
            var category = _categoryDal.Get(Convert.ToInt32(id));
            if (category == null)
            {
                return NotFound();

            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _contex.Categories == null)
            {
                return NotFound();
            }
            // await: id gelene kadar beklesin diye kullanıldı
            var category = await _contex.Categories.FindAsync(id);
            if (category == null || _contex.Categories == null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDal.Update(category);
                _contex.Update(category);
                await _contex.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else if (id != category.Id)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(category);

        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _contex.Categories == null)
            {
                return NotFound();
            }
            // await: id gelene kadar beklesin diye kullanıldı
            var category = await _contex.Categories.FindAsync(id);
            if (category == null || _contex.Categories == null)
            {
                return RedirectToAction("Error", "Home");

            }
            return View(category);

        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (_contex.Categories == null)
            {
                return NotFound();
            }

            var category = await _contex.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryDal.Delete(category);
            _contex.Categories.Remove(category);
            await _contex.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
