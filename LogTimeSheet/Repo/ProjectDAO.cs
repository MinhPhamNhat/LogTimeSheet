using LogTimeSheet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Repo
{
    public class ProjectDAO
    {
        SystemContext db;
        public ProjectDAO(SystemContext db)
        {
            this.db = db;
        }

        public Project getProject(int ProjectId, string UserId)
        {
            return db.Projects.FirstOrDefault(p => p.ProjectId == ProjectId && (p.ProjectUsers.Contains(p.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == ProjectId)) || p.Type));
        }

        public List<Project> getProjectList(string UserId)
        {
            return db.Projects.Where(p => p.ProjectUsers.Contains(p.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == p.ProjectId)) || p.Type).ToList();
        }

        public Project addProject(Project project)
        {
            db.Projects.Add(project);
            db.SaveChanges();
            return project;
        }


    }
}