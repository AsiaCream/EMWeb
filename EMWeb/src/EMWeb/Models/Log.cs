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
        老师,
        系主任,
    }
    public enum Operation
    {//日志情况
        //学生
        下载文件,
        查看文件,
        上传文件,
        添加毕业设计选题,
        修改毕业设计题目,
        删除毕业设计题目,
        添加项目,
        修改项目,
        删除项目,
        编辑个人信息,
        //老师和系主任
        审核论文,
        审核题目通过,
        审核题目未通过,
        评分,
        //系主任
        添加专业,
        编辑专业,
        删除专业,
        添加学院,
        编辑学院,
        删除学院,
        添加学生,
        添加老师,
        添加系主任,
        删除学生,
        删除老师,
        删除系主任,
        添加系统公告,
    }
    public class Log
    {
        public int Id { get; set; }

        public Roles Roles { get; set; }

        public Operation Operation { get; set; }

        public DateTime Time { get; set; }//操作时间

        public int Number { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public virtual User User { get; set; }
    }
}
