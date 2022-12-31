using BookShoppingProject_DataAccess.Data.Repository.IRepository;
using BookShoppingProject_Models;
using BookShoppingProject_Utility;
using Dapper;
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
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CoverTypeController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int?id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
                return View(coverType);
            var param = new DynamicParameters();
            param.Add("@Id", id.GetValueOrDefault());
            coverType = _unitofwork.SP_Call.OneRecord<CoverType>(SD.Pro_CoverType_GetCoverType,param);
           // var covertypeindb = _unitofwork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
                return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null)
                return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            var param = new DynamicParameters();
            param.Add("@Name", coverType.Name);
            if (coverType.Id == 0)
                _unitofwork.SP_Call.Execute(SD.Pro_CoverType_Create, param);
            //_unitofwork.CoverType.Add(coverType);
            else
            {
                param.Add("@Id", coverType.Id);
                _unitofwork.SP_Call.Execute(SD.Pro_CoverType_Update, param);
            }
                //_unitofwork.CoverType.Update(coverType);
            //_unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            // var CoverTypelist  = _unitofwork.CoverType.GetAll();
            var CoverTypelist = _unitofwork.SP_Call.List<CoverType>(SD.Pro_CoverType_GetCoverTypes);
            return Json(new { data = CoverTypelist });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var covertypeindb = _unitofwork.CoverType.Get(id);
            if (covertypeindb == null)
                return Json(new { success = false, message = "Error while Delete Data?" });
            var param = new DynamicParameters();
            param.Add("@Id", id);
            _unitofwork.SP_Call.Execute(SD.Pro_CoverType_Delete,param);
            //_unitofwork.CoverType.Remove(covertypeindb);
            //_unitofwork.Save();
            return Json(new { success = true, message = "Data Successfully Deleted!!" });
        }
        #endregion
    }
}
