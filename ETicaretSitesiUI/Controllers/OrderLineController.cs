using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretSitesiUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrderLineController : Controller
    {

        private readonly IOrderLineDal _orderLineDal;
        private readonly ETicaretContext _context;

        public OrderLineController(IOrderLineDal orderLineDal, ETicaretContext context)
        {
            _orderLineDal = orderLineDal;
            _context = context;
        }

        public IActionResult Index()
        {
            var orderLines = _context.OrderLines
                          .Include(ol => ol.Order)   // Order ilişkisini yükle
                          .Include(ol => ol.Product)  // Product ilişkisini yükle
                          .ToList();

            var groupedOrderLines = orderLines
                .GroupBy(ol => ol.OrderId)
                .ToList();

            return View(groupedOrderLines);  // Gruplama yapılmış veriyi View'a gönder
        }
    }
}
