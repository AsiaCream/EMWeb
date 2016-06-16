using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using CodeComb.AspNet.Upload;
using CodeComb.AspNet.Upload.Models;

namespace EMWeb.Models
{
    public class EMContext:IdentityDbContext<User,IdentityRole<long>,long>,IFileUploadDbContext
    {
        public DbSet<FileInfo> FinleInfos { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Information> Informations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FileInfo>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.StudentId);
            });
            builder.Entity<Teacher>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.CollegeId);
                e.HasIndex(x => x.MajorId);
                e.HasIndex(x => x.Number);
                e.HasIndex(x => x.UserId);
            });
            builder.Entity<Student>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.UserId);
                e.HasIndex(x => x.CollegeId);
                e.HasIndex(x => x.MajorId);
                e.HasIndex(x => x.Number);
                e.HasIndex(x => x.GraduateTime);
            });
            builder.Entity<College>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<Major>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CollegeId);
            });
            builder.Entity<Subject>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.DrawTime);
                e.HasIndex(x => x.PostTime);
                e.HasIndex(x => x.StudentId);
                e.HasIndex(x => x.TeacherId);
            });
            builder.Entity<Log>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Number);
                e.HasIndex(x => x.UserId);
            });
            builder.Entity<Announcement>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.MajorId);
            });
            builder.Entity<Result>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.TeacherId);
                e.HasIndex(x => x.SubjectId);
                e.HasIndex(x => x.Score);
            });
            builder.Entity<Information>(e =>
            {
                e.HasIndex(x => x.Id);
                e.HasIndex(x => x.ReadTime);
                e.HasIndex(x => x.CreateTime);
                e.HasIndex(x => x.SNumber);
                e.HasIndex(x => x.TNumber);
            });
        }
    }
}
