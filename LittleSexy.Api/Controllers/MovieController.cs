using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LittleSexy.Common;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.Service;
using LittleSexy.Service.Interface;

namespace LittleSexy.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        protected IMovieService _service;
        public MovieController(IMovieService movieService)
        {
            _service = movieService;
        }
        [HttpGet("List")]
        public async Task<ApiResult> List(int pageIndex = 1, int pageSize =20)
        {
            var result = await _service.GetMoviesListAsync(pageIndex, pageSize);
            Response.StatusCode= 200;
            return result;
        }
        [HttpPut]
        public async Task<ApiResult> Update()
        {
            var result = await _service.UpdateMoviesListAsync();
            Response.StatusCode= 200;
            return result;
        }

    }
}