using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        
        [MaxLength(12)]
        public string Name { get; set; }

        [MaxLength(10)]
        public int Number { get; set; }

        public DateTime CreateTime { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("College")]
        public int CollegeId { get; set; }
        public virtual College College { get; set; }

        [ForeignKey("Major")]
        public int MajorId { get; set; }
        public virtual Major Major { get; set; }
    }
}
