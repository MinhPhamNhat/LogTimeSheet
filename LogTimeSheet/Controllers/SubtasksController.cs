using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
    public class SubtasksController : ApiController
    {
        private SystemContext db = new SystemContext();
        private SubtaskDAO subtaskDAO;
        private Jwt jwtValidator = new Jwt();


        // GET: api/Subtasks -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả subtask
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Subtasks")]
        public List<Subtask> GetAll()
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            return subtaskDAO.getList(user);
        }

        // GET: api/getByProject/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả Subtask theo ProjectId
        /// - Params: 
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Subtasks/getByProject/{ProjectId}")]
        public List<Subtask> GetByProject(int ProjectId)
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            return subtaskDAO.getSubtaskByProject(user, ProjectId);
        }

        // GET: api/Subtasks/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy Subtask theo SubtaskId
        /// - Params: 
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Subtasks/{SubtaskId}")]
        public IHttpActionResult GetSubtask(int SubtaskId)
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            Subtask subtask = subtaskDAO.getSubtask(user, SubtaskId);
            if (subtask == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Subtask not found" });
            }
            return Ok(subtask);
        }

        // PUT: api/Subtasks/5 -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Sửa Subtask
        /// - Params: 
        /// Optional{ Name:String, ProjectId:Int }
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpPut]
        [Route("api/Subtasks/{SubtaskId}")]
        public IHttpActionResult PutSubtask(int SubtaskId, [FromBody] dynamic subtask)
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            int Role = Convert.ToInt32(user.role);
            Subtask Subtask = subtaskDAO.getSubtask(user, SubtaskId);
            if (Subtask == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Subtask not found" });
            }
            if (Subtask.Project.Type && Role!=0)
            {
                return responseMessage(HttpStatusCode.Forbidden, new { message = "You cannot modify this project (Default Project)!!!" });
            }
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(subtask.Name)))
                {
                    Subtask.Name = Convert.ToString(subtask.Name);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(subtask.ProjectId)))
                {
                    int ProjectId = Convert.ToInt32(subtask.ProjectId);
                    Project Project = db.Projects.FirstOrDefault(p => p.ProjectId == ProjectId);
                    if (Project == null)
                    {
                        return responseMessage(HttpStatusCode.NotFound, new { message = "Project not found" });
                    }
                    Subtask.Project = Project;
                }
                db.SaveChanges();
                return Ok(Subtask);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }

        }

        // POST: api/Subtasks -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Thêm Subtask
        /// - Params: 
        /// Required{ Name:String, ProjectId:Int }
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpPost]
        [Route("api/Subtasks")]
        public IHttpActionResult PostSubtask([FromBody] dynamic subtask)
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);

            if (string.IsNullOrEmpty(Convert.ToString(subtask.Name)) ||
                string.IsNullOrEmpty(Convert.ToString(subtask.ProjectId)))
            {
                return responseMessage(HttpStatusCode.BadRequest, new { message = "Invalid params" });
            }

            string Name = Convert.ToString(subtask.Name);
            try
            {
                //  "1/1/2010 12:10:15 PM"
                int ProjectId = Convert.ToInt32(subtask.ProjectId);

                Project Project = db.Projects.FirstOrDefault(p => p.ProjectId == ProjectId && (p.ProjectUsers.Contains(p.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == ProjectId)) || p.Type));
                if (Project == null)
                {
                    return responseMessage(HttpStatusCode.NotFound, new { message = "Project not found" });
                }
                if (Project.Type && Role != 0)
                {
                    return responseMessage(HttpStatusCode.Forbidden, new { message = "You cannot modify this project (Default Project)!!!" });
                }
                Subtask s = subtaskDAO.addSubtask(new Subtask()
                {
                    Name = Name,
                    Project = Project
                });
                return Content(HttpStatusCode.Created, s);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }
        }

        // DELETE: api/Subtasks/5 -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Xoá Subtask theo SubtaskId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpDelete]
        [Route("api/Subtasks/{SubtaskId}")]
        public IHttpActionResult DeleteSubtask(int SubtaskId)
        {
            subtaskDAO = new SubtaskDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            Subtask subtask = subtaskDAO.deleteSubtask(user, SubtaskId);
            if (subtask == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Subtask not found" });
            }
            return Ok(subtask);
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