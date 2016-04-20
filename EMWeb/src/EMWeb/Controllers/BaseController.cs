using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using EMWeb.Models;

namespace EMWeb.Controllers
{
    public class BaseController : BaseController<EMContext,User,string>
    {
    }
}
