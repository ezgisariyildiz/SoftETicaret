using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderDal _orderDal;
        private readonly ETicaretContext _context;

        public OrderController(IOrderDal orderDal, ETicaretContext context)
        {
            _orderDal = orderDal;
            _context = context;
        }

        public IActionResult Index()
        {
            var order = _context.Orders.ToList();
            var orderLines = _context.OrderLines
                          .Include(ol => ol.Order)   // Order ilişkisini yükle
                          .Include(ol => ol.Product)  // Product ilişkisini yükle
                          .ToList();

            return View(order);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var order = _orderDal.Get(Convert.ToInt32(id));
            if (order == null)
            {
                return NotFound();

            }
            return View(order);
        }


        public IActionResult Confirm(int id)
        {
            // Onaylama işlemine gelindiğinde sadece siparişin gösterilmesini sağlıyoruz.
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> Confirm(int id, string confirmAction)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.orderState = EnumOrderState.Completed;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }
    }
}
