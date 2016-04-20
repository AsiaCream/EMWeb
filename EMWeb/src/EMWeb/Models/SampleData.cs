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

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (DB.Database != null && DB.Database.EnsureCreated())
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "系主任" });
                await roleManager.CreateAsync(new IdentityRole { Name = "指导老师" });
                await roleManager.CreateAsync(new IdentityRole { Name = "学生" });

                var headteacher = new User { UserName = "Admin",Email="343224963@qq.com" };
                await userManager.CreateAsync(headteacher, "Cream2015!@#");
                await userManager.AddToRoleAsync(headteacher, "系主任");

                var teacher = new User { UserName = "Cream", Email = "343224963@qq.com" };
                await userManager.CreateAsync(teacher, "Cream2015!@#");
                await userManager.AddToRoleAsync(teacher, "指导老师");

                var guest = new User { UserName = "Guest", Email = "627148026@qq.com" };
                await userManager.CreateAsync(guest, "Cream2015!@#");
                await userManager.AddToRoleAsync(guest, "学生");

                DB.Colleges.Add(new College { Title = "计算机与控制工程学院" });

            }
            DB.SaveChanges();
        }
    }
}
