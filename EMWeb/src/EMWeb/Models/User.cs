using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using CodeComb.AspNet.Upload.Models;
namespace EMWeb.Models
{
    public class User:IdentityUser<long>
    {
        [ForeignKey("Avatar")]
        public Guid? AvatarId { get; set; }
        public virtual File Avatar { get; set; }
    }
}
