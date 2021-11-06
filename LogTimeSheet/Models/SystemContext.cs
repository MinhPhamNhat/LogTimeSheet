using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Models
{
    public class SystemContext : DbContext
    {
        public SystemContext() : base("Data Source=MinhPham;Initial Catalog=LogTimeSheet;Integrated Security=True") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
    }
}