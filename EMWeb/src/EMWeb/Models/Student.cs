using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public enum State
    {//题目是否锁定
        未锁定,
        锁定
    }
    public enum IsGraduate
    {//是否毕业
        是,
        否
    }
    public class Student
    {
        public int Id { get; set; }

        [MaxLength(12)]
        public string Name { get; set; }

        [MaxLength(10)]
        public int Number { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime GraduateTime { get; set; }

        public State State { get; set; }

        public IsGraduate IsGraduate { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("College")]
        public int CollegeId { get; set; }
        public virtual College College { get; set; }

        [ForeignKey("Major")]
        public int MajorId { get; set; }
        public virtual Major Major { get; set; }

    }
}
