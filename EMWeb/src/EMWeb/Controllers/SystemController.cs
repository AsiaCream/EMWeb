using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using EMWeb.Models;
using EMWeb.ViewModels;


namespace EMWeb.Controllers
{
    public class SystemController : BaseController
    {
        //添加专业
        [Authorize(Roles =("系主任"))]
        [HttpPost]
       public IActionResult CreateMajor(Major major,string college)
        {
            var cid = DB.Colleges
                .Where(x => x.Title == college)
                .Single();
            DB.Majors.Add(major);
            var log = new Log
            {
                UserId = User.Current.Id,
                Roles = Roles.系主任,
                Operation = Operation.添加专业,
                Time = DateTime.Now,
                Number = major.Id,
            };
            DB.Logs.Add(log);
            major.CollegeId = cid.Id;
            DB.SaveChanges();
            return Content("success");
        }
        [Authorize(Roles =("系主任"))]
        [HttpPost]
        public IActionResult CreateCollege(College college)
        {
            var oldcollege = DB.Colleges
                .Where(x => x.Title == college.Title)
                .SingleOrDefault();
            if (oldcollege != null)
            {
                return Content("error");
            }
            else
            {
                DB.Colleges.Add(college);
                var log = new Log
                {
                    Roles = Roles.系主任,
                    Operation = Operation.添加学院,
                    Time = DateTime.Now,
                    Number = college.Id,
                    UserId = User.Current.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
        [Authorize(Roles=("系主任"))]
        [HttpGet]
        public IActionResult Log()
        {
            var log = DB.Logs
                .OrderByDescending(x => x.Time)
                .ToList();
            if (log.Count() != 0)
            {
                var ret = new List<SystemLog>();
                foreach (var x in log)
                {
                    ret.Add(new SystemLog
                    {
                        Id = x.Id,
                        AdminNumber = DB.Teachers.Where(y => y.UserId == x.UserId).SingleOrDefault().Number,
                        AdminName = DB.Teachers.Where(y => y.UserId == x.UserId).SingleOrDefault().Name,
                        Role = x.Roles.ToString(),
                        Operation = x.Operation.ToString(),
                        Time = x.Time,
                        TargetNumber = x.Number,
                    });
                };

                return View(ret);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            
        }
    }
}
