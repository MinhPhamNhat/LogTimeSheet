using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
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

        public int numberOfProjects()
        {
            var count = db.Projects
            .Count();
            return count;
        }

        public int numberOfLogs()
        {
            var count = db.Logs
            .Count();
            return count;
        }
    }
}