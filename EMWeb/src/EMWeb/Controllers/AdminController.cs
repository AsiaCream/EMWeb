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
    public class AdminController : BaseController
    {
        [Authorize(Roles ="学生")]
        [HttpPost]
        public IActionResult CreateSubject(Subject subject,string teacher)
        {
            var sub = DB.Subjects
                .Where(x => x.Title == subject.Title)
                .SingleOrDefault();
            var student = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            if (student == null)
            {
                return Content("error");
            }
            else
            {
                if (sub != null)
                {
                    return Content("重复添加");
                }
                else
                {
                    if (student.State == State.锁定)
                    {
                        return Content("锁定");
                    }

                    else
                    {
                        var tea = DB.Teachers
                        .Where(x => x.Name == teacher)
                        .SingleOrDefault();
                        subject.Draw = Draw.待审核;
                        subject.StudentId = student.Id;
                        subject.TeacherId = tea.Id;
                        subject.PostTime = DateTime.Now;
                        DB.Subjects.Add(subject);
                        DB.SaveChanges();
                        var log = new Log
                        {
                            Time = DateTime.Now,
                            Roles = Roles.学生,
                            Operation = Operation.添加毕业设计选题,
                            UserId = User.Current.Id,
                            Number = subject.Id,
                        };
                        DB.Logs.Add(log);
                        DB.SaveChanges();
                        return Content("success");
                    }
                }
            }
            
        }
        [AnyRoles("指导老师,系主任")]
        [HttpGet]
        public IActionResult Subject()
        {
            var teacher = DB.Teachers
                .Include(x=>x.Major)
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            if (teacher == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                var subject = DB.Subjects
                    .Include(x=>x.Teacher)
                    .Include(x=>x.Student)
                .OrderBy(x => x.Id)
                .ToList();
                var ret = new List<MajorStudent>();
                foreach (var x in subject)
                {
                    ret.Add(new MajorStudent
                    {
                        Id = x.Id,
                        StudentName = x.Student.Name,
                        TeacherName = x.Teacher.Name,
                        SubjectTitle = x.Title,
                        DrawTime = x.DrawTime,
                        PostTime = x.PostTime,
                        Draw = x.Draw.ToString(),
                        Major = x.Teacher.Major.Title,
                    });
                }

                
                return View(ret);
            }
            
        }
        [AnyRoles("系主任,指导老师")]
        [HttpGet]
        public IActionResult Manage()
        {
            ViewBag.College = DB.Colleges
                .OrderBy(x => x.Id)
                .ToList();
            var teacher = DB.Teachers
                .Include(x => x.Major)
                .Include(x => x.College)
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            return View(teacher);
        }
        [HttpPost]
        [AnyRoles("指导老师,系主任")]
        public IActionResult Pass(int id)
        {
            var subject = DB.Subjects
                .Include(x=>x.Student)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (subject == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                var student = DB.Students
                    .Where(x => x.Id == subject.StudentId)
                    .SingleOrDefault();
                student.State = State.锁定;
                subject.Draw = Draw.通过;
                subject.DrawTime = DateTime.Now;
                DB.SaveChanges();
                var ordersub = DB.Subjects
                    .Where(x => x.StudentId == student.Id)
                    .Where(x => x.Id != subject.Id)
                    .Where(x=>x.Draw==Draw.待审核)
                    .ToList();
                foreach(var x in ordersub)
                {
                    x.Draw = Draw.未通过;
                    x.DrawTime = DateTime.Now;
                    DB.Logs.Add(new Log
                    {
                        Roles = Roles.老师,
                        Operation = Operation.审核题目未通过,
                        Time = DateTime.Now,
                        UserId = User.Current.Id,
                        Number = x.Id,
                    });
                }
                var log = new Log
                {
                    UserId = User.Current.Id,
                    Roles = Roles.老师,
                    Number = subject.Id,
                    Operation = Operation.审核题目通过,
                    Time = DateTime.Now,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
        [HttpPost]
        [AnyRoles("指导老师,系主任")]
        public IActionResult Failure(int id)
        {
            var subject = DB.Subjects
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (subject == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                subject.Draw = Draw.未通过;
                subject.DrawTime = DateTime.Now;
                var log = new Log
                {
                    Roles = Roles.老师,
                    Operation = Operation.审核题目未通过,
                    Time = DateTime.Now,
                    Number = subject.Id,
                    UserId = User.Current.Id,
                };
                DB.Logs.Add(log);
                DB.SaveChanges();
                return Content("success");
            }
        }
    }
}
