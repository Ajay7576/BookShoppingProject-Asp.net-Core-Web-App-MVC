using BookShoppingProject_DataAccess.Data;
using BookShoppingProject_Models;
using BookShoppingProject_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MainBookShoppingProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var Userlist = _context.ApplicationUsers.Include(c => c.Company).ToList(); //  AspnetUsers
            var roles = _context.Roles.ToList();                                      //    Aspnetroles
            var Userrole = _context.UserRoles.ToList();                              // AspnNetRole
            foreach (var user in Userlist)
            {
                var roleId = Userrole.FirstOrDefault(u => u.UserId == user.Id).RoleId;    //id
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;              // name
                if(user.Company==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            if(!User.IsInRole(SD.Role_Admin))                 //   admin user ni dikhaye table m check
            {
                var adminuser = Userlist.FirstOrDefault(u => u.Role == SD.Role_Admin);
                Userlist.Remove(adminuser);
            }
            return Json(new { data = Userlist });
        }
        [HttpPost]
        public IActionResult LockUnLock([FromBody] string id)
        {
            bool islocked = false;
            var userindb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (userindb == null)
                return Json(new { success = false, message = "Error while locking and Unlocking data" });

            //if (userindb.Role != SD.Role_Admin)          // employe admin ko lock na kr paaye  check
            //    return Json(new { success = false, message = "lock and unlock not allowed" });
            if(userindb!=null && userindb.LockoutEnd>DateTime.Now)
            {
                userindb.LockoutEnd = DateTime.Now;        // Unlock k liye 
                islocked = false;
            }
            else
            {
                userindb.LockoutEnd = DateTime.Now.AddYears(100);   // lock k liye
                islocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = islocked == true ? "User successfully locked" : "User Success Unlocked" });
        }
        #endregion
    }
}
