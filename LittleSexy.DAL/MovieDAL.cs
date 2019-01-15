using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using LittleSexy.Model.ViewModel;

namespace LittleSexy.DAL
{
    [Inject]
    public class MovieDAL
    {
        public async Task<int> InsertMovieListAsync(List<t_Movie> movies)
        {
            int row = await DB.Conn().ExecuteAsync("INSERT INTO t_Movie(Title,FanHao,Source,Cover,CreationTime) VALUES(@Title,@FanHao,@Source,@Cover,@CreationTime);",movies);
            return row;
        }
    }

}
