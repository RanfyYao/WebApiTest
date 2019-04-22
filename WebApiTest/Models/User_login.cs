using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Models
{
    public class User_login
    {
        [Required(ErrorMessage = "缺少手机号码")]
        public string account { get; set; }
        [Required(ErrorMessage = "缺少密码")]
        public string pwd { get; set; }
    }
}