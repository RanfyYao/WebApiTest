using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiTest.Models
{
    public class users
    {
        public int userid { get; set; }
        public string username { get; set; }
    }

    public class GetRandomPic_Request
    {
        public int w { get; set; }
        public int h { get; set; }
        public int fz { get; set; }

    }

    public class GetRandomPic_Response
    {
        public string key { get; set; }
        public string imgbase64 { get; set; }
    }

    public class CheckRandomCode_Request
    {
        [Required(ErrorMessage = "key值不能为空")]
        public string key { get; set; }
        [Required(ErrorMessage = "code不能为空")]
        public string code { get; set; }

    }
}