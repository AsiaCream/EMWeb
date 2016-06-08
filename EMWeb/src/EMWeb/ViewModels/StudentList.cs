using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.ViewModels
{
    public class StudentList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StudentNumber { get; set; }
        public string College { get; set; }
        public string Major { get; set; }
        public string CreateTime { get; set; }
        public string GraduateTime { get; set; }
        public string Teacher { get; set; }
        public string Subject { get; set; }
    }
}
