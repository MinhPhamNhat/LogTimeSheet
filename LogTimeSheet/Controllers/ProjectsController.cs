using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LogTimeSheet.Config;
using LogTimeSheet.Models;
using LogTimeSheet.Repo;
using LogTimeSheet.Utils;

namespace LogTimeSheet.Controllers
{
    public class ProjectsController : ApiController
    {
        Jwt jwtValidator = new Jwt();
        private SystemContext db = new SystemContext();
        private ProjectDAO projectDAO;

        // GET: api/Projects -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả project 
        /// </summary>
        [AuthorizeRequest("All")]
        [Route("api/Projects")]
        [HttpGet]
        public List<Project> Getprojects()
        {
            projectDAO = new ProjectDAO(db);
            var token = Request.Headers.GetValues("token").First();
            dynamic user = jwtValidator.ValidateToken(token);
            return projectDAO.getProjectList(Convert.ToString(user.id));
        }

        // GET: api/Projects/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy project theo id
        /// </summary>
        [AuthorizeRequest("All")]
        [Route("api/Projects/{ProjectId}")]
        [HttpGet]
        public IHttpActionResult GetProject(int ProjectId)
        {
            projectDAO = new ProjectDAO(db);
            var token = Request.Headers.GetValues("token").First();
            dynamic user = jwtValidator.ValidateToken(token);
            Project project = projectDAO.getProject(ProjectId, Convert.ToString(user.id));
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        // PUT: api/Projects/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Sửa project theo id
        /// - Params: 
        /// Optional{ Name:String, StartDate:DateTime(Format: 1/1/2010 12:10:15 PM), EndDate:DateTime(Format: 1/1/2010 12:10:15 PM), Type:Boolean, ProjectCode:String, Manager:String }
        /// </summary>
        [AuthorizeRequest("Admin")]
        [Route("api/Projects/{ProjectId}")]
        [HttpPut]
        public IHttpActionResult PutProject(int ProjectId, [FromBody] dynamic project)
        {
            projectDAO = new ProjectDAO(db);

            var token = Request.Headers.GetValues("token").First();
            dynamic user = jwtValidator.ValidateToken(token);

            Project oldProject = projectDAO.getProject(ProjectId, Convert.ToString(user.id));

            if (oldProject == null)
            {

                return NotFound();
            }
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(project.Name)))
                {
                    oldProject.Name = Convert.ToString(project.Name);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.startTime)))
                {
                    DateTime StartDate = Convert.ToDateTime(Convert.ToString(project.StartDate));
                    if (DateTime.Compare(StartDate, oldProject.EndDate) >= 0)
                    {
                        return BadRequest(StartDate.ToLocalTime() + " Start day must be sooner than the OLD end day " + oldProject.EndDate.ToLocalTime());
                    }
                    oldProject.StartDate = StartDate;
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.endTime)))
                {
                    DateTime EndDate = Convert.ToDateTime(Convert.ToString(project.EndDate));
                    if (DateTime.Compare(oldProject.StartDate, EndDate) >= 0)
                    {
                        return BadRequest(oldProject.StartDate.ToLocalTime() + " Start day must be sooner than the end day " + EndDate.ToLocalTime());
                    }
                    oldProject.EndDate = EndDate;
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.Type)))
                {
                    oldProject.Type = Convert.ToBoolean(project.Type);
                    if (oldProject.Type)
                    {
                        if (oldProject.Manager != null)
                        {
                            ProjectUser ProjectManager = oldProject.ProjectUsers.FirstOrDefault(pu => pu.ProjectId == oldProject.ProjectId && pu.UserId == oldProject.Manager.UserId);
                            oldProject.ProjectUsers.Remove(ProjectManager);
                            oldProject.Manager = null;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.ProjectCode)))
                {
                    oldProject.ProjectCode = Convert.ToString(project.ProjectCode);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.Manager)))
                {
                    if (oldProject.Type)
                    {
                        return BadRequest("Type true (Default project) must not has Manager!!!");
                    }
                    string ManagerId = Convert.ToString(project.Manager);
                    User Manager = db.Users.FirstOrDefault(u => u.UserId == ManagerId);
                    if (Manager == null)
                    {
                        return BadRequest("Manager not found");
                    }
                    if (oldProject.Manager != null)
                    {
                        ProjectUser ProjectManager = oldProject.ProjectUsers.FirstOrDefault(pu => pu.ProjectId == oldProject.ProjectId && pu.UserId == oldProject.Manager.UserId);
                        oldProject.ProjectUsers.Remove(ProjectManager);
                    }
                    oldProject.ProjectUsers.Add(new ProjectUser() { ProjectId = oldProject.ProjectId, UserId = Manager.UserId });
                    oldProject.Manager = Manager;
                }

                db.SaveChanges();
                return Ok(oldProject);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        // POST: api/Projects -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Thêm project
        /// - Params: 
        /// Required{Name:String, StartDate:DateTime(Format: 1/1/2010 12:10:15 PM), EndDate:DateTime(Format: 1/1/2010 12:10:15 PM), Type:Boolean, ProjectCode:String }
        /// Optional{ Manager:String }
        /// </summary>
        [AuthorizeRequest("Admin")]
        [Route("api/Projects")]
        [ResponseType(typeof(Project))]
        public IHttpActionResult PostProject([FromBody] dynamic project)
        {
            projectDAO = new ProjectDAO(db);
            if (string.IsNullOrEmpty(Convert.ToString(project.Name)) ||
                string.IsNullOrEmpty(Convert.ToString(project.StartDate)) ||
                string.IsNullOrEmpty(Convert.ToString(project.EndDate)) ||
                string.IsNullOrEmpty(Convert.ToString(project.Type)) ||
                string.IsNullOrEmpty(Convert.ToString(project.ProjectCode)))
            {
                return BadRequest("Invalid params");
            }
            string Name = Convert.ToString(project.Name);
            DateTime InitTime = DateTime.Now;
            string ProjectCode = Convert.ToString(project.ProjectCode);
            try
            {
                bool Type = Convert.ToBoolean(project.Type);

                if (Type && !string.IsNullOrEmpty(Convert.ToString(project.Manager)))
                {
                    return BadRequest("Type true (Default project) must not has Manager!!!");
                }
                //  "1/1/2010 12:10:15 PM"
                DateTime StartDate = Convert.ToDateTime(Convert.ToString(project.StartDate));
                DateTime EndDate = Convert.ToDateTime(Convert.ToString(project.EndDate));
                if (DateTime.Compare(StartDate, EndDate) >= 0)
                {
                    return BadRequest(StartDate.ToLocalTime() + " End day must be greater than start day " + EndDate.ToLocalTime());
                }

                User ADMIN = db.Users.FirstOrDefault(u => u.UserId == "ADMIN");
                User Manager = null;
                if (!string.IsNullOrEmpty(Convert.ToString(project.Manager)))
                {
                    string ManagerId = Convert.ToString(project.Manager);
                    Manager = db.Users.FirstOrDefault(u => u.UserId == ManagerId);
                    if (Manager == null)
                    {
                        return BadRequest("Manager not found");
                    }
                }

                Project pro = projectDAO.addProject(new Project()
                {
                    Name = Name,
                    ProjectCode = ProjectCode,
                    InitTime = InitTime,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Type = Type,
                    Manager = Manager,
                });
                if (Manager != null)
                {
                    pro.ProjectUsers = new List<ProjectUser>() {
                        new ProjectUser() { User = Manager, Project = pro },
                        new ProjectUser() { User = ADMIN, Project = pro }
                    };
                }
                else
                {
                    pro.ProjectUsers = new List<ProjectUser>() {
                        new ProjectUser() { User = ADMIN, Project = pro }
                    };
                }
                db.SaveChanges();
                return Ok(pro);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/Projects/assignProjectToStaff/5 -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Assign Staff vào project
        /// - Params: 
        /// Required{ UserId:String }
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [Route("api/Projects/assignProjectToStaff/{ProjectId}")]
        [HttpPut]
        public IHttpActionResult AssignProjectToStaff(int ProjectId, [FromBody] dynamic staff)
        {
            projectDAO = new ProjectDAO(db);

            var token = Request.Headers.GetValues("token").First();
            dynamic user = jwtValidator.ValidateToken(token);

            Project Project = projectDAO.getProject(ProjectId, Convert.ToString(user.id));

            if (Project != null)
            {
                if (Project.Type)
                {
                    return Ok(new { statusCode = 403, message = "You cannot modify this project (default project)" });
                }
                try
                {
                    string UserId = Convert.ToString(staff.UserId);
                    if (string.IsNullOrEmpty(UserId))
                    {
                        return BadRequest("Invalid params");
                    }
                    User User = db.Users.FirstOrDefault(u => u.UserId == UserId);
                    if (User == null)
                    {
                        return BadRequest("Staff not found");
                    }
                    Project.ProjectUsers.Add(new ProjectUser() { Project = Project, User = User });
                    db.SaveChanges();
                    return Ok(Project);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            return NotFound();
        }

        // DELETE: api/Projects/5 -- PASS
        /// <summary>
        /// - Role: Admin
        /// - Desc: Xoá project
        /// </summary>
        [AuthorizeRequest("Admin")]
        [Route("api/Projects/{ProjectId}")]
        [HttpDelete]
        public IHttpActionResult DeleteProject(int ProjectId)
        {
            Project project = db.Projects.Find(ProjectId);
            if (project == null)
            {
                return NotFound();
            }

            db.Projects.Remove(project);
            db.SaveChanges();

            return Ok(project);
        }
    }
}