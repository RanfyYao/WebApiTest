using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebApiTest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        //一般前端框架angular、vue都会先option一下。
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var res = HttpContext.Current.Response;
            var req = HttpContext.Current.Request;

            //自定义header时进行处理
            if (req.HttpMethod.ToUpper() == "OPTIONS")
            {
                res.AppendHeader("Access-Control-Allow-Headers", "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name,Token,Cookie");
                res.AppendHeader("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
                res.StatusCode = 200;
                res.End();
            }
        }
    }
}
