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
            int row = await DB.Conn().ExecuteAsync("INSERT INTO t_Movie(Title,FanHao,Source,Cover,Details,CreationTime) VALUES(@Title,@FanHao,@Source,@Cover,@Details,@CreationTime);",movies);
            return row;
        }
        public async Task<int> UpdateInsertMovieListAsync(List<t_Movie> movies)
        {
            int rows = 0;
            foreach (var item in movies)
            {
                t_Movie movie = await DB.Conn().QueryFirstOrDefaultAsync<t_Movie>("SELECT Title,FanHao,Source,Cover,CreationTime FROM t_Movie WHERE FanHao=@FanHao",new { item.FanHao});
                if (movie == null)
                {
                    rows += await DB.Conn().ExecuteAsync("INSERT INTO t_Movie(Title,FanHao,Source,Cover,Details,CreationTime) VALUES(@Title,@FanHao,@Source,@Cover,@Details,@CreationTime);",item);
                }
                else
                {
                     rows +=await DB.Conn().ExecuteAsync("UPDATE t_Movie SET Title=@Title,Source=@Source,Cover=@Cover,Details=@Details,CreationTime=@CreationTime WHERE FanHao=@FanHao;",item);
                }
            }
            return rows;
        }
        public async Task<IEnumerable<t_Movie>> GetMovieListAsync(int pageIndex, int pageSize)
        {
            int skip = pageSize *(pageIndex -1);
            int take= pageSize;
            var list = await DB.Conn().QueryAsync<t_Movie>("SELECT Id,Title,FanHao,Source,Cover,CreationTime FROM t_Movie LIMIT @skip,@take;",new {skip,take});
            return list;
        }
        public async Task<t_Movie> GetMovieOneAsync(long id)
        {
            var movie = await DB.Conn().QueryFirstOrDefaultAsync<t_Movie>("SELECT Id,Title,FanHao,Source,Cover,CreationTime FROM t_Movie WHERE Id=@id;",new { id});
            return movie;
        }
        public async Task<IEnumerable<t_Movie>> GetMongoMovieListAsync(int pageIndex, int pageSize)
        {
            int skip = pageSize *(pageIndex -1);
            int take= pageSize;
            var list = await DB.Conn().QueryAsync<t_Movie>("SELECT Title,FanHao,Source,Cover,CreationTime FROM t_Movie LIMIT @skip,@take;",new {skip,take});
            return list;
        }
    }

}
