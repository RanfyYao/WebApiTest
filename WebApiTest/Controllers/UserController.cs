using System;
using System.Configuration;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApiTest.Common;
using WebApiTest.Models;

namespace WebApiTest.Controllers
{
    public class UserController : BaseController
    {
        #region 用户登录接口
        [ActionName("login")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody]User_login userlogin)
        {
            if (userlogin.account == "test" && userlogin.pwd == "123456")
            {

                //根据账号和密码去数据库取值来验证密码是否正确
                users umodel = new users() { userid = 12, username = "webapitest" };

                //1. 生成token值
                string restoken = System.Guid.NewGuid().ToString().Replace("-", "");

                //2.Token更新到数据库
                //TODO

                //3.存入缓存
                CacheHelper.Remove(restoken); //添加缓存之前先要remove一下，确保之前的缓存去掉。
                CacheHelper.Insert(restoken, umodel, timeout_token);

                //4. 记录日志

                return DoEnd(0, "登录成功", "返回所需信息的实体");
            }
            else
            {
                return DoEnd(0, "失败", "返回所需信息的实体");
            }

        }
        #endregion
        #region 获取验证码图片
        [ActionName("GetRandomPic")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage GetRandomPic([FromUri] GetRandomPic_Request request)
        {
            try
            {
                int width = 132;
                int height = 35;
                int fontsize = 20;

                if (request.w != 0)
                {
                    width = request.w;
                }

                if (request.h != 0)
                {
                    height = request.h;
                }

                if (request.fz != 0)
                {
                    fontsize = request.fz;
                }

                PicValidCodeHelper pic = new PicValidCodeHelper();
                string checkcode = pic.CreateRandomCode(4);
                string codekey = System.Guid.NewGuid().ToString().Replace("-", "");
                string str64 = pic.GetImgWithValidateCode(checkcode, width, height, fontsize);
                GetRandomPic_Response ret = new GetRandomPic_Response { key = codekey, imgbase64 = str64 };

                //存入缓存
                CacheHelper.Insert(codekey, checkcode, PicCodeValidTimeout); //图片验证码过期时间

                return DoEnd(0, "获取成功", ret);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region 检验验证码
        [ActionName("checkrandomcode")]
        [HttpPost]
        public HttpResponseMessage CheckRandomCode([FromBody] CheckRandomCode_Request request)
        {
            try
            {
                object checkcode_cache = CacheHelper.Get(request.key);
                if (checkcode_cache == null) return DoEnd(2, "验证码已过期");
                if (checkcode_cache.ToString() != request.code)
                {
                    CacheHelper.Remove(request.key);
                    return DoEnd(1, "验证码不正确");
                }
                CacheHelper.Remove(request.key);
                return DoEnd(0, "验证成功");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion 
    }
}