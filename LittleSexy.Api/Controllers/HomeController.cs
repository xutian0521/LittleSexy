using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleSexy.Api.Services;
using LittleSexy.Api.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LittleSexy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        protected HomeService _homeService;
        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }
        [HttpGet("Banner")]
        public async Task<ApiResult> Banner(int pageId)
        {
            var list = await _homeService.GetBnners();
            ApiResult result = new ApiResult();
            result.Code = 200;
            result.Message = "成功";
            result.Content = list;
            Response.StatusCode = 200;
            return result;
        }
    }
}
