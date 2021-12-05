using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
    public class LogsController : ApiController
    {
        private SystemContext db = new SystemContext();
        private LogDAO logDAO;
        private Jwt jwtValidator = new Jwt();

        // GET: api/Logs
        /// <summary>
        /// - Role: Admin
        /// - Desc: Lấy tất cả Log
        /// </summary>
        [AuthorizeRequest("Admin")]
        [HttpGet]
        [Route("api/Logs")]
        public List<Log> GetAll()
        {
            logDAO = new LogDAO(db);
            return logDAO.getAll();
        }

        // GET: api/Logs/AllByUser/5
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả log của người yêu cầu
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Logs/myLog")]
        public List<Log> GetByUser()
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            return logDAO.getLogsByUser(UserId);
        }

        // GET: api/Logs/myLogByProject/5
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả log của người yêu cầu theo ProjectId 
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Logs/myLogByProject/{ProjectId}")]
        public List<Log> GetByProject(int ProjectId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            return logDAO.getUserLogsByProject(UserId, ProjectId);
        }

        // GET: api/Logs/myLogBySubtask/5
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy tất cả log của người yêu cầu theo SubtaskId 
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Logs/myLogBySubtask/{SubtaskId}")]
        public List<Log> GetBySubtask(int SubtaskId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            return logDAO.getUserLogsBySubtask(UserId, SubtaskId);
        }

        // GET: api/Logs/AllByUser/5
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Lấy tất cả log của một UserId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpGet]
        [Route("api/Logs/AllByUser/{UserId}")]
        public List<Log> GetByUser(string UserId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            return logDAO.getLogsByUser(user, UserId);
        }

        // GET: api/Logs/byProject/5
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Lấy tất cả log của một UserId theo ProjectId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpGet]
        [Route("api/Logs/AllByProject/{ProjectId}")]
        public List<Log> GetAllByProject(int ProjectId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            return logDAO.getAllLogsByProject(user, ProjectId);
        }

        // GET: api/Logs/getListBySubtask/5
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Lấy tất cả log của một UserId theo SubtaskId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpGet]
        [Route("api/Logs/AllBySubtask/{SubtaskId}")]
        public List<Log> GetAllBySubtask(int SubtaskId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            return logDAO.getAllLogsBySubtask(user, SubtaskId);
        }


        // GET: api/Logs/5
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Lấy Log theo LogId
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Logs/{LogId}")]
        public IHttpActionResult GetLog(int LogId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);

            Log log = logDAO.getLog(user, LogId);
            if (log == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Log not found" });
            }

            return Ok(log);
        }

        // PUT: api/Logs/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Sửa Log theo LogId
        /// - Parmas: Optional{ Note:String, StdTime:Double, Overtime:Double, SubtaskId:Int, DateLog:DateTime(Format: 1/1/2010 12:10:15 PM) }
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpPut]
        [Route("api/Logs/{LogId}")]
        public IHttpActionResult PutLog(int LogId, [FromBody] dynamic log)
        {
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);

            logDAO = new LogDAO(db);

            Log Log = logDAO.getLog(user, LogId);
            if (Log == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Log not found" });
            }
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(log.Note)))
                {
                    Log.Note = Convert.ToString(log.Note);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(log.Stdtime)))
                {
                    Log.Stdtime = Convert.ToDouble(log.Stdtime);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(log.Overtime)))
                {
                    Log.Overtime = Convert.ToDouble(log.Overtime);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(log.SubtaskId)))
                {
                    int SubtaskId = Convert.ToInt32(log.SubtaskId);
                    Subtask Subtask = db.Subtasks.FirstOrDefault(s => s.SubtaskId == SubtaskId && (s.Project.ProjectUsers.Contains(
                    s.Project.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == s.Project.ProjectId)) || s.Project.Type));
                    if (Subtask == null)
                    {
                        return responseMessage(HttpStatusCode.NotFound, new { message = "Subtask not found" });
                    }
                    Log.Subtask = Subtask;
                }
                if (!string.IsNullOrEmpty(Convert.ToString(log.DateLog)))
                {
                    Log.DateLog = Convert.ToDateTime(log.DateLog);
                }
                db.SaveChanges();
                return Ok(Log);

            }catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }
        }

        // PUT: api/Logs/approve/5 -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Duyệt Log theo LogId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpPut]
        [Route("api/Logs/approve/{LogId}")]
        public IHttpActionResult ApproveLog(int LogId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);

            string UserId = Convert.ToString(user.id);
            User User = db.Users.FirstOrDefault(u => u.UserId == UserId);

            Log log = logDAO.getLog(user, LogId);
            if (log == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Log not found" });
            }
            try
            {
                log.Subtask = log.Subtask;
                log.User = log.User;
                log.IsApproved = true;
                log.UserApproved = User;
                log.DateApproved = DateTime.Now;
                db.SaveChanges();
                return Ok(log);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }

        }

        // PUT: api/Logs/disapprove/5 -- PASS
        /// <summary>
        /// - Role: Admin, PM
        /// - Desc: Huỷ duyệt Log theo LogId
        /// </summary>
        [AuthorizeRequest("Admin", "PM")]
        [HttpPut]
        [Route("api/Logs/disapprove/{LogId}")]
        public IHttpActionResult DisapproveLog(int LogId)
        {
            logDAO = new LogDAO(db);
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);

            string UserId = Convert.ToString(user.id);
            User User = db.Users.FirstOrDefault(u => u.UserId == UserId);

            Log log = logDAO.getLog(user, LogId);
            if (log == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Log not found" });
            }
            try
            {
                log.Subtask = log.Subtask;
                log.User = log.User;
                log.IsApproved = false;
                log.UserApproved = null;
                log.DateApproved = null;
                db.SaveChanges();
                return Ok(log);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }

        }

        // POST: api/Logs -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Thêm Log
        /// - Parmas: Required{ Note:String, StdTime:Double, Overtime:Double, SubtaskId:Int, DateLog:DateTime(Format: 1/1/2010 12:10:15 PM) }
        /// </summary>
        [AuthorizeRequest("All")]
        [HttpPost]
        [Route("api/Logs")]
        public IHttpActionResult PostLog(dynamic log)
        {
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            string UserId = Convert.ToString(user.id);
            int Role = Convert.ToInt32(user.role);

            logDAO = new LogDAO(db);
            if (string.IsNullOrEmpty(Convert.ToString(log.Note)) ||
                string.IsNullOrEmpty(Convert.ToString(log.Stdtime)) ||
                string.IsNullOrEmpty(Convert.ToString(log.Overtime)) ||
                string.IsNullOrEmpty(Convert.ToString(log.SubtaskId)) ||
                string.IsNullOrEmpty(Convert.ToString(log.DateLog)))
            {
                return responseMessage(HttpStatusCode.BadRequest, new { message = "Invalid params" });
            }

            string Note = Convert.ToString(log.Note);
            try
            {
                Double Stdtime = Convert.ToDouble(log.Stdtime);
                Double Overtime = Convert.ToDouble(log.Overtime);
                //  "1/1/2010 12:10:15 PM"
                DateTime DateLog = Convert.ToDateTime(log.DateLog);
                int SubtaskId = Convert.ToInt32(log.SubtaskId);

                Subtask Subtask = db.Subtasks.FirstOrDefault(s => s.SubtaskId == SubtaskId && (s.Project.ProjectUsers.Contains(
                    s.Project.ProjectUsers.FirstOrDefault(_ => _.UserId == UserId && _.ProjectId == s.Project.ProjectId)) || s.Project.Type));
                if (Subtask == null)
                {
                    return responseMessage(HttpStatusCode.NotFound, new { message = "Subtask not found" });
                }
                User User = db.Users.FirstOrDefault(u => u.UserId == UserId);
                Log l = logDAO.addLog(new Log()
                {
                    Note = Note,
                    InitTime = DateTime.Now,
                    Stdtime = Stdtime,
                    Overtime = Overtime,
                    DateLog = DateLog,
                    Subtask = Subtask,
                    User = User
                });
                return Content(HttpStatusCode.Created, l);
            }
            catch (Exception ex)
            {
                return responseMessage(HttpStatusCode.InternalServerError, new { message = ex.ToString() });
            }
        }

        // DELETE: api/Logs/5 -- PASS
        /// <summary>
        /// - Role: Tất cả
        /// - Desc: Xoá LogId
        /// </summary>
        [AuthorizeRequest("All")]
        [Route("api/Logs/{LogsId}")]
        [HttpDelete]
        public IHttpActionResult DeleteLog(int LogsId)
        {
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            logDAO = new LogDAO(db);
            Log log = logDAO.deleteLog(user, LogsId);
            if (log == null)
            {
                return responseMessage(HttpStatusCode.NotFound, new { message = "Log not found" });
            }

            return Ok(log);
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