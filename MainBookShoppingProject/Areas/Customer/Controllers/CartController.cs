using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using BookShoppingProject_Models.ViewModels;
using BookShoppingProject_Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MainBookShoppingProject.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitofwork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM  shoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;                   // login kiya 
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);     // claim.value userId 
            // login nhi h
            if(claim==null)
            {
                shoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(shoppingCartVM);
            }
            else
            {
                var count = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_Session, count);
            }

            // Login h
            shoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, IncludeProperties: "Product")
            };
            shoppingCartVM.OrderHeader.OrderTotal = 0;
           shoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstorDefault(u => u.Id == claim.Value, IncludeProperties: "Company");

            // Price set shopping cart
            foreach (var list in shoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                shoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);

                 // Description 
                 if(list.Product.Discription.Length>100)
                {
                    list.Product.Discription = list.Product.Discription.Substring(0, 99) + "....";
                }
            } 
                                    // Email confirmation
            if(isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has Been Sent Kindly Verify your Email ";
                ViewBag.EmailCss = "text-success";
                isEmailConfirm = false;

            }
            else
            {
                ViewBag.EmailMessage = "Email Must be Confirm for authorize customer";
                ViewBag.EmailCss = "text-danger";
            }
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var ClaimIdentity = (ClaimsIdentity)(User.Identity);
            var Claims = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitofwork.ApplicationUser.FirstorDefault(u => u.Id == Claims.Value) ;
            if(user==null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code},
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                ModelState.AddModelError("", "Verification email send,Kindly confirm your email");
                isEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Plus(int CartId)
        {
            var cart = _unitofwork.ShoppingCart.FirstorDefault(sc => sc.Id == CartId, IncludeProperties: "Product");
            cart.Count += 1;
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int CartId)
        {
            var cart = _unitofwork.ShoppingCart.FirstorDefault(sc => sc.Id == CartId, IncludeProperties: "Product");

            cart.Count -= 1;
            if (cart.Count == 0)
                _unitofwork.ShoppingCart.Remove(cart);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int CartId)
        {
            var cart = _unitofwork.ShoppingCart.FirstorDefault(sc => sc.Id == CartId, IncludeProperties: "Product");
            _unitofwork.ShoppingCart.Remove(cart);

            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var ClaimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofwork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, IncludeProperties: "Product")

            };
            shoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstorDefault(u => u.Id == claim.Value, IncludeProperties: "Company");

            foreach (var list in shoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                list.Product.Discription = SD.ConvertToRawHtml(list.Product.Discription);
            }                                                                                              // appliactionuser ki details orderheader m by default
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.City= shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.PostalCode= shoppingCartVM.OrderHeader.ApplicationUser.Postalcode;
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string StripeToken)
        {
            var claimIdentity = (ClaimsIdentity)(User.Identity);      // login
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier); // login information id

            shoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.FirstorDefault(u => u.Id == claim.Value, IncludeProperties: "Company"); // company user
            shoppingCartVM.ListCart = _unitofwork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, IncludeProperties: "Product"); // product details

            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            _unitofwork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitofwork.Save();

            foreach (var list in shoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100); // Price set 

                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count
                };
                shoppingCartVM.OrderHeader.OrderTotal += (orderDetails.Price * orderDetails.Count);
                _unitofwork.OrderDetails.Add(orderDetails);
                _unitofwork.Save();
            }

            _unitofwork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);
            _unitofwork.Save();

            HttpContext.Session.SetInt32(SD.Ss_Session, 0);
            #region StripeSettings
            if(StripeToken==null)
            {
                shoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelay;
                shoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                // Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(shoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "inr",
                    Description = "order Id:" + shoppingCartVM.OrderHeader.Id,
                    Source = StripeToken
                };
                //payment 
                var service = new ChargeService();

                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    shoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;

                if(charge.Status.ToLower()=="succeeded")
                {
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    shoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    shoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }

            }
            _unitofwork.Save();
            #endregion
            return RedirectToAction("OrderConfirmation", "Cart", new { id = shoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
