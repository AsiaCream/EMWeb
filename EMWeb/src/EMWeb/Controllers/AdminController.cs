using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Http;
using EMWeb.Models;
using EMWeb.ViewModels;


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
            
                var subject = DB.Subjects
                    .Include(x=>x.Teacher)
                    .Include(x=>x.Student)
                    .Where(x=>x.Student.MajorId==teacher.MajorId&&x.Student.IsGraduate==IsGraduate.否)
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

                ViewBag.Teacher = DB.Teachers
                    .Where(x => x.UserId == teacher.UserId)
                    .SingleOrDefault();
                return PagedView(ret.OrderByDescending(x=>x.PostTime).ToList(),20);
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
            ViewBag.Avatar = User.Current.AvatarId;
            return View(teacher);
        }
        #region 指导老师和系主任审核题目方法
        [HttpPost]
        [AnyRoles("指导老师,系主任")]
        public IActionResult Pass(int id)
        {
            var subject = DB.Subjects
                .Include(x => x.Student)
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
                    .Where(x => x.Draw == Draw.待审核)
                    .ToList();
                foreach (var x in ordersub)
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
        #endregion
        /// <summary>
        /// 系主任查看所有学生，指导老师查看已经选择了他的学生
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AnyRoles("指导老师,系主任")]
        public IActionResult StudentList()
        {
            var teacher = DB.Teachers
                    .Where(x => x.UserId == User.Current.Id)
                    .SingleOrDefault();
            if (User.IsInRole("指导老师"))
            {
                //指导老师查看选择了自己的学生
                var student = DB.Subjects
                    .Include(x => x.Student)
                    .Where(x => x.TeacherId == teacher.Id && x.Student.IsGraduate == IsGraduate.否 && x.Student.State == State.锁定&&x.Draw==Draw.通过)
                    .OrderBy(x=>x.StudentId)
                    .ToList();
                var ret = new List<StudentList>();
                foreach (var x in student)
                {
                    ret.Add(new StudentList
                    {
                        Id = x.Student.Id,
                        Name = x.Student.Name,
                        StudentNumber = x.Student.Number.ToString(),
                        College = DB.Colleges.Where(y => y.Id == x.Student.CollegeId).SingleOrDefault().Title,
                        Major = DB.Majors.Where(y => y.Id == x.Student.MajorId).SingleOrDefault().Title,
                        CreateTime = x.Student.CreateTime.ToString(),
                        Teacher=teacher.Name,
                        Subject=x.Title,
                        SubjectNumber=x.Id,
                    });
                }
                return View(ret);
            }
            else
            {
                //系主任查看自己专业相关的学生
                var student = DB.Subjects
                    .Include(x => x.Student)
                    .Where(x=>x.Student.MajorId==teacher.MajorId&&x.Student.IsGraduate==IsGraduate.否&&x.Student.State==State.锁定&&x.Draw==Draw.通过)
                    .OrderBy(x => x.StudentId)
                    .ToList();
                var ret = new List<StudentList>();
                foreach(var x in student) {
                    ret.Add(new StudentList
                    {
                        Id=x.Student.Id,
                        Name=x.Student.Name,
                        StudentNumber=x.Student.Number.ToString(),
                        College=DB.Colleges.Where(y=>y.Id==x.Student.CollegeId).SingleOrDefault().Title,
                        Major=DB.Majors.Where(y=>y.Id==x.Student.MajorId).SingleOrDefault().Title,
                        CreateTime=x.Student.CreateTime.ToString(),
                        Teacher=DB.Teachers.Where(y=>y.Id==x.TeacherId).SingleOrDefault().Name,
                        Subject=x.Title,
                        SubjectNumber = x.Id,
                    });
                }
                return View(ret);
            }
        }
        /// <summary>
        /// 指导老师和系主任查看学生详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AnyRoles("系主任,指导老师")]
        public IActionResult StudentDetails(int id)
        {
            var teacher = DB.Teachers
                    .Where(x => x.UserId == User.Current.Id)
                    .SingleOrDefault();
            if (User.IsInRole("指导老师"))
            {
                var ret = DB.FinleInfos
                    .Include(x => x.Student)
                    .Where(x => x.StudentId == id)
                    .ToList();
                return View(ret);
            }
            else
            {
                var ret = DB.FinleInfos
                    .Include(x=>x.Student)
                    .Where(x => x.StudentId == id)
                    .ToList();
                return View(ret);
            }
        }
        [HttpPost]
        [AnyRoles("系主任,指导老师")]
        public IActionResult EditSubject(int id,string title)
        {
            var old = DB.Subjects
                .Where(x => x.Title == title)
                .SingleOrDefault();
            if (old != null)
            {
                return Content("error");
            }
            else
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
                    subject.Title = title;
                    var log = new Log
                    {
                        Operation = Operation.修改毕业设计题目,
                        Roles = Roles.老师,
                        Time = DateTime.Now,
                        Number = id,
                        UserId = User.Current.Id,
                    };
                    DB.Logs.Add(log);
                    DB.SaveChanges();
                    return Content("success");
                }
            }
        }
        [HttpGet]
        [AnyRoles("系主任,指导老师")]
        public IActionResult GraduateStudentList()
        {
            var student = DB.Subjects
                .Include(x => x.Student)
                .Include(x => x.Teacher)
                .Where(x => x.Student.IsGraduate == IsGraduate.是 && x.Student.MajorId == DB.Teachers.Where(y => y.UserId == User.Current.Id).SingleOrDefault().MajorId&&x.Draw==Draw.通过)
                .OrderByDescending(x=>x.Student.Number)
                .ToList();
            var ret = new List<StudentList>();
            foreach (var x in student)
            {
                ret.Add(new StudentList
                {
                    Id = x.Student.Id,
                    Name = x.Student.Name,
                    StudentNumber = x.Student.Number.ToString(),
                    College = DB.Colleges.Where(y => y.Id == x.Student.CollegeId).SingleOrDefault().Title,
                    Major = DB.Majors.Where(y => y.Id == x.Student.MajorId).SingleOrDefault().Title,
                    CreateTime = x.Student.CreateTime.ToString(),
                    GraduateTime=x.Student.GraduateTime.ToString(),
                    Teacher = x.Teacher.Name,
                    Subject = x.Title,
                });
            }
            return PagedView(ret,50);
        }
        /// <summary>
        /// 执行已经毕业操作
        /// </summary>
        /// <returns></returns>
        [AnyRoles("系主任,指导老师")]
        [HttpPost]
        public IActionResult GraduateJug(int id)
        {
            var student = DB.Students
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (student == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                var result = DB.Results
                    .Include(x => x.Subject)
                    .Include(x => x.Subject.Student)
                    .Where(x => x.Subject.StudentId == student.Id)
                    .ToList();
                if (result.Count > 0)
                {
                    student.IsGraduate = IsGraduate.是;
                    student.GraduateTime = DateTime.Now;
                    DB.SaveChanges();
                    return Content("success");
                }
                else
                {
                    return Content("score");
                }
            }
        }
        [AnyRoles("系主任,指导老师")]
        [HttpGet]
        public IActionResult StudentScore()
        {
            var teacher = DB.Teachers
                .Include(x=>x.Major)
                    .Where(x => x.UserId == User.Current.Id)
                    .SingleOrDefault();
            var subject = DB.Results
                .Include(x=>x.Subject)
                .Include(x=>x.Subject.Student)
                .Include(x=>x.Subject.Teacher)
                .Include(x=>x.Teacher)
                .Where(x => x.Teacher.MajorId == teacher.MajorId)
                .ToList();
                var ret = new List<StudentList>();
                foreach (var x in subject)
                {
                if (x.Score.ToString()=="")
                {
                    x.Score = 0;
                }
                if(x.Subject.Student.IsGraduate == IsGraduate.否)
                {
                    ret.Add(new StudentList
                    {

                        Id = x.Subject.StudentId,
                        Name = x.Subject.Student.Name,
                        StudentNumber = x.Subject.Student.Number.ToString(),
                        College = DB.Colleges.Where(y => y.Id == x.Subject.Student.CollegeId).SingleOrDefault().Title,
                        Major = DB.Majors.Where(y => y.Id == x.Subject.Student.MajorId).SingleOrDefault().Title,
                        CreateTime = x.CreateTime.ToString(),
                        Teacher = x.Subject.Teacher.Name,
                        Subject = x.Subject.Title,
                        SubjectNumber = x.Subject.Id,
                        Result = x.Score.ToString(),
                        ScoreTeacher = x.Teacher.Name,
                    });
                }
                }
                return View(ret.OrderBy(x=>x.StudentNumber).ToList());
        }
        [AnyRoles("系主任,指导老师")]
        [HttpGet]
        public IActionResult WaitScore()
        {
            var teacher = DB.Teachers
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var student = DB.Subjects
                    .Include(x => x.Student)
                    .Where(x => x.Student.MajorId == teacher.MajorId && x.Student.IsGraduate == IsGraduate.否 && x.Student.State == State.锁定 && x.Draw == Draw.通过)
                    .OrderBy(x => x.StudentId)
                    .ToList();
            var ret = new List<StudentList>();
            foreach (var x in student)
            {
                ret.Add(new StudentList
                {
                    Id = x.Student.Id,
                    Name = x.Student.Name,
                    StudentNumber = x.Student.Number.ToString(),
                    College = DB.Colleges.Where(y => y.Id == x.Student.CollegeId).SingleOrDefault().Title,
                    Major = DB.Majors.Where(y => y.Id == x.Student.MajorId).SingleOrDefault().Title,
                    CreateTime = x.Student.CreateTime.ToString(),
                    Teacher = DB.Teachers.Where(y => y.Id == x.TeacherId).SingleOrDefault().Name,
                    Subject = x.Title,
                    SubjectNumber = x.Id,
                });
            }
            return View(ret);
        }
        [AnyRoles("系主任,指导老师")]
        [HttpPost]
        public IActionResult CreateScore(int id,double score)
        {
            var teacher = DB.Teachers
                .Where(x => x.UserId == User.Current.Id)
                .SingleOrDefault();
            var subject = DB.Subjects
                .Include(x=>x.Student)
                .Include(x=>x.Teacher)
                .Where(x => x.Id == id&&x.Student.IsGraduate==IsGraduate.否&&x.Draw==Draw.通过)
                .SingleOrDefault();
            var oldresult = DB.Results
                .Include(x => x.Teacher)
                .Include(x=>x.Subject)
                .Where(x => x.TeacherId == teacher.Id&&x.SubjectId==subject.Id)
                .SingleOrDefault();
            if (oldresult!=null)
            {
                return Content("您已经进行过了评分工作");
            }
            else
            {
                var result = new Result
                {
                    Score = score,
                    SubjectId = subject.Id,
                    TeacherId = teacher.Id,
                    CreateTime=DateTime.Now,
                };
                DB.Results.Add(result);
                DB.SaveChanges();
                var log = DB.Logs.Add(new Log
                {
                    Operation = Operation.评分,
                    Roles = Roles.老师,
                    Number = subject.Id,
                    Time = DateTime.Now,
                    UserId = User.Current.Id,
                });
                DB.SaveChanges();
                return Content("success");
            }
        }
        [AnyRoles("系主任,指导老师")]
        [HttpPost]
        public IActionResult Search(string key)
        {
            //老师只能查找到本专业毕业相关的学生信息
            var student = DB.Subjects
                .Include(x=>x.Student)
                .Include(x=>x.Teacher)
                .Include(x=>x.Student.Major)
                .Include(x=>x.Student.College)
                .Where(x => x.Student.Number.ToString() == key || x.Student.Name == key)
                .Where(x=>x.Student.IsGraduate==IsGraduate.是&&x.Draw==Draw.通过)
                .Where(x=>x.Student.MajorId==DB.Teachers.Where(y=>y.UserId==User.Current.Id).SingleOrDefault().MajorId)
                .ToList();
            ViewBag.Student = student;
            return View("SearchResult");
        }
        [AnyRoles("系主任,指导老师")]
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }
        [AnyRoles("系主任,指导老师")]
        public IActionResult TeacherList()
        {
            var ret = DB.Teachers
                .Include(x=>x.Major)
                .Include(x=>x.College)
                .Where(x => x.MajorId == DB.Teachers.Where(y => y.UserId == User.Current.Id)
                .SingleOrDefault().MajorId)
                .OrderBy(x=>x.Id)
                .ToList();
            return View(ret);
        }
        [Authorize(Roles ="系主任")]
        public IActionResult DeleteTeacher(int id)
        {
            var teacher = DB.Teachers
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (teacher == null)
            {
                return Content("error");
            }
            else
            {
                var user = DB.Users
                    .Where(x => x.Id == teacher.UserId)
                    .SingleOrDefault();
                DB.Users.Remove(user);
                DB.Teachers.Remove(teacher);
                DB.SaveChanges();
                return Content("success");
            }
        }
        /// <summary>
        /// 更改头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatar"></param>
        /// <param name="Model"></param>
        /// <returns></returns>
        [AnyRoles("系主任,指导老师")]
        [HttpPost]
        public async Task<IActionResult> EditAvatar(long id, IFormFile avatar, User Model)
        {
            var user = DB.Users
                .Include(x => x.Avatar)
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
            return RedirectToAction("Manage", "Admin");
        }
    }
}
