using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using LogTimeSheet.Config;
using LogTimeSheet.Models;
using LogTimeSheet.Repo;
using LogTimeSheet.Utils;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace LogTimeSheet.Controllers
{
    public class UsersController : ApiController
    {
        Jwt jwtValidator = new Jwt();
        private SystemContext db = new SystemContext();
        private UserDAO userDAO;

        // GET: api/Users
        [AuthorizeRequest("Admin")]
        [HttpGet]
        [Route("api/Users/getAll")]
        public List<User> GetAll()
        {
            userDAO = new UserDAO(db);
            return userDAO.getList();
        }

        // POST: api/Users/getManager
        [AuthorizeRequest("Admin", "PM")]
        [HttpGet]
        [Route("api/Users/getManager")]
        public IHttpActionResult GetManager()
        {
            userDAO = new UserDAO(db);
            return Ok(userDAO.getManager());
        }

        // POST: api/Users/getStaff
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Users/getStaff")]
        public IHttpActionResult GetStaff()
        {
            userDAO = new UserDAO(db);
            return Ok(userDAO.getStaff());
        }

        // POST: api/Users/getUser/5
        [AuthorizeRequest("All")]
        [HttpGet]
        [Route("api/Users/getUser/{UserId}")]
        public IHttpActionResult GetUser(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return responseMessage(HttpStatusCode.BadRequest, new { message = "Invalid param" });
            }
            else
            {
                userDAO = new UserDAO(db);
                User user =userDAO.getUser(UserId);
                if (user != null)
                {
                    return Ok(user);
                }
                return responseMessage(HttpStatusCode.NotFound, new { message = "User not found" });
            }
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