using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public class Thesis
    {//毕业论文
        public int Id { get; set; }

        public string Title { get; set; }

        public string Evaluation{ get; set; }//评语

        public double Score { get; set; }//分数

        [ForeignKey("User")]
        public long UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("FileInfo")]
        public int FileInfoId { get; set; }
        public virtual FileInfo FileInfo { get; set; }
    }
}
