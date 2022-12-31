using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using BookShoppingProject_Models.ViewModels;
using BookShoppingProject_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MainBookShoppingProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitofwork,IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int?id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofwork.Category.GetAll().Select(c1 => new SelectListItem()
                {
                    Text = c1.Name,
                    Value = c1.Id.ToString()
                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(ct => new SelectListItem()
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                })
            };
            if (id == null)
                return View(productVM);
            productVM.Product = _unitofwork.Product.Get(id.GetValueOrDefault());
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid)
            {
                var webrootPath = _webHostEnvironment.WebRootPath;       // Path
                var files = HttpContext.Request.Form.Files;            // file access control


                if(files.Count>0)                // file select ki h toh             
                {
                    var filename  = Guid.NewGuid().ToString();                   // duplicate name ni hoga
                    var uploads = Path.Combine(webrootPath, @"images\products");     //path set 
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productVM.Product.Id!=0)             // Edit case No change Image
                    {
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                    if(productVM.Product.ImageUrl!=null)             // Exist image (phele sey h)
                    {
                        var imagePath = Path.Combine(webrootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(imagePath))        
                        {                                              //  Delete kre edit wale case m purani image ko
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var fileStream=new FileStream(Path.Combine(uploads,filename+extension),FileMode.Create))            // files bnaaye gaaa
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + filename + extension;
                }
                else
                {
                    if(productVM.Product.Id!=0)
                    {
                        var imageExists = _unitofwork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    }
                }
                if (productVM.Product.Id == 0)
                    _unitofwork.Product.Add(productVM.Product);
                else
                    _unitofwork.Product.Update(productVM.Product);
                _unitofwork.Save();
                return RedirectToAction(nameof(Index));

            }    
            else
            {
                productVM = new ProductVM()
                {
                    CategoryList = _unitofwork.Category.GetAll().Select(c1 => new SelectListItem()
                    {
                        Text = c1.Name,
                        Value = c1.Id.ToString()
                    }),
                    CoverTypeList = _unitofwork.CoverType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    })
                };
                if(productVM.Product.Id!=0)
                {
                    productVM.Product = _unitofwork.Product.Get(productVM.Product.Id);

                }
                return View(productVM);
            }
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var Productlist = _unitofwork.Product.GetAll(IncludeProperties: "Category,CoverType");      // main table k saath 2 hor table ka data 
            return Json(new { data = Productlist });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ProductIndb = _unitofwork.Product.Get(id);
            if (ProductIndb == null)
                return Json(new { success = false, message = "Error" });
            if(ProductIndb.ImageUrl!="")
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, ProductIndb.ImageUrl.TrimStart('\\'));
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _unitofwork.Product.Remove(ProductIndb);
            _unitofwork.Save();
            return Json(new { success = true, message = "data deleted successfully!!" });
        }
        #endregion
    }
}
