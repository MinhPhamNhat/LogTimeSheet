using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class LogDAO
    {
        SystemContext db;
        public LogDAO(SystemContext db)
        {
            this.db = db;
        }

        public List<Log> getAll(dynamic user)
        {
            int role = int.Parse(user.role);
            if (role == 0)
            {
                return db.Logs.ToList();
            }
            else if (role == 1)
            {
                string manager = Convert.ToString(user.id);
                return db.Logs.Where(l => l.Subtask.Project.Manager.UserId.Equals(manager)).ToList();
            }
            else
            {
                string staff = Convert.ToString(user.id);
                return db.Logs.Where(l => l.User.UserId.Equals(staff)).ToList();
            }
        }

        public List<Log> getLogsByUser(string UserId)
        {
           return db.Logs.Where(l => l.User.UserId == UserId).ToList();
        }

        public List<Log> getUserLogsByProject(string UserId, int ProjectId)
        {
            return db.Logs.Where(l => l.User.UserId == UserId && l.Subtask.Project.ProjectId == ProjectId).ToList();
        }

        public List<Log> getUserLogsBySubtask(string UserId, int SubtaskId)
        {
            return db.Logs.Where(l => l.User.UserId == UserId && l.Subtask.SubtaskId == SubtaskId).ToList();
        }

        public List<Log> getLogsByUser(dynamic user, string UserId)
        {
            string AuthenUserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);
            if (Role == 0)
            {
                return db.Logs.Where(l => l.User.UserId == UserId).ToList();
            }
            else if (Role == 1)
            {
                return db.Logs.Where(l => l.User.UserId == UserId && l.Subtask.Project.Manager.UserId.Equals(AuthenUserId)).ToList();

            }
            return null;
        }
        
        public List<Log> getAllLogsByProject(dynamic user, int ProjectId)
        {
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);
            if (Role == 0)
            {
                return db.Logs.Where(l => l.Subtask.Project.ProjectId == ProjectId).ToList();
            }
            else if (Role == 1)
            {
                return db.Logs.Where(l => l.Subtask.Project.ProjectId == ProjectId && l.Subtask.Project.Manager.UserId.Equals(UserId)).ToList();
            }
            return null;
        }

        public List<Log> getAllLogsBySubtask(dynamic user, int SubtaskId)
        {
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);
            if (Role == 0)
            {
                return db.Logs.Where(l => l.Subtask.SubtaskId == SubtaskId).ToList();
            }
            else if (Role == 1)
            {
                return db.Logs.Where(l => l.Subtask.SubtaskId == SubtaskId && l.Subtask.Project.Manager.UserId.Equals(UserId)).ToList();
            }
            return null;
        }

        
        public Log getLog(dynamic user, int LogId)
        {
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);
            if (Role == 0)
            {
                return db.Logs.FirstOrDefault(l => l.LogId == LogId);
            }
            else if (Role == 1)
            {
                return db.Logs.FirstOrDefault(l => l.LogId == LogId && l.Subtask.Project.Manager.UserId.Equals(UserId));
            }
            else if (Role == 2)
            {
                return db.Logs.FirstOrDefault(l => l.LogId == LogId && l.User.UserId.Equals(UserId));
            }
            return null;
        }

        public Log addLog(Log log)
        {
            Log l = db.Logs.Add(log);
            db.SaveChanges();
            return l;
        }

        public Log deleteLog(dynamic user, int LogId)
        {
            Log log = getLog(user, LogId);
            if (log != null)
            {
                db.Logs.Remove(log);
                db.SaveChanges();
            }
            return log;
        }
    }
}