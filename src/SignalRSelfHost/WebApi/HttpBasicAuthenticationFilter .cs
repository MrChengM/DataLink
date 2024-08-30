using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http;
using System.Security.Principal;
using System.Threading;
using System.Net.Http.Headers;
using DataServer.Permission;
using Unity;

namespace SignalRSelfHost.WebApi
{
    public class BasicAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            string authParameter = null;

            var authValue = actionContext.Request.Headers.Authorization;
            if (authValue != null && authValue.Scheme == "Basic")
            {
                authParameter = authValue.Parameter;  //authparameter:获取请求中经过Base64编码的（用户：密码）
            }

            if (string.IsNullOrEmpty(authParameter))
            {
                Challenge(actionContext);
                return;
            }

            authParameter = Encoding.Default.GetString(Convert.FromBase64String(authParameter));

            var authToken = authParameter.Split(':');
            if (authToken.Length < 2)
            {
                Challenge(actionContext);
                return;
            }

            if (!ValidateUser(authToken[0], authToken[1]))
            {
                Challenge(actionContext);
                return;
            }

            var principal = new GenericPrincipal(new GenericIdentity(authToken[0]), null);
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }

            base.OnAuthorization(actionContext);
        }

        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "请求未授权，拒绝访问。");
            //actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));//可以使用如下语句
            actionContext.Response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", string.Format("realm=\"{0}\"", host)));
        }

        protected virtual bool ValidateUser(string userName, string password)
        {
            var _permissionManager = SignalRServer.Container.Resolve<IPermissionManager>();
            if (_permissionManager.ValidateUser(userName, password)) //判断用户名及密码，实际可从数据库查询验证,可重写
            {
                return true;
            }
            return false;
        }

    }
    public class AdminBasicAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            string authParameter = null;

            var authValue = actionContext.Request.Headers.Authorization;
            if (authValue != null && authValue.Scheme == "Basic")
            {
                authParameter = authValue.Parameter;  //authparameter:获取请求中经过Base64编码的（用户：密码）
            }

            if (string.IsNullOrEmpty(authParameter))
            {
                Challenge(actionContext);
                return;
            }

            authParameter = Encoding.Default.GetString(Convert.FromBase64String(authParameter));

            var authToken = authParameter.Split(':');
            if (authToken.Length < 2)
            {
                Challenge(actionContext);
                return;
            }

            if (!ValidateUser(authToken[0], authToken[1]))
            {
                Challenge(actionContext);
                return;
            }

            var principal = new GenericPrincipal(new GenericIdentity(authToken[0]), null);
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }

            base.OnAuthorization(actionContext);
        }

        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "请求未授权，请用管理员访问。");
            //actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));//可以使用如下语句
            actionContext.Response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", string.Format("realm=\"{0}\"", host)));
        }

        protected virtual bool ValidateUser(string userName, string password)
        {
            var _permissionManager = SignalRServer.Container.Resolve<IPermissionManager>();
            var user = _permissionManager.GetUser(userName, password);
            if (user != null) //判断用户名及密码，实际可从数据库查询验证,可重写
            {
                foreach (var role in user.Roles)
                {
                    if (role.Resources.Find(s => s.Name == ResourceType.Sys_Admin.ToString()) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}