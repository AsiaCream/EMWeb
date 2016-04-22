using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.ViewModels
{
    public class SubjectLog
    {
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string StudentName { get; set; }
        public int SNumber { get; set; }
        public string SName { get; set; }
        public DateTime Time { get; set; }
    }
}
