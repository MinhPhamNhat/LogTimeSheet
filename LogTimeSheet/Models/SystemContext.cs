using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Models
{
    public class SystemContext : DbContext
    {
        //"Data Source=103.151.241.83;Initial Catalog=LogTimeSheet;User ID=sa;Password=Sa@123456;"
        //"Data Source=MinhPham;Initial Catalog=LogTimeSheet;Integrated Security=true;"
        public SystemContext() : base("Data Source=103.151.241.83;Initial Catalog=LogTimeSheet;User ID=sa;Password=Sa@123456;") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
    }
}