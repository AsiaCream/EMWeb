using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public enum Draw
    {
        待审核,
        通过,
        未通过
    }
    public class Subject
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Title { get; set; }

        public Draw Draw { get; set; }

        public DateTime PostTime { get; set; }

        public DateTime DrawTime { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
