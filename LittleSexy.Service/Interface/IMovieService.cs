using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.DAL;
using System.Threading.Tasks;
using LittleSexy.Model.ViewModel;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LittleSexy.Service.Interface
{
    [Inject]
    public interface IMovieService
    {
        /// <summary>
        /// 获取电影列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult> GetMoviesListAsync();
    }

}
