using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EMWeb.Models
{
    public class EMContext:IdentityDbContext<User>
    {
        public DbSet<File> Files { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<File>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<Teacher>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<Student>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<College>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<Major>(e =>
            {
                e.HasIndex(x => x.Id);
            });
            builder.Entity<Subject>(e =>
            {
                e.HasIndex(x => x.Id);
            });
        }
    }
}
