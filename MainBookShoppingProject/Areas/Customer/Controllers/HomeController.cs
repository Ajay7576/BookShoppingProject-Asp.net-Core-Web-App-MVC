using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using BookShoppingProject_Models.ViewModels;
using BookShoppingProject_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MainBookShoppingProject.Areas.Customer.Controllers
{
    [Area("customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;
        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitofwork = unitOfWork;
        }

        public IActionResult Index()
        {
            var ProductList = _unitofwork.Product.GetAll(IncludeProperties: "Category,CoverType");
            // 
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_Session, count);
            }
                   return View(ProductList);

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
        public IActionResult Details(int id)
        {
            var productindb = _unitofwork.Product.FirstorDefault(p => p.Id == id, IncludeProperties: "Category,CoverType");
            if (productindb == null)
                return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productindb,
                ProductId = productindb.Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details (ShoppingCart shoppingCartobj)
        {
            shoppingCartobj.Id = 0;
            if(ModelState.IsValid)
            {
                var ClaimIdentity = (ClaimsIdentity)User.Identity;         // login information
                var Claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);  // name

                shoppingCartobj.ApplicationUserId = Claim.Value; //  claim sey saari detail 

                var ShoppingCartFromDb = _unitofwork.ShoppingCart.FirstorDefault(u => u.ApplicationUserId == Claim.Value && u.ProductId == shoppingCartobj.ProductId);
                if(ShoppingCartFromDb==null)
                {
                    // add to cart
                    _unitofwork.ShoppingCart.Add(shoppingCartobj);
                }
                else
                {
                    // update to cart
                    ShoppingCartFromDb.Count += shoppingCartobj.Count;
                }

                _unitofwork.Save();
                // session 
                var count = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == Claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_Session, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var ProductIndb = _unitofwork.Product.FirstorDefault(p => p.Id == shoppingCartobj.ProductId, IncludeProperties: "Category,CoverType");
                var shoppingCart = new ShoppingCart()
                {
                    Product = ProductIndb,
                    ProductId = ProductIndb.Id
                };
                return View(shoppingCart);
            }
        }
    }
}
