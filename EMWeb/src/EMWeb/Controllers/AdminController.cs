﻿using System;
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
        [HttpGet]
        public IActionResult CreateSubject()
        {
            return View();
        }
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
                if (student.State == State.锁定)
                {
                    return Content("锁定");
                }
                else
                {
                    if (sub != null)
                    {
                        return Content("重复添加");
                    }
                    else
                    {
                        var tea = DB.Teachers
                        .Where(x => x.Name == teacher)
                        .SingleOrDefault();
                        DB.Subjects.Add(subject);
                        subject.PostTime = DateTime.Now;
                        subject.Draw = Draw.待审核;
                        subject.StudentId = student.Id;
                        subject.TeacherId = tea.Id;
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
        [HttpPost]
        [AnyRoles("指导老师,系主任")]
        public IActionResult Pass(int id)
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
                var student = DB.Students
                    .Where(x => x.Id == subject.StudentId)
                    .SingleOrDefault();
                student.State = State.锁定;
                subject.Draw = Draw.通过;
                subject.DrawTime = DateTime.Now;
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
                DB.SaveChanges();
                return Content("success");
            }
        }
    }
}
