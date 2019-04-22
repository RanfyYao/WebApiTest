using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebApiTest.Common;
using WebApiTest.Models;
using static WebApiTest.Common.DTEnum;

namespace WebApiTest.ApiAuth
{
    //用户接口进行token的验证
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected static int timeout_token = Convert.ToInt32(ConfigurationManager.AppSettings["tokentimeout"]);
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //前端请求api时会将token存放在请求头中
            var authHeader = from h in actionContext.Request.Headers where h.Key == "token" select h.Value.FirstOrDefault();

            if (authHeader != null)
            {
                string token = authHeader.FirstOrDefault();
                if (token == null)
                {
                    actionContext.RequestContext.RouteData.Values.Add("code", "-103"); //token为空值
                    return true;
                }

                try
                {
                    users userinfo = (users)CacheHelper.Get(token);

                    if (userinfo == null)
                    {
                        //判断出token已经过期了
                        PutTokenInfo(actionContext, "code", TokenStatus.TokenIsOvertime.ToString()); //token已经过期
                        return true;
                    }
                    else
                    {
                        //缓存重新刷新时间
                        CacheHelper.Remove(token); //添加缓存之前先要remove一下，确保之前的缓存去掉。
                        CacheHelper.Insert(token, userinfo, timeout_token); //设置7天过期 10080

                        //将用户信息存放起来，供后续调用
                        PutTokenInfo(actionContext, "code", TokenStatus.TokenIsOK.ToString());//正常执行
                        PutTokenInfo(actionContext, "userinfo", userinfo);//存放用户信息
                    }
                }
                catch (Exception ex)
                {
                    PutTokenInfo(actionContext, "code", TokenStatus.TokenCheckError.ToString());//http验证token时报错
                    PutTokenInfo(actionContext, "msg", ex.Message);//http验证token时报错
                    return true;
                }

            }
            else
            {
                PutTokenInfo(actionContext, "code", TokenStatus.HeaderNoToken.ToString());//http的header里缺少token
            }
            return true;
        }

        protected void PutTokenInfo(HttpActionContext actionContext,string key,object value)
        {
            actionContext.RequestContext.RouteData.Values.Add(key,value);
        }
    }
}