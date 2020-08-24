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
    public class MovieController : ControllerBase
    {
        protected MovieService _service;
        public MovieController(MovieService movieService)
        {
            _service = movieService;
        }
        [HttpGet("List")]
        public async Task<ApiResult> List(int pageIndex = 1, int pageSize =20)
        {
            var result = await _service.GetList(pageIndex, pageSize);
            Response.StatusCode= 200;
            return result;
        }
        [HttpGet("Detail")]
        public async Task<ApiResult> Detail(int id)
        {
            var result = await _service.DetailAsync(id);
            Response.StatusCode= 200;
            return result;
        }

    }
}