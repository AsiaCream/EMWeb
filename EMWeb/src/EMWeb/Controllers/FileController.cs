using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace EMWeb.Controllers
{
    public class FileController : BaseController
    {
       public IActionResult Upload()
        {
            return View();
        }
    }
}
