using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Common
{
    public class ApiResult
    {
        public ApiResult()
        {
            this.Code = 200;
            this.Message = "成功";
        }
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }
}
