using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTest.Common
{
    public class DTEnum
    {
        public enum TokenStatus
        {
            //账单状态
            TokenIsOK=1,
            TokenIsOvertime = -100,
            TokenCheckError = -101,
            HeaderNoToken=-102,
            TokenIsNull=-103
        }
    }
}