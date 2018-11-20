using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LittleSexy.Common;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.Service;

namespace LittleSexy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected UserService _userService;
        public UserController(IServiceProvider service)
        {
            _userService = service.GetService<UserService>();
        }
        [HttpGet]
        public async Task<ApiResult> ListAsync()
        {
            var list= await _userService.GetUserListAsync();
            ApiResult result = new ApiResult();
            result.Code = 200;
            result.Message = "成功";
            result.Content = list;
            return result;
        }
        [HttpPost]
        public async Task<ApiResult> Add()
        {
            var list= await _userService.GetUserListAsync();
            ApiResult result = new ApiResult();
            result.Code = 200;
            result.Message = "成功";
            result.Content = list;
            return result;
        }

    }
}