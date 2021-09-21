using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Website.Models;
using Website.ViewModels;

namespace Website.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly MobileContext _db;
        private readonly UserManager<User> _userManager;

        public BasketController(MobileContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var basketItems = _db.BasketItems.Where(b => b.Basket.UserId == user.Id).Include(p => p.Phone).ToList();
            Dictionary<Phone, int> dictionary = new Dictionary<Phone, int>();

            foreach (var item in basketItems)
            {
                dictionary.Add(item.Phone, item.Count);
            }
            BasketViewModel viewModel = new BasketViewModel()
            {
                Dictionary = dictionary
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(string returnUrl, int phoneId)
        {
            var user = await _userManager.GetUserAsync(User);
            var phone = _db.Phones.SingleOrDefault(p => p.Id == phoneId);
            var basketsDb = _db.Baskets.Any(u => u.UserId == user.Id);
            var basketItemDb = _db.BasketItems.Any(item => item.Basket.UserId == user.Id && item.Phone.Id == phoneId);
            BasketItem item;
            
            if (!basketsDb)
            {
                Basket basket = new Basket { UserId = user.Id };
                item = new BasketItem { Phone = phone, Count = 1, Basket = basket};
                _db.BasketItems.Add(item);
                _db.Baskets.Add(basket);
                await _db.SaveChangesAsync();
            }
            else if (!basketItemDb)
            {
                item = new BasketItem { Phone = phone, Count = 1, Basket = _db.Baskets.Single(u => u.UserId == user.Id)};
                _db.BasketItems.Add(item);
                await _db.SaveChangesAsync();
            }
            else
            {
                _db.BasketItems.Single(basketItem => basketItem.Basket.UserId == user.Id && basketItem.Phone.Id == phoneId).Count++;
                await _db.SaveChangesAsync();
            }
            
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> RemoveItem(string returnUrl, int phoneId)
        {
            var user = await _userManager.GetUserAsync(User);

            var basketItem = _db.BasketItems.Single(item => item.Basket.UserId == user.Id && item.Phone.Id == phoneId);

            if (basketItem.Count == 1)
            {
                _db.Remove(basketItem);
            }
            else
            {
                _db.BasketItems.Single(u => u.Basket.UserId == user.Id && u.Phone.Id == phoneId).Count--;
            }
            
            await _db.SaveChangesAsync();

            return Redirect(returnUrl);
        }
    }
}