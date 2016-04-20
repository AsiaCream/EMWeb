using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using EMWeb.Models;

namespace EMWeb.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {
            var result = await SignInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                if (User.IsInRole("学生"))
                {
                    return Content("student");
                }
                else
                {
                    return Content("teacher");
                }
            }
            else
            {
                return Content("error");
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            var college = DB.Colleges
                .OrderByDescending(x => x.Id)
                .ToList();
            ViewBag.College = college;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string name,int number,string username,
            string password,string college,string major)
        {
            var olduser = DB.Users
                .Where(x => x.UserName == username)
                .SingleOrDefault();
            if (olduser != null)
            {
                return Content("error");
            }
            else
            {
                var col = DB.Colleges
                    .Where(x => x.Title == college)
                    .SingleOrDefault();
                var maj = DB.Majors
                    .Where(x => x.Title == major)
                    .SingleOrDefault();
                var user = new User
                {
                    UserName = username,
                };
                await UserManager.CreateAsync(user, password);
                await UserManager.AddToRoleAsync(user, "学生");
                var student = new Student
                {
                    Name = name,
                    Number = number,
                    UserId = user.Id,
                    CollegeId = col.Id,
                    MajorId = maj.Id,
                    CreateTime = DateTime.Now,
                    State=State.未锁定,
                };
                DB.Students.Add(student);
                DB.SaveChanges();
                return Content("success");
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles =("系主任"))]
        public async Task<IActionResult> CreateTeacher(string name,int number,string username,
            string password,string college,string major,string isheadTeacher)
        {
            var oldteacher = DB.Users
                .Where(x => x.UserName == username)
                .SingleOrDefault();
            if (oldteacher != null)
            {
                return Content("重复创建");
            }
            else
            {
                var col = DB.Colleges
                    .Where(x => x.Title == college)
                    .SingleOrDefault();
                var maj = DB.Majors
                    .Where(x => x.Title == major)
                    .SingleOrDefault();
                var user = new User
                {
                    UserName = username,
                };
                await UserManager.CreateAsync(user, password);
                if (isheadTeacher == "YES")
                {
                    await UserManager.AddToRoleAsync(user, "系主任");
                }
                else
                {
                    await UserManager.AddToRoleAsync(user, "指导老师");
                }
                var teacher = new Teacher
                {
                    Name = name,
                    Number = number,
                    UserId = user.Id,
                    CollegeId = col.Id,
                    MajorId = maj.Id,
                    CreateTime = DateTime.Now,
                };
                DB.Teachers.Add(teacher);
                DB.SaveChanges();
                return Content("success");
            }
        }
    }
}
