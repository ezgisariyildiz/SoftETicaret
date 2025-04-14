using ETicaretDal.Abstract;
using ETicaretData.Entities;
using ETicaretData.Helpers;
using ETicaretData.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NuGet.Protocol.Core.Types;

namespace ETicaretSitesiUI.Controllers
{
    public class CardController : Controller
    {
        private readonly IOrderDal _orderDal;
        private readonly IProductDal _productDal;

        public CardController(IOrderDal orderDal, IProductDal productDal)
        {
            _orderDal = orderDal;
            _productDal = productDal;
        }

        public IActionResult Index()
        {
            var card = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if(card == null)
            {
                return View();
            }

            ViewBag.Total = card.Sum(x => x.Product.Price * x.Quantity).ToString("c");
            SessionHelper.Count = card.Count;

            return View(card);
        }

        public IActionResult Buy(int id)
        {
            if (SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card")==null)
            {
                //yoksa sepet oluştur, ürünü getir
                var cart = new List<CardItem>();
                cart.Add(new CardItem
                {
                    Product = _productDal.Get(id),
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
            }
            else
            {
                //ürün varsa, ürünü getir
                var cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
                //hangi ürün sepette bul
                int index = isExists(cart, id);
                if (index<0)
                {
                    cart.Add(new CardItem
                    {
                        Product = _productDal.Get(id),
                        Quantity = 1
                    });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", cart);
            }
                return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CheckOut()
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if (Cart == null || Cart.Count<1)
            {
                ModelState.AddModelError("Ürün yok", "Sepette ürün bulunmamaktadır!");
                return RedirectToAction("Index");
            }
            return View(new ShippingDetails());
        }

        [HttpPost]
        public IActionResult CheckOut(ShippingDetails detail)
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            if(Cart == null)
            {
                ModelState.AddModelError("Ürün yok","Sepette ürün bulunmamaktadır!");
            }
            if (ModelState.IsValid)
            {
                SaveOrder(Cart, detail);
                Cart.Clear();
                SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
                
            }
            return View(detail);
        }

        public IActionResult Remove(int id)
        {
            var Cart = SessionHelper.GetObjectFromJson<List<CardItem>>(HttpContext.Session, "Card");
            int index = isExists(Cart, id);
            Cart.RemoveAt(index);
            
            if(Cart.Count == 0)
            {
                Cart = null;
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "Card", Cart);
            return RedirectToAction("Index");
        }

        private void SaveOrder(List<CardItem>? cart, ShippingDetails detail)
        {
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();

            string randomLetters = new string(Enumerable.Range(0, 3)
                .Select(_ => letters[random.Next(letters.Length)])
                .ToArray());

            string randomNumbers = random.NextInt64(111111111111111111, 999999999999999999).ToString();

            var orderNumber = randomLetters + randomNumbers;

            var order = new Order();
            order.OrderNumber = orderNumber;
            order.Total = cart.Sum(i => i.Product.Price * i.Quantity);
            order.OrderDate = DateTime.Now;
            order.orderState = EnumOrderState.Waiting;
            order.UserName = detail.UserName;
            order.Address = detail.Address;
            order.AddressTitle = detail.AddressTitle;
            order.City = detail.City;
            order.OrderLines = new List<OrderLine>();

            foreach (var item in cart)
            {
                var orderline = new OrderLine();
                orderline.Quantity = item.Quantity;
                orderline.Price = item.Quantity * item.Product.Price;
                orderline.ProductId = item.Product.Id;
                order.OrderLines.Add(orderline);
            }

            _orderDal.Add(order);
        }

        //card ın içindeki kaçıncı ürün, id ye eşit olan kaçıncı ürün sepette, bu ürün var mı exist mi
        private int isExists(List<CardItem> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
       
                }
                
            }
            return -1;
        }
    }
}
