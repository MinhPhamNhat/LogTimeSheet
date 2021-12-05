using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Http.Results;
using LogTimeSheet.Config;
using LogTimeSheet.Models;
using LogTimeSheet.Repo;
using LogTimeSheet.Utils;
using Newtonsoft.Json;

namespace LogTimeSheet.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
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
            var token = Request.Headers.Authorization.Parameter;
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
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            Project project = projectDAO.getProject(ProjectId, Convert.ToString(user.id));
            if (project == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new
                {
                    message = "Project not found"
                });
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

            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);

            Project oldProject = projectDAO.getProject(ProjectId, Convert.ToString(user.id));

            if (oldProject == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Project not found" });
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
                        return responseMessage(HttpStatusCode.BadRequest, new { message = (StartDate.ToLocalTime() + " Start day must be sooner than the OLD end day " + oldProject.EndDate.ToLocalTime()) });
                    }
                    oldProject.StartDate = StartDate;
                }
                if (!string.IsNullOrEmpty(Convert.ToString(project.endTime)))
                {
                    DateTime EndDate = Convert.ToDateTime(Convert.ToString(project.EndDate));
                    if (DateTime.Compare(oldProject.StartDate, EndDate) >= 0)
                    {
                        return responseMessage(HttpStatusCode.BadRequest, new { message = (oldProject.StartDate.ToLocalTime() + " Start day must be sooner than the end day " + EndDate.ToLocalTime()) });
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
                        return responseMessage(HttpStatusCode.BadRequest, new { message = "Type true (Default project) must not has Manager!!!" });
                    }
                    string ManagerId = Convert.ToString(project.Manager);
                    User Manager = db.Users.FirstOrDefault(u => u.UserId == ManagerId);
                    if (Manager == null)
                    {
                        return responseMessage(HttpStatusCode.NotFound, new { message = "Manager not found" });
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
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
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
        [HttpPost]
        public IHttpActionResult PostProject([FromBody] dynamic project)
        {
            projectDAO = new ProjectDAO(db);
            if (string.IsNullOrEmpty(Convert.ToString(project.Name)) ||
                string.IsNullOrEmpty(Convert.ToString(project.StartDate)) ||
                string.IsNullOrEmpty(Convert.ToString(project.EndDate)) ||
                string.IsNullOrEmpty(Convert.ToString(project.Type)) ||
                string.IsNullOrEmpty(Convert.ToString(project.ProjectCode)))
            {
                return responseMessage(HttpStatusCode.BadRequest, new { message = "Invalid params" });
            }
            string Name = Convert.ToString(project.Name);
            DateTime InitTime = DateTime.Now;
            string ProjectCode = Convert.ToString(project.ProjectCode);
            try
            {
                bool Type = Convert.ToBoolean(project.Type);

                if (Type && !string.IsNullOrEmpty(Convert.ToString(project.Manager)))
                {
                    return responseMessage(HttpStatusCode.BadRequest, new { message = "Type true (Default project) must not has Manager!!!" });
                }
                //  "1/1/2010 12:10:15 PM"
                DateTime StartDate = Convert.ToDateTime(Convert.ToString(project.StartDate));
                DateTime EndDate = Convert.ToDateTime(Convert.ToString(project.EndDate));
                if (DateTime.Compare(StartDate, EndDate) >= 0)
                {
                    return responseMessage(HttpStatusCode.BadRequest, new { message = (StartDate.ToLocalTime() + " End day must be greater than start day " + EndDate.ToLocalTime()) });
                }

                User ADMIN = db.Users.FirstOrDefault(u => u.UserId == "ADMIN");
                User Manager = null;
                if (!string.IsNullOrEmpty(Convert.ToString(project.Manager)))
                {
                    string ManagerId = Convert.ToString(project.Manager);
                    Manager = db.Users.FirstOrDefault(u => u.UserId == ManagerId);
                    if (Manager == null)
                    {
                        return responseMessage(HttpStatusCode.NotFound, new { message = "Manager not found" });
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
                return Content(HttpStatusCode.Created, pro);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
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

            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);

            Project Project = projectDAO.getProject(ProjectId, Convert.ToString(user.id));

            if (Project != null)
            {
                if (Project.Type)
                {
                    return responseMessage(HttpStatusCode.Forbidden, new { message = "You cannot modify this project (default project)" });
                }
                try
                {
                    string UserId = Convert.ToString(staff.UserId);
                    if (string.IsNullOrEmpty(UserId))
                    {
                        return responseMessage(HttpStatusCode.BadRequest, new { message = "Invalid params" });
                    }
                    User User = db.Users.FirstOrDefault(u => u.UserId == UserId);
                    if (User == null)
                    {
                        return responseMessage(HttpStatusCode.NotFound, new { message = "Staff not found" });
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
            return responseMessage(HttpStatusCode.NotFound, new
            {
                message = "Project not found"
            });
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
                return responseMessage(HttpStatusCode.NotFound, new { 
                    message = "Project not found"
                });
            }

            db.Projects.Remove(project);
            db.SaveChanges();

            return Ok(project);
        }



        private ResponseMessageResult responseMessage(HttpStatusCode statusCode, object message)
        {
            string responseMessage = JsonConvert.SerializeObject(message);
            ResponseMessageResult response = new ResponseMessageResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseMessage, Encoding.UTF8, "application/json")
            });
            return response;
        }
    }
}