using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class StatisticDAO
    {
        SystemContext db;
        public StatisticDAO(SystemContext db)
        {
            this.db = db;
        }

        public int numberOfUser()
        {
            var count = db.Users
            .Count();
            return count;
        }

        public int numberOfStaffs()
        {
            var count = db.Users
            .Where(u => u.Role == 2)
            .Count();
            return count;
        }

        public int numberOfPMs()
        {
            var count = db.Users
            .Where(u => u.Role == 1)
            .Count();
            return count;
        }

        public int numberOfProjects(dynamic user)
        {
            string userId = Convert.ToString(user.id);
            var count = db.Projects.Where(p => p.ProjectUsers.Contains(p.ProjectUsers.FirstOrDefault(_ => _.UserId == userId && _.ProjectId == p.ProjectId)) || p.Type)
            .Count();
            return count;
        }

        public int numberOfLogs(dynamic user)
        {
            int role = Convert.ToInt32(user.role);
            if (role == 0)
            {
                return db.Logs.Count();
            }
            else if (role == 1)
            {
                string manager = Convert.ToString(user.id);
                return db.Logs.Where(l => l.Subtask.Project.Manager.UserId.Equals(manager)).Count();
            }
            else
            {
                string staff = Convert.ToString(user.id);
                return db.Logs.Where(l => l.User.UserId.Equals(staff)).Count();
            }
        }

        public List<Log> currWeekLogs(dynamic user)
        {
            string userId = Convert.ToString(user.id);
            int role = Convert.ToInt32(user.role);
            //return db.Projects.Where(l => SqlFunctions.DatePart("ISO_WEEK", l.InitTime) == SqlFunctions.DatePart("ISO_WEEK", DateTime.Now) && SqlFunctions.DatePart("year", l.StartDate) == SqlFunctions.DatePart("year", DateTime.Now)).ToList();
            if (role == 0)
            {
                return db.Logs.Where(l => SqlFunctions.DatePart("ISO_WEEK", l.InitTime) == SqlFunctions.DatePart("ISO_WEEK", DateTime.Now) && SqlFunctions.DatePart("year", l.InitTime) == SqlFunctions.DatePart("year", DateTime.Now)).ToList();
            }
            else if (role == 1)
            {
                string manager = Convert.ToString(user.id);
                return db.Logs.Where(l => l.Subtask.Project.Manager.UserId.Equals(manager) && SqlFunctions.DatePart("ISO_WEEK", l.InitTime) == SqlFunctions.DatePart("ISO_WEEK", DateTime.Now) && SqlFunctions.DatePart("year", l.InitTime) == SqlFunctions.DatePart("year", DateTime.Now)).ToList();
            }
            else
            {
                string staff = Convert.ToString(user.id);
                return db.Logs.Where(l => l.User.UserId.Equals(staff) && SqlFunctions.DatePart("ISO_WEEK", l.InitTime) == SqlFunctions.DatePart("ISO_WEEK", DateTime.Now) && SqlFunctions.DatePart("year", l.InitTime) == SqlFunctions.DatePart("year", DateTime.Now)).ToList();
            }
        }
    }
}