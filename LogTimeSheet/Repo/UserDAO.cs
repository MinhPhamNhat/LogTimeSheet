using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class UserDAO
    {
        SystemContext db;
        public UserDAO(SystemContext db)
        {
            this.db = db;
        }

        public List<User> getList()
        {
            return db.Users.ToList();
        }
        public List<User> getManager()
        {
            return db.Users.Where(u=>u.Role == 1).ToList();
        }
        public List<User> getStaff()
        {
            return db.Users.Where(u => u.Role == 2).ToList();
        }
        public User getUser(string UserId)
        {
            return db.Users.Find(UserId);
        }
    }
}