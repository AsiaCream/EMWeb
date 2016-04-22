using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using EMWeb.Models;
using EMWeb.ViewModels;
using Microsoft.Data.Entity;

namespace EMWeb.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles =("学生"))]
        public IActionResult Subject()
        {
            var student = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            if (student == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                var sub = DB.Subjects
                    .Where(x => x.StudentId == student.Id)
                    .OrderByDescending(x => x.PostTime)
                    .FirstOrDefault();
                var subjects = DB.Subjects
                    .Include(x=>x.Teacher)
                    .Include(x=>x.Student)
                    .Where(x => x.TeacherId == sub.TeacherId)
                    .ToList();
                return View(subjects);
            }
        }
        [HttpGet]
        [Authorize(Roles =("学生"))]
        public IActionResult MySubjectLog()
        {
            var logs = DB.Logs
                .Where(x => x.Roles == Roles.学生)
                .Where(x => x.UserId == User.Current.Id)
                .ToList();
            var student = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var subject = DB.Subjects
                .Where(x => x.StudentId == student.Id)
                .ToList();
            if (logs.Count != 0&&subject.Count()!=0)
            {
                var ret = new List<MySubjectLog>();
                foreach (var x in logs)
                {
                    ret.Add(new MySubjectLog
                    {
                        Id = x.Id,
                        SNumber = x.Number,
                        STitle = DB.Subjects.Where(y => y.Id == x.Number).SingleOrDefault().Title,
                        Teacher = DB.Teachers.Where(y => y.Id == (DB.Subjects.Where(z => z.Id == x.Number).SingleOrDefault().TeacherId)).SingleOrDefault().Name,
                        Time = x.Time,
                    });
                };
                return PagedView(logs, 20);
            }
            else
            {
                return RedirectToAction("Error", "System");
            }
            
        }
        [Authorize(Roles =("学生"))]
        [HttpGet]
        public IActionResult Center()
        {
            var student = DB.Students
                .Include(x=>x.College)
                .Include(x=>x.Major)
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            if (student != null)
            {
                var subject = DB.Subjects
                    .Where(x => x.StudentId == student.Id)
                    .ToList();
                if (subject.Count()==0)
                {
                    var teacher = DB.Teachers
                    .Where(x => x.MajorId == student.MajorId)
                    .OrderByDescending(x => x.CreateTime)
                    .ToList();
                    ViewBag.Teacher = teacher;
                    return View(student);
                }
                else
                {
                    var teacherid= subject.OrderBy(x => x.Id).First().TeacherId;
                    ViewBag.SubjectTeacher = DB.Teachers
                        .Where(x => x.Id == teacherid)
                        .SingleOrDefault();
                    return View(student);
                }
                
            }
            else
            {
                return RedirectToAction("Error","Home");
            }
                    }
        [HttpGet]
        public IActionResult GetMajor(string college)
        {
            var col = DB.Colleges
                .Where(x => x.Title == college)
                .SingleOrDefault();
            if (college != null)
            {
                var major = DB.Majors
                .Where(x => x.CollegeId == col.Id)
                .OrderBy(x => x.Id)
                .ToList();
                return View(major);
            }
            else
            {
                return Content("error");
            }
        }
        [Authorize(Roles =("系主任"))]
        [HttpGet]
        public IActionResult AdminGetCollege()
        {
            var college = DB.Colleges
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(college);
        }
        [HttpGet]
        public IActionResult AdminGetNextCollege()
        {
            var college = DB.Colleges
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(college);
        }
        
    }
}
