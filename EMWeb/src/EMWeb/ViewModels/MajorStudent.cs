using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.ViewModels
{
    public class MajorStudent
    {
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string StudentName { get; set; }
        public string TeacherName { get; set; }
        public string SubjectTitle { get; set; }
        public DateTime PostTime { get; set; }
        public DateTime DrawTime { get; set; }
        public string Draw { get; set; }
        public string Major { get; set; }
    }
}
