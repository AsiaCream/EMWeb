using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace EMWeb.Models
{
    public class File
    {
        public int Id { get; set; }

        public string Path { get; set; }
    }
}
