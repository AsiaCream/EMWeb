using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using EMWeb.Models;

namespace EMWeb.Controllers
{
    public class FileController : BaseController
    {
        [HttpPost]
       public IActionResult UploadReport(int id,IFormFile file)
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult UploadSourceCode()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadDocument()
        {
            return View();
        }
    }
}
