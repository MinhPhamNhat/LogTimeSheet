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
using System.Web.Http.Cors;

namespace LogTimeSheet.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class StatisticController : ApiController
    {
        private SystemContext db = new SystemContext();
        private StatisticDAO statisticDAO;
        private Jwt jwtValidator = new Jwt();

        // GET: api/Statistic
        /// <summary>
        /// - Role: Admin
        /// - Desc: Lấy total projects, total PM, total employee, total logs, total users
        /// </summary>
        [HttpGet]
        [Route("api/Statistic")]
        [AuthorizeRequest("All")]
        public IHttpActionResult GetAll()
        {
            statisticDAO = new StatisticDAO(db);
            return Ok(new { 
                countUser = statisticDAO.numberOfUser(),
                countStaff = statisticDAO.numberOfStaffs(),
                countPM = statisticDAO.numberOfPMs(),
                countProject = statisticDAO.numberOfProjects(),
                countLog = statisticDAO.numberOfLogs(),
            });
        }
    }
}
