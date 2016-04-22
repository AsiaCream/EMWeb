using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using EMWeb.Models;
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
