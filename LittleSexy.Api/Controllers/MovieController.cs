using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LittleSexy.Api.Models;
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
        [HttpPut]
        public async Task<IActionResult> UpdateAllMovies()
        {
            var result = await _service.UpdateAllMovies();
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }

        }
        [HttpGet("List")]
        public async Task<List<v_Movie>> MovieList
            (string sort = "CreateTime", string actressName =null, int? isLiked = null, int pageIndex = 1, int pageSize =20 )
        {
            var result = await _service.GetList( sort, actressName, isLiked, pageIndex, pageSize);
            return result;
        }
        //[HttpGet("LikedMovies")]
        //public async Task<List<v_Movie>> LikedMovies(string sort = "CreateTime", int pageIndex = 1, int pageSize = 20)
        //{
        //    var result = await _service.GetList(sort, null, 1, pageIndex, pageSize);
        //    return result;
        //}
        [HttpGet("Detail")]
        public async Task<v_Movie> MovieDetail(int id)
        {
            var result = await _service.DetailAsync(id);
            return result;
        }

        [HttpGet("Actresses")]
        public async Task<List<v_Actress>> Actresses(string sort = "CreateTime", int? isLiked = null)
        {
            var result = await _service.Actresses(sort, isLiked);
            return result;
        }
        //[HttpGet("LikedActresses")]
        //public async Task<List<v_Actress>> LikedActresses(string sort = "CreateTime")
        //{
        //    var result = await _service.Actresses(sort, 1);
        //    return result;
        //}

        [HttpGet("ActressDetail")]
        public async Task<v_Actress> ActressDetail(string actressName)
        {
            var result = await _service.ActressDetails(actressName);
            return result;
        }
        
        //[HttpGet("ActressMovies")]
        //public async Task<List<v_Movie>> ActressMovies(string actressName, string sort = "CreateTime", int pageIndex = 1, int pageSize = 20)
        //{
        //    var result = await _service.GetList(sort, actressName, null, pageIndex, pageSize);
        //    return result;
        //}
        [HttpPatch("LikingMovie")]
        public async Task<IActionResult> LikingMovie(int id, int isLiked)
        {
            var result = await _service.LikingMovie(id, isLiked);
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPatch("LikingActress")]
        public async Task<IActionResult> LikingActress(string actressName, int isLiked)
        {
            var result = await _service.LikingActress(actressName, isLiked);
            if (result)
            {
                return Accepted();
            }
            else
            {
                return NotFound();
            }
        }

    }
}