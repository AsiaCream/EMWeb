using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EMWeb.Models
{
    public enum FType
    {
        论文,
        报告,
        源代码,
        文档,
        外文翻译,
    }
    public class FileInfo
    {
        public int Id { get; set; }
        public FType FType { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public DateTime CreateTime { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
    }
}
