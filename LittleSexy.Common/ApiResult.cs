using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Common
{
    public class ApiResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }
}
