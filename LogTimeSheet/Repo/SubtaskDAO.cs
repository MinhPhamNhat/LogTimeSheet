using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class SubtaskDAO 
    {
        SystemContext db;
        public SubtaskDAO(SystemContext db)
        {
            this.db = db;
        }

        public List<Subtask> getSubtaskByProject(dynamic user, int ProjectId)
        {
            string UserId = Convert.ToString(user.id);
            return db.Subtasks.Where(s => s.Project.ProjectId == ProjectId && (s.Project.ProjectUsers.Contains(s.Project.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == ProjectId)) || s.Project.Type)).ToList();
        }

        public List<Subtask> getList(dynamic user, int page, int limit)
        {
            string UserId = Convert.ToString(user.id);
            int role = Convert.ToInt32(user.role);
            return db.Subtasks.Where(s => s.Project.ProjectUsers.Contains(s.Project.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == s.Project.ProjectId)) || s.Project.Type).OrderByDescending(p => p.Name).Skip(limit * (page - 1)).Take(limit).ToList().ToList();
        }

        public Subtask getSubtask(dynamic user, int SubtaskId)
        {
            string UserId = Convert.ToString(user.id);
            return db.Subtasks.FirstOrDefault(s => s.SubtaskId == SubtaskId && (s.Project.ProjectUsers.Contains(s.Project.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == s.Project.ProjectId)) || s.Project.Type));
        }

        public Subtask addSubtask(Subtask subtask)
        {
            Subtask s = db.Subtasks.Add(subtask);
            db.SaveChanges();
            return s;
        }

        public Subtask deleteSubtask(dynamic user, int SubtaskId)
        {
            int Role = Convert.ToInt32(user.role);
            string UserId= Convert.ToString(user.id);
            Subtask subtask = null;
            if (Role == 0)
            {
                subtask = db.Subtasks.FirstOrDefault(s => s.SubtaskId == SubtaskId);
            }else if (Role == 1)
            {
                subtask = db.Subtasks.FirstOrDefault(s => s.SubtaskId == SubtaskId && s.Project.Manager.UserId.Equals(UserId));
            }
            if (subtask != null)
            {
                db.Subtasks.Remove(subtask);
                db.SaveChanges();
            }
            return subtask;
        }
            

    }
}