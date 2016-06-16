using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.Models
{
    public class Information
    {
        public int Id { get; set; }
        public int SNumber { get; set; }
        public int TNumber { get; set; }//老师ID
        public bool IsRead { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ReadTime { get; set; }
    }
}
