using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EMWeb.Models
{
    public static class SampleData
    {
        public async static Task InitDB(IServiceProvider services)
        {
            var DB = services.GetRequiredService<EMContext>();

            var userManager = services.GetRequiredService<UserManager<User>>();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();

            if (DB.Database != null && DB.Database.EnsureCreated())
            {
                await roleManager.CreateAsync(new IdentityRole<long> { Name = "系主任" });
                await roleManager.CreateAsync(new IdentityRole<long> { Name = "指导老师" });
                await roleManager.CreateAsync(new IdentityRole<long> { Name = "学生" });

                var headteacher = new User { UserName = "Admin",Email="343224963@qq.com" };
                await userManager.CreateAsync(headteacher, "123456");
                await userManager.AddToRoleAsync(headteacher, "系主任");

                var teacher = new User { UserName = "Cream", Email = "343224963@qq.com" };
                await userManager.CreateAsync(teacher, "123456");
                await userManager.AddToRoleAsync(teacher, "指导老师");

                var guest = new User { UserName = "Guest", Email = "627148026@qq.com" };
                await userManager.CreateAsync(guest, "123456");
                await userManager.AddToRoleAsync(guest, "学生");

                var college = new College { Title = "计算机与控制工程学院" };
                DB.Colleges.Add(college);
                var major = new Major { CollegeId = college.Id, Title = "软件工程" };
                DB.Majors.Add(major);
                var announcement = new Announcement
                {
                    Title = "请提交毕业设计题目",
                    Content = "请提交毕业设计题目",
                    CreateTime = DateTime.Now,
                };
                DB.Announcements.Add(announcement);
                DB.SaveChanges();
                var teacherzhang = new Teacher
                {   Name = "张老师",
                    Number = 2012023110,
                    CreateTime = DateTime.Now,
                    UserId=headteacher.Id,
                    CollegeId = college.Id,
                    MajorId = major.Id
                };
                DB.Teachers.Add(teacherzhang);

                var teacherfdd = new Teacher
                {
                    Name = "王老师",
                    Number = 2012023005,
                    CreateTime = DateTime.Now,
                    UserId = teacher.Id,
                    CollegeId = college.Id,
                    MajorId = major.Id,
                };
                DB.Teachers.Add(teacherfdd);
                var studentdu = new Student
                {
                    Name = "杜晨勇",
                    Number = 2012023009,
                    CreateTime = DateTime.Now,
                    UserId = guest.Id,
                    CollegeId = college.Id,
                    MajorId = major.Id,
                };
                DB.Students.Add(studentdu);
                DB.SaveChanges();
                var log1 = new Log {
                    Roles = Roles.系主任,
                    Operation = Operation.添加系主任,
                    Time = DateTime.Now,
                    Number = teacherzhang.Id,
                    UserId = headteacher.Id
                };
                DB.Logs.Add(log1);

                var log2 = new Log {
                    Roles = Roles.系主任,
                    Operation = Operation.添加老师,
                    Time = DateTime.Now,
                    Number = teacherfdd.Id,
                    UserId = headteacher.Id
                };
                DB.Logs.Add(log2);

                var log3 = new Log {
                    Roles = Roles.系主任,
                    Operation = Operation.添加学生,
                    Time = DateTime.Now,
                    Number = studentdu.Id,
                    UserId = headteacher.Id
                };
                DB.Logs.Add(log3);
                var log4 = new Log
                {
                    Roles = Roles.系主任,
                    Operation = Operation.添加系统公告,
                    Time = DateTime.Now,
                    Number=announcement.Id,
                    UserId = headteacher.Id
                };
                DB.Logs.Add(log4);
                
            }
            DB.SaveChanges();
        }
    }
}
