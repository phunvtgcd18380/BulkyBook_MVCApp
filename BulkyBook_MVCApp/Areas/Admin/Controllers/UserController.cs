﻿using BulkyBook_MVCApp.DataAccess.Data;
using BulkyBook_MVCApp.DataAccess.Repository.IRepository;
using BulkyBook_MVCApp.Models;
using BulkyBook_MVCApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook_MVCApp.Areas.Admin.Controllers
{
    [Area ("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _db.ApplicationUsers.Include(o => o.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roleList = _db.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = _db.UserRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roleList.FirstOrDefault(o => o.Id == roleId).Name;
                if(user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(o => o.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Success!" });
        }
        #endregion
    }
}