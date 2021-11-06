using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class Login
    {
        SystemContext db;

        public Login(SystemContext db)
        {
            this.db = db;
        }
        public User checkUser(string username, string password)
        {
            User user = db.Users.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
            return user;
        }
    }
}