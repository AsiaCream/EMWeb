using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using EMWeb.Models;

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
            major.CollegeId = cid.Id;
            DB.SaveChanges();
            return Content("success");
        }
    }
}
