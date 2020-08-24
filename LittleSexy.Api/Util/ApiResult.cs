using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Api.Util
{
    public class ApiResult
    {
        public ApiResult()
        {
            this.Code = 200;
            this.Message = "成功";
        }
        public ApiResult(int code,string message)
        {
            this.Code = 200;
            this.Message = "成功";
        }
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }
}
