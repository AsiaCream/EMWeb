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
    [Authorize(Roles=("系主任"))]
    public class SystemController : BaseController
    {
        [HttpPost]
       public IActionResult CreateMajor(Major major,string college)
        {
            var cid = DB.Colleges
                .Where(x => x.Title == college)
                .Single();
            var oldmajor = DB.Majors
                .Where(x => x.Title == major.Title)
                .SingleOrDefault();
            if (oldmajor != null)
            {
                return Content("error");
            }
            else
            {
                DB.Majors.Add(major);
                major.CollegeId = cid.Id;
                DB.SaveChanges();
                var log = new Log
                {
                    UserId = User.Current.Id,
                    Roles = Roles.系主任,
                    Operation = Operation.添加专业,
                    Time = DateTime.Now,
                    Number = major.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
            
        }
        [HttpGet]
        public IActionResult AllCollege()
        {
            var college = DB.Colleges
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(college);
        }
        [HttpGet]
        public IActionResult NewAllCollege()
        {
            var college = DB.Colleges
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(college);
        }
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
                DB.SaveChanges();
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
        [HttpGet]
        public IActionResult EditCollege(int id)
        {
            var college = DB.Colleges
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (college == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(college);
            }
        }
        [HttpPost]
        public IActionResult EditCollege(int id,College college)
        {
            var oldcollege = DB.Colleges
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (oldcollege == null)
            {
                return Content("error");
            }
            else
            {
                oldcollege.Title = college.Title;
                var log = new Log
                {
                    UserId = User.Current.Id,
                    Roles = Roles.系主任,
                    Operation = Operation.编辑学院,
                    Time = DateTime.Now,
                    Number = oldcollege.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
        [HttpPost]
        public IActionResult DeleteCollege(int id)
        {
            var college = DB.Colleges
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (college == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                DB.Colleges.Remove(college);
                var log = new Log
                {
                    UserId = User.Current.Id,
                    Roles = Roles.系主任,
                    Operation = Operation.删除学院,
                    Time = DateTime.Now,
                    Number = college.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
        [HttpGet]
        public IActionResult Log()
        {
            var log = DB.Logs
                .Where(x=>x.Roles==Roles.老师||x.Roles==Roles.系主任)
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

                return PagedView(ret,50);
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
            
        }
        [HttpGet]
        public IActionResult AllMajor()
        {
            var major = DB.Majors
                .Include(x=>x.College)
                .OrderByDescending(x => x.Id)
                .ToList();
            var ret = new List<CollegeMajor>();
            foreach(var x in major)
            {
                ret.Add(new CollegeMajor
                {
                    CollegeNumber = x.College.Id,
                    CollegeName = x.College.Title,
                    MajorNumber = x.Id,
                    MajorName=x.Title,
                });
            }
            return PagedView(ret,50);
        }
        [HttpGet]
        public IActionResult NewAllMajor()
        {
            var major = DB.Majors
                .Include(x => x.College)
                .OrderByDescending(x => x.Id)
                .ToList();
            var ret = new List<CollegeMajor>();
            foreach (var x in major)
            {
                ret.Add(new CollegeMajor
                {
                    CollegeNumber = x.College.Id,
                    CollegeName = x.College.Title,
                    MajorNumber = x.Id,
                    MajorName = x.Title,
                });
            }
            return View(ret);
        }
        [HttpGet]
        public IActionResult EditMajor(int id)
        {
            var major = DB.Majors
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (major == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(major);
            }
        }
        [HttpPost]
        public IActionResult EditMajor(int id,Major major)
        {
            var oldmajor = DB.Majors
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (oldmajor == null)
            {
                return RedirectToAction("Error","Home");
            }
            else
            {
                oldmajor.Title = major.Title;
                var log = new Log
                {
                    UserId = User.Current.Id,
                    Roles = Roles.系主任,
                    Operation = Operation.编辑专业,
                    Time = DateTime.Now,
                    Number = oldmajor.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
        [HttpPost]
        public IActionResult DeleteMajor(int id)
        {
            var major = DB.Majors
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (major == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                DB.Majors.Remove(major);
                var log = DB.Logs.Add(new Log
                {
                    Roles = Roles.系主任,
                    Operation = Operation.删除专业,
                    Number = major.Id,
                    Time = DateTime.Now,
                    UserId = User.Current.Id,
                });
                DB.SaveChanges();
                return Content("success");
            }
        }
        
    }
}
