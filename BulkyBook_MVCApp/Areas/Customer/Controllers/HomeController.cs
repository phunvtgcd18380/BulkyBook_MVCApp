using BulkyBook_MVCApp.DataAccess.Data;
using BulkyBook_MVCApp.DataAccess.Repository.IRepository;
using BulkyBook_MVCApp.Models;
using BulkyBook_MVCApp.Models.ViewModels;
using BulkyBook_MVCApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook_MVCApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork,ApplicationDbContext db)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            //IEnumerable<Product> productList = _db.Products.Include(i => i.Category).Include(n => n.CoverType).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFormDb = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart
            {
                Product = productFormDb,
                ProductId = productFormDb.Id
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObj)
        {
            CartObj.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObj.ApplicationUserId && u.ProductId == CartObj.ProductId
                    ,includeProperties: "Product"
                    );
                if(cartFromDb == null)
                {
                    //no records  exsit
                    _unitOfWork.ShoppingCart.Add(CartObj);
                }
                else
                {
                    cartFromDb.Count += CartObj.Count;
                }

                _unitOfWork.Save();
                
                var count = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == CartObj.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFormDb = _unitOfWork.Product.
               GetFirstOrDefault(u => u.Id == CartObj.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart
                {
                    Product = productFormDb,
                    ProductId = productFormDb.Id
                };
                return View(cartObj);
            }           

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
