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
        public async Task<ApiResult> Detail(long id)
        {
            var result = await _service.DetailAsync(id);
            Response.StatusCode= 200;
            return result;
        }

    }
}