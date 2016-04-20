using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public class Major
    {//专业
        public int Id { get; set; }

        public string Title { get; set; }

        [ForeignKey("College")]
        public int CollegeId { get; set; }
        public virtual College College { get; set; }
    }
}
