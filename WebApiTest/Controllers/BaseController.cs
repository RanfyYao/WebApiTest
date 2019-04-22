using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebApiTest.ApiAuth;
using WebApiTest.Models;

namespace WebApiTest.Controllers
{
    [AuthFilter]
    public class BaseController : ApiController
    {
        protected static int timeout_token = Convert.ToInt32(ConfigurationManager.AppSettings["tokentimeout"]);
        protected static int PicCodeValidTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["PicCodeValidTimeout"]);
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