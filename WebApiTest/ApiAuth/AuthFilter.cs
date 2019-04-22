using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiTest.Models;

namespace WebApiTest.ApiAuth
{
    public class AuthFilter : ActionFilterAttribute
    {
        //执行action之后
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            string req = string.Empty; //访问参数

            var request = actionExecutedContext.Request;

            if (!request.Content.IsMimeMultipartContent())
            {
                //获取post访问参数
                if (actionExecutedContext.Request.Method.ToString() == "POST")
                {
                    ((HttpContextBase)actionExecutedContext.Request.Properties["MS_HttpContext"]).Request.InputStream.Position = 0;//核心代码
                    byte[] byts = new byte[((HttpContextBase)actionExecutedContext.Request.Properties["MS_HttpContext"]).Request.InputStream.Length];
                    ((HttpContextBase)actionExecutedContext.Request.Properties["MS_HttpContext"]).Request.InputStream.Read(byts, 0, byts.Length);
                    req = System.Text.Encoding.Default.GetString(byts);
                }
                else
                {
                    req = actionExecutedContext.Request.RequestUri.Query.TrimStart('?');
                }
            }
            else
            {
                req = "文件上传";
            }

            //拿token值
            var authHeader = from h in actionExecutedContext.Request.Headers where h.Key == "token" select h.Value.FirstOrDefault();
            var token = authHeader.FirstOrDefault();

            //记录日志 所有调用API的接口都记录下来
            //Model.jy_api_log modellog = new Model.jy_api_log();
            //BLL.jy_api_log blllog = new BLL.jy_api_log();
            //modellog.token = token == null ? "" : token.ToString();
            //modellog.parameter = req;
            //modellog.host = actionExecutedContext.Request.RequestUri.Authority;
            //modellog.OriginalString = actionExecutedContext.Request.RequestUri.PathAndQuery;
            //modellog.method = actionExecutedContext.Request.Method.ToString();
            //modellog.Referrer = actionExecutedContext.Request.Headers.Referrer == null ? "" : actionExecutedContext.Request.Headers.Referrer.ToString();
            //modellog.response = GetResponseValues(actionExecutedContext);
            //modellog.log_time = DateTime.Now;
            //modellog.ipaddr = Utils.GetHostAddress();
            //blllog.Add(modellog);

        }

        /// <summary>
        /// 读取request 的提交内容
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public string GetRequestValues(HttpActionExecutedContext actionExecutedContext)
        {

            Stream stream = actionExecutedContext.Request.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            /*
                这个StreamReader不能关闭，也不能dispose， 关了就傻逼了
                因为你关掉后，后面的管道  或拦截器就没办法读取了
            */
            var reader = new StreamReader(stream, encoding);
            string result = reader.ReadToEnd();
            /*
            这里也要注意：   stream.Position = 0;
            当你读取完之后必须把stream的位置设为开始
            因为request和response读取完以后Position到最后一个位置，交给下一个方法处理的时候就会读不到内容了。
            */
            stream.Position = 0;
            return result;
        }

        /// <summary>
        /// 读取action返回的result
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public string GetResponseValues(HttpActionExecutedContext actionExecutedContext)
        {
            Stream stream = actionExecutedContext.Response.Content.ReadAsStreamAsync().Result;
            Encoding encoding = Encoding.UTF8;
            /*
            这个StreamReader不能关闭，也不能dispose， 关了就傻逼了
            因为你关掉后，后面的管道  或拦截器就没办法读取了
            */
            var reader = new StreamReader(stream, encoding);
            string result = reader.ReadToEnd();
            /*
            这里也要注意：   stream.Position = 0; 
            当你读取完之后必须把stream的位置设为开始
            因为request和response读取完以后Position到最后一个位置，交给下一个方法处理的时候就会读不到内容了。
            */
            stream.Position = 0;
            return result;
        }

        //执行action之前
        public override void OnActionExecuting(HttpActionContext actionContext)
        {


            if (!actionContext.ModelState.IsValid)
            {
                var item = actionContext.ModelState.Values.Take(1).SingleOrDefault();
                actionContext.Response = DoEnd(-888, item.Errors.Where(b => !string.IsNullOrWhiteSpace(b.ErrorMessage)).Take(1).SingleOrDefault().ErrorMessage);
            }
            base.OnActionExecuting(actionContext);

            //以下是为了重写返回status code
            int code = actionContext.RequestContext.RouteData.Values["code"] == null ? 0 : Convert.ToInt32(actionContext.RequestContext.RouteData.Values["code"].ToString());

            if (code != 1 && code != 0)  //等于1是过滤掉正常登陆  等于0是其他方法的过滤
            {
                string retMsg = string.Empty;
                switch (code)
                {
                    case -100: retMsg = "密码已过期，请重新登录"; break;
                    case -101: retMsg = actionContext.RequestContext.RouteData.Values["msg"].ToString(); break; //验证token时报错
                    case -102: retMsg = "http的header里缺少token"; break;
                    case -103: retMsg = "token不能为空"; break;
                    case -999: retMsg = "未知错误 -999"; break;
                }
                actionContext.Response = DoEnd(code, retMsg);
            }
        }

        public HttpResponseMessage DoEnd(int code, string msg)
        {
            return DoEnd(code, msg, null);
        }
        public HttpResponseMessage DoEnd(int code, string msg, object data)
        {

            RetJson ret = new RetJson();
            ret.code = code;
            ret.msg = msg;
            ret.data = data == null ? new List<string>() : data;

            string json = JsonConvert.SerializeObject(ret);
            return new HttpResponseMessage { Content = new StringContent(json, Encoding.GetEncoding("UTF-8"), "application/json") };
        }
    }
}