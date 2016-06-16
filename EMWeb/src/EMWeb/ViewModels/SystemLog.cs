using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.ViewModels
{
    public class SystemLog
    {
        public int Id { get; set; }
        public int AdminNumber { get; set; }
        public string AdminName { get; set; }
        public string Role { get; set; }
        public string Operation { get; set; }
        public DateTime Time { get; set; }
        public int TargetNumber { get; set; }
        public string STitle { get; set; }
    }
}
