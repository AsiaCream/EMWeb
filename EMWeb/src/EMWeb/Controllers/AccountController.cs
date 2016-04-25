using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
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
                var user = await UserManager.FindByNameAsync(username);

                if (await UserManager.IsInRoleAsync(user, "学生"))
                {
                    return Content("学生");
                }
                else
                {
                    return Content("老师");
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
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user, "学生");
                    var student = new Student
                    {
                        Name = name,
                        Number = number,
                        UserId = user.Id,
                        CollegeId = col.Id,
                        MajorId = maj.Id,
                        CreateTime = DateTime.Now,
                        State = State.未锁定,
                    };
                    DB.Students.Add(student);
                    DB.SaveChanges();
                    return Content("success");
                }
                else
                {
                    return Content("password");
                }
                
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Modify(string password,string newpwd)
        {
           var result= await UserManager.ChangePasswordAsync(User.Current,password,newpwd);
            if (result.Succeeded)
            {
                await SignInManager.SignOutAsync();
                return Content("success");
            }
            else
            {
                return Content("error");
            }
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
                    var teacher = new Teacher
                    {
                        Name = name,
                        Number = number,
                        UserId = user.Id,
                        CollegeId = col.Id,
                        MajorId = maj.Id,
                        CreateTime = DateTime.Now,
                    };
                    var log = new Log
                    {
                        UserId = User.Current.Id,
                        Roles = Roles.系主任,
                        Operation = Operation.添加系主任,
                        Time = DateTime.Now,
                        Number = teacher.Id,
                    };
                    DB.Logs.Add(log);
                    DB.Teachers.Add(teacher);
                    DB.SaveChanges();
                    return Content("success");
                }
                else
                {
                    await UserManager.AddToRoleAsync(user, "指导老师");
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
                    var log = new Log
                    {
                        UserId = User.Current.Id,
                        Roles = Roles.系主任,
                        Operation = Operation.添加老师,
                        Time = DateTime.Now,
                        Number = teacher.Id,
                    };
                    DB.Logs.Add(log);
                    DB.SaveChanges();
                    return Content("success");
                }
                            }
        }
    }
}
