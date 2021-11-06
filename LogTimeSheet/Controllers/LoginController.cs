using LogTimeSheet.Config;
using LogTimeSheet.Models;
using LogTimeSheet.Repo;
using LogTimeSheet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogTimeSheet.Controllers
{
    public class LoginController : ApiController
    {
        Jwt jwtValidator = new Jwt();
        private SystemContext db = new SystemContext();
        private Login login;
        // POST: api/Login
        /// <summary>
        /// - Desc: Đăng nhập
        /// - Params: Required{ username:String, password:String }
        /// </summary>
        [HttpPost]
        [Route("api/Login")]
        public IHttpActionResult Login([FromBody] dynamic user)
        {
            login = new Login(db);
            string username = Convert.ToString(user.username);
            string password = Convert.ToString(user.password);
            if (!(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
            {
                try
                {
                    User authenticatedUser = login.checkUser(username, password);

                    if (authenticatedUser != null)
                    {
                        string token = jwtValidator.GenerateToken(authenticatedUser);
                        return Ok(new { token });
                    }
                    else
                    {
                        return Ok(new
                        {
                            message = "Unidentified username or password"
                        });
                    }
                }
                catch(Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
            return BadRequest();
        }
    }
}