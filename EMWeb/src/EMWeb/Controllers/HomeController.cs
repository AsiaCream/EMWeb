using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using System.IO;
using Microsoft.AspNet.Http;
using EMWeb.Models;
using EMWeb.ViewModels;

namespace EMWeb.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            ViewBag.GraduateCount = DB.Students
                .Where(x => x.IsGraduate == IsGraduate.是)
                .Count();
            return View();
        }
        [HttpGet]
        public IActionResult LogError()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Error()
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
                var subject = DB.Subjects
                    .Where(x => x.StudentId == student.Id)
                    .OrderByDescending(x => x.PostTime)
                    .FirstOrDefault();
                if (subject == null)
                {
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    var ret = DB.Subjects
                    .Include(x => x.Teacher)
                    .Include(x => x.Student)
                    .Where(x => x.TeacherId == subject.TeacherId)
                    .ToList();
                    return View(ret);
                }
            }
        }
        [HttpGet]
        [Authorize(Roles =("学生"))]
        public IActionResult MySubjectLog()
        {
            var student = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();

            var logs = DB.Logs
                .Where(x => x.Roles == Roles.学生&&x.Operation==Operation.添加毕业设计选题)
                .Where(x => x.UserId == User.Current.Id)
                .ToList();

            var subject = DB.Subjects
                .Where(x => x.StudentId == student.Id)
                .ToList();

            if (logs.Count() != 0&&subject.Count()!=0)
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
                        Time = DB.Subjects.Where(y => y.Id == x.Number).SingleOrDefault().PostTime,
                        DrawTime = DB.Subjects.Where(y => y.Id == x.Number).SingleOrDefault().DrawTime,
                        Draw=DB.Subjects.Where(y=>y.Id==x.Number).SingleOrDefault().Draw.ToString(),
                    });
                };
                return PagedView(ret, 20);
            }
            else
            {
                return RedirectToAction("LogError", "Home");
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
                    ViewBag.Announcement = DB.Announcements.OrderByDescending(x => x.Id).First().CreateTime;
                    ViewBag.Avatar = User.Current.AvatarId;
                    return View(student);
                }
                else
                {
                    var teacherid= subject.OrderBy(x => x.Id).First().TeacherId;
                    ViewBag.SubjectTeacher = DB.Teachers
                        .Where(x => x.Id == teacherid)
                        .SingleOrDefault();
                    ViewBag.Announcement = DB.Announcements.OrderByDescending(x => x.Id).First().CreateTime;
                    ViewBag.Avatar = User.Current.AvatarId;
                    return View(student);
                }
                
            }
            else
            {
                return RedirectToAction("Error","Home");
            }
                    }
        [HttpGet]
        [Authorize(Roles =("学生"))]
        public IActionResult Report()
        {
            var stud = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var rep = DB.FinleInfos
                .Where(x => x.StudentId == stud.Id)
                .Where(x=>x.FType==FType.报告)
                .ToList();
                return View(rep);
        }
        [Authorize(Roles =("学生"))]
        [HttpPost]
        public IActionResult CreateReport(long id,string filename,IFormFile file)
        {
            var user = DB.Users
                .Where(x => x.Id == id)
                .SingleOrDefault();
            file.SaveAs(".\\wwwroot\\uploads\\" + user.UserName+"\\report\\"+filename+".docx");
            var fileinfo = new Models.FileInfo
            {
                Title=filename,
                CreateTime=DateTime.Now,
                Path= user.UserName + "\\report\\" + filename + ".docx",
                FType=FType.报告,
                StudentId = DB.Students.Where(x => x.UserId == user.Id).SingleOrDefault().Id,
               
            };
            DB.FinleInfos.Add(fileinfo);
            DB.SaveChanges();
            var log = DB.Logs.Add(new Log
            {
                Roles=Roles.学生,
                Operation=Operation.上传文件,
                Time=DateTime.Now,
                Number=fileinfo.Id,
                UserId=User.Current.Id,
            });
            DB.SaveChanges();
            return RedirectToAction("Report","Home");
        }
        [HttpGet]
        [Authorize(Roles = ("学生"))]
        public IActionResult SourceCode()
        {
            var stud = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var ret = DB.FinleInfos
                .Where(x => x.StudentId == stud.Id)
                .Where(x => x.FType == FType.源代码)
                .ToList();
            return View(ret);
        }
        [HttpPost]
        [Authorize(Roles =("学生"))]
        public IActionResult CreateSourceCode(long id,string filename,IFormFile file)
        {
            var user = DB.Users
                .Where(x => x.Id == id)
                .SingleOrDefault();
            file.SaveAs(".\\wwwroot\\uploads\\" + user.UserName + "\\sourcecode\\" + filename + ".zip");
            var fileinfo = new Models.FileInfo
            {
                Title = filename,
                CreateTime = DateTime.Now,
                Path = user.UserName + "\\sourcecode\\" + filename + ".zip",
                FType = FType.源代码,
                StudentId = DB.Students.Where(x => x.UserId == user.Id).SingleOrDefault().Id,

            };
            DB.FinleInfos.Add(fileinfo);
            DB.SaveChanges();
            var log = DB.Logs.Add(new Log
            {
                Roles = Roles.学生,
                Operation = Operation.上传文件,
                Time = DateTime.Now,
                Number = fileinfo.Id,
                UserId = User.Current.Id,
            });
            DB.SaveChanges();
            return RedirectToAction("SourceCode", "Home");
        }
        [HttpGet]
        [Authorize(Roles = ("学生"))]
        public IActionResult Thesis()
        {
            var stud = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var ret = DB.FinleInfos
                .Where(x => x.StudentId == stud.Id)
                .Where(x => x.FType == FType.论文)
                .ToList();
            return View(ret);
        }
        [HttpPost]
        [Authorize(Roles = ("学生"))]
        public IActionResult CreateThesis(long id,string filename,IFormFile file)
        {
            var user = DB.Users
                .Where(x => x.Id == id)
                .SingleOrDefault();
            file.SaveAs(".\\wwwroot\\uploads\\" + user.UserName + "\\thesis\\" + filename + ".docx");
            var fileinfo = new Models.FileInfo
            {
                Title = filename,
                CreateTime = DateTime.Now,
                Path = user.UserName + "\\thesis\\" + filename + ".docx",
                FType = FType.论文,
                StudentId = DB.Students.Where(x => x.UserId == user.Id).SingleOrDefault().Id,

            };
            DB.FinleInfos.Add(fileinfo);
            DB.SaveChanges();
            var log = DB.Logs.Add(new Log
            {
                Roles = Roles.学生,
                Operation = Operation.上传文件,
                Time = DateTime.Now,
                Number = fileinfo.Id,
                UserId = User.Current.Id,
            });
            DB.SaveChanges();
            return RedirectToAction("Thesis", "Home");
        }
        [Authorize(Roles = ("学生"))]
        public IActionResult Document()
        {
            var stud = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var ret = DB.FinleInfos
                .Where(x => x.StudentId == stud.Id)
                .Where(x => x.FType == FType.文档)
                .ToList();
            return View(ret);
        }
        [HttpPost]
        [Authorize(Roles = ("学生"))]
        public IActionResult CreateDocument(long id, string filename, IFormFile file)
        {
            var user = DB.Users
                .Where(x => x.Id == id)
                .SingleOrDefault();
            file.SaveAs(".\\wwwroot\\uploads\\" + user.UserName + "\\document\\" + filename + ".docx");
            var fileinfo = new Models.FileInfo
            {
                Title = filename,
                CreateTime = DateTime.Now,
                Path = user.UserName + "\\document\\" + filename + ".docx",
                FType = FType.文档,
                StudentId = DB.Students.Where(x => x.UserId == user.Id).SingleOrDefault().Id,

            };
            DB.FinleInfos.Add(fileinfo);
            DB.SaveChanges();
            var log = DB.Logs.Add(new Log
            {
                Roles = Roles.学生,
                Operation = Operation.上传文件,
                Time = DateTime.Now,
                Number = fileinfo.Id,
                UserId = User.Current.Id,
            });
            DB.SaveChanges();
            return RedirectToAction("Document", "Home");
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
        [HttpGet]
        public IActionResult EditGetCollege(int id)
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
        [HttpGet]
        public IActionResult EditGetMajor(int id)
        {
            var major = DB.Majors
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (major == null)
            {
                return RedirectToAction("Error", "Homw");
            }
            else
            {
                return View(major);
            }
        }
        [HttpGet]
        public IActionResult Announcement()
        {
            var student = DB.Students
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            if (student != null)
            {
                var ret = DB.Announcements
                    .Where(x=>x.MajorId==student.MajorId&&x.CreateTime.Year==DateTime.Now.Year)
                .OrderByDescending(x => x.CreateTime)
                .ToList();
                return View(ret);
            }
            else
            {
                var teacher = DB.Teachers
                    .Where(x => x.UserId == User.Current.Id)
                    .SingleOrDefault();
                var ret = DB.Announcements
                    .Where(x => x.MajorId == teacher.MajorId && x.CreateTime.Year == DateTime.Now.Year)
                .OrderByDescending(x => x.CreateTime)
                .ToList();
                return View(ret);
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> EditAvatar(long id,IFormFile avatar,User Model)
        {
            var user = DB.Users
                .Include(x=>x.Avatar)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (avatar != null)
            {
                try
                {
                    DB.Files.Remove(DB.Files.Single(x => x.Id == user.AvatarId));
                }
                catch { }
                var file = new CodeComb.AspNet.Upload.Models.File
                {
                    Bytes = await avatar.ReadAllBytesAsync(),
                    ContentLength = avatar.Length,
                    ContentType = avatar.ContentType,
                    FileName = avatar.GetFileName(),
                    Time = DateTime.Now
                };
                DB.Files.Add(file);
                user.AvatarId = file.Id;
            }
            DB.SaveChanges();
            return RedirectToAction("Center", "Home");
        }
        [HttpGet]
        public IActionResult AnnouncementDetails(int id)
        {
            var announce = DB.Announcements
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (announce == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View(announce);
            }
        }
    }
}
