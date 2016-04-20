using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMWeb.Models
{
    public enum Roles
    {//日志角色
        学生,
        老师
    }
    public enum Operation
    {//日志情况
        下载文件,
        查看文件,
        上传文件,
        添加项目,
        修改项目,
        删除项目,
        编辑个人信息,
        添加学生,
        删除学生,
        审核论文,
        审核题目,
    }
    public class Log
    {
        public int Id { get; set; }

        public Roles Roles { get; set; }

        public Operation Operation { get; set; }

        public DateTime Time { get; set; }//操作时间

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
