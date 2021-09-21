using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Website.Models;
using Website.ViewModels;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private MobileContext db;

        public HomeController(MobileContext context)
        {
            db = context;
            if (!db.Companies.Any())
            {
                Company apple = new Company { Name = "Apple", Country = "США" };
                Company samsung = new Company { Name = "Samsung", Country = "Республика Корея" };
                Company google = new Company { Name = "Google", Country = "США" };

                Phone phone1 = new Phone { Manufacturer = apple, Name = "iPhone X", Price = 56000 };
                Phone phone2 = new Phone { Manufacturer = apple, Name = "iPhone XS", Price = 65000 };
                Phone phone3 = new Phone { Manufacturer = samsung, Name = "Galaxy 9", Price = 9000 };
                Phone phone4 = new Phone { Manufacturer = samsung, Name = "Galaxy 10", Price = 40000 };
                Phone phone5 = new Phone { Manufacturer = google, Name = "Pixel 2", Price = 30000 };
                Phone phone6 = new Phone { Manufacturer = google, Name = "Pixel XL", Price = 50000 };

                db.Companies.AddRange(apple, samsung, google);
                db.Phones.AddRange(phone1, phone2, phone3, phone4, phone5, phone6);
                db.SaveChanges();
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy() => View();

        public IActionResult Product(int? company, string name, int page = 1,
            SortState sortOrder = SortState.NameAsc)
        {
            int pageSize = 3;

            IQueryable<Phone> phones = db.Phones.Include(x => x.Manufacturer);

            if (company != null && company != 0)
            {
                phones = phones.Where(p => p.Manufacturer.Id == company);
            }
            if (!String.IsNullOrEmpty(name))
            {
                phones = phones.Where(p => p.Name.Contains(name));
            }

            switch (sortOrder)
            {
                case SortState.NameDesc:
                    phones = phones.OrderByDescending(s => s.Name);
                    break;
                case SortState.CompanyAsc:
                    phones = phones.OrderBy(s => s.Manufacturer.Name);
                    break;
                case SortState.CompanyDesc:
                    phones = phones.OrderByDescending(s => s.Manufacturer.Name);
                    break;
                case SortState.PriceAsc:
                    phones = phones.OrderBy(s => s.Price);
                    break;
                case SortState.PriceDesc:
                    phones = phones.OrderByDescending(s => s.Price);
                    break;
                default:
                    phones = phones.OrderBy(s => s.Name);
                    break;
            }

            var count = phones.Count();
            var items = phones.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ProductViewModel viewModel = new ProductViewModel
            {
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterViewModel = new FilterViewModel(db.Companies.ToList(), company, name),
                Phones = items
            };
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}