using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;
using LogTimeSheet.Utils;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;
using System.Text;

namespace LogTimeSheet.Config
{
    public class AuthorizeRequestAttribute : AuthorizeAttribute
    {
        private Jwt jwtValidator = new Jwt();
        private readonly string[] roles;
        private string message;
        private bool authorize;
        private HttpStatusCode statusCode;

        public AuthorizeRequestAttribute(params string[] roles)
        {
            this.roles = roles;
        }

        private void SetHttpResponseContent(HttpStatusCode status, int code, string msg, HttpActionContext actionContext)
        {
            statusCode = status;
            message = "{\"code\":" + code + ", \"message\":\"" + msg + "\"}";
            HandleUnauthorizedRequest(actionContext);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            authorize = false;
            try
            {
                if (actionContext.Request.Headers.Authorization == null)
                {
                    SetHttpResponseContent(HttpStatusCode.Unauthorized, 0, "Your indentity is not authorized", actionContext);
                    return;
                }
                if (actionContext.Request.Headers.Authorization.Scheme.Contains("Bearer"))
                {
                    var token = actionContext.Request.Headers.Authorization.Parameter;
                    dynamic user = jwtValidator.ValidateToken(token);
                    if (user != null)
                    {
                        foreach (var role in roles)
                        {
                            if (role.Equals("Admin"))
                            {
                                if (Convert.ToUInt32(user.role) == 0)
                                {
                                    authorize = true;
                                    return;
                                }
                            }
                            else if (role.Equals("PM"))
                            {
                                if (Convert.ToUInt32(user.role) == 1)
                                {
                                    authorize = true;
                                    return;
                                }
                            }
                            else if (role.Equals("Staff"))
                            {
                                if (Convert.ToUInt32(user.role) == 2)
                                {
                                    authorize = true;
                                    return;
                                }
                            }
                            else if (role.Equals("All"))
                            {
                                authorize = true;
                                return;
                            }
                        }
                        SetHttpResponseContent(HttpStatusCode.Forbidden, 1, "You dont have permission to access this resource", actionContext);
                        return;
                    }
                }
                SetHttpResponseContent(HttpStatusCode.Unauthorized, 0, "Your indentity is not authorized", actionContext);
            }
            catch (Exception ex)
            {
                SetHttpResponseContent(HttpStatusCode.InternalServerError, -1, ex.ToString(), actionContext);
            }
        }
        protected override Boolean IsAuthorized(HttpActionContext actionContext)
        {
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(message, Encoding.UTF8, "application/json")
            };
        }
    }
}