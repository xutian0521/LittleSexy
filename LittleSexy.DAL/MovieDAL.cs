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
        public async Task<IEnumerable<t_Movie>> GetMovieListAsync(int pageIndex, int pageSize)
        {
            int skip = pageSize *(pageIndex -1);
            int take= pageSize;
            var list = await DB.Conn().QueryAsync<t_Movie>("SELECT Title,FanHao,Source,Cover,CreationTime FROM t_Movie LIMIT @skip,@take;",new {skip,take});
            return list;
        }
    }

}
