using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using BookShoppingProject_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainBookShoppingProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CategoryController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)                                                               // Create k liye
                return View(category);

            var CategoryInDb = _unitofwork.Category.Get(id.GetValueOrDefault());           // Update k kiye
            if (CategoryInDb == null)
                return NotFound();
            return View(CategoryInDb);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
                return NotFound();
            if (!ModelState.IsValid)                  //server side validation
                return View(category);
            if (category.Id == 0)                       // Create k liye
                _unitofwork.Category.Add(category);
            else
                _unitofwork.Category.Update(category);      // update k liye
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll ()
        {
            var Categorylist = _unitofwork.Category.GetAll();
            return Json(new { data = Categorylist });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryindb = _unitofwork.Category.Get(id);
            if (categoryindb == null)
                return Json(new { success = false, message = "Error while Delete Data !!!" });
            _unitofwork.Category.Remove(id);
            _unitofwork.Save();
            return Json(new { success = true, message = "Data Successfully deleted!!!" });
        }
        #endregion
    }
}
