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
            var token = Request.Headers.Authorization.Parameter;
            dynamic user = jwtValidator.ValidateToken(token);
            statisticDAO = new StatisticDAO(db);
            return Ok(new { 
                countUser = statisticDAO.numberOfUser(),
                countStaff = statisticDAO.numberOfStaffs(),
                countPM = statisticDAO.numberOfPMs(),
                countProject = statisticDAO.numberOfProjects(user),
                countLog = statisticDAO.numberOfLogs(user),
                currentLogs = parseLogOfWeek(statisticDAO.currWeekLogs(user))
            });
        }

        private object parseLogOfWeek(List<Log> Logs)
        {
            var totalLog = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            var totalTime = new double[] { 0, 0, 0, 0, 0, 0, 0 };

            Logs.ForEach(log =>
            {
                if (log.InitTime.DayOfWeek == DayOfWeek.Monday)
                {
                    totalLog[0] += 1;
                    totalTime[0] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Tuesday)
                {
                    totalLog[1] += 1;
                    totalTime[1] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Wednesday)
                {
                    totalLog[2] += 1;
                    totalTime[2] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Thursday)
                {
                    totalLog[3] += 1;
                    totalTime[3] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Friday)
                {
                    totalLog[4] += 1;
                    totalTime[4] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Saturday)
                {
                    totalLog[5] += 1;
                    totalTime[5] += log.Overtime;
                }
                else if (log.InitTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    totalLog[6] += 1;
                    totalTime[6] += log.Overtime;
                }
            });

            return new
            {
                totalLog = totalLog,
                totalTime = totalTime
            };
        }
    }
}
