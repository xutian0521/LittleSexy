using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Data;
using LittleSexy.Api.Util;
using LittleSexy.Api.Models;

namespace LittleSexy.Api.Services
{

    public class MovieService
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        string movieRootPath =  @"E:\publish\wwwroot\ftp\";


        public IConfiguration _configuration { get; }
        public MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 递归获取电影文件
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="fileList"></param>
        private  void GetAllDir(string dir1,ref List<List<FileInfo>> fileList)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var files = dir.GetFiles();
            var dirs =dir.GetDirectories();
            
            List<FileInfo> ones=new List<FileInfo>();
            for (int i = 0; i < files.Length; i++)
            {
                var ext= Path.GetExtension( files[i].FullName);
                if(ext == ".jpg" || ext == ".png")
                {
                    ones.Add(files[i]);
                }
                if(ext ==".webm" || ext == ".mp4")
                {
                    ones.Add(files[i]);
                    fileList.Add(ones);
                }

            }
            for (int i = 0; i < dirs.Length; i++)
            {
                if(dirs.Length > 0)
                    GetAllDir(dirs[i].FullName, ref fileList);
            }
            
            for (int i = 0; i < ones.Count; i++)
            {
                Console.WriteLine(ones[i].FullName);
            }
        }
        private List<DirectoryInfo> GetActressDirs(string dir1)
        {
            DirectoryInfo dir = new DirectoryInfo(dir1);
            var dirs = dir.GetDirectories();
            return dirs.ToList();
        }
        private async Task<List<v_Movie>> GetListInternal(string actressName, int? isLiked)
        {

            //播放量字典
            Dictionary<string, (int count, int isLiked)> viewCountsDict = this.GetViewCountDict();
            string ApiHost = _configuration.GetValue<string>("ApiHost");
            string apiHostExt = ApiHost + @"/ftp/";

            List<v_Movie> LsMovies = new List<v_Movie>();

            var actressDirs = string.IsNullOrEmpty(actressName) ? this.GetActressDirs(movieRootPath): this.GetActressDirs(movieRootPath).Where(y =>y.Name.Contains(actressName));
            foreach (var actressItem in actressDirs) //每女优
            {
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = apiHostExt + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover))
                    {
                        actress.Cover = path;
                    }
                }
                var fanHaoDirs = actressItem.GetDirectories(); //所有番号 文件夹
                foreach (var fanHaoItem in fanHaoDirs)
                {
                    var fanHaoFiles = fanHaoItem.GetFiles();
                    string FanHao = fanHaoItem.Name.Replace("-C", "");
                    v_Movie movie = new v_Movie();
                    foreach (var item in fanHaoFiles)
                    {
                        var ext = item.Extension;
                        if(ext == ".mp4" || ext == ".webm") //视频
                        {
                            var Source = apiHostExt + item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");
                            movie.Sources.Add(Source);
                            if (!LsMovies.Contains(movie))
                            {
                                movie.Id = LsMovies.Count + 1;
                                
                                movie.FanHao = FanHao;

                                movie.Date = item.CreationTime.ToString("yyyy-MM-dd hh:mm");
                                movie.CreationTime = item.CreationTime;
                                movie.ViewCount = viewCountsDict.Any(x => x.Key == FanHao) ? viewCountsDict[FanHao].count : 0;
                                movie.IsLiked = viewCountsDict.Any(x => x.Key == FanHao) ? viewCountsDict[FanHao].isLiked : 0;
                                movie.Actress = actress;

                                LsMovies.Add(movie);
                            }
                        }
                        if (ext == ".jpg" || ext == ".png") //图片
                        {
                            string picRelativePath = item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");

                            if (item.Name.Contains(FanHao))
                            {
                                movie.Title = Path.GetFileNameWithoutExtension(item.Name);
                                movie.Cover = apiHostExt + picRelativePath;
                            }
                            else
                            {
                                movie.Preview.Add(apiHostExt + picRelativePath);
                            }
                        }
                    }
                }
            }
            //默认值
            foreach (var item in LsMovies)
            {
                if(item.Cover == null)
                {
                    continue;
                }
                if (!item.Cover.Contains(".jpg")) //视频不包含图片添加默认图片
                {
                    item.Cover = "../images/default.jpg";
                }
            }
            //是否喜欢条件
            LsMovies = isLiked == null? LsMovies : LsMovies.Where(x => x.IsLiked == isLiked.GetValueOrDefault()).ToList();
            _memoryCache.Set("movieTempList", LsMovies, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return LsMovies;
        }

        private Dictionary<string, (int count, int isLiked)> GetViewCountDict()
        {
            Dictionary<string, (int count, int isLiked)> viewCountsDict = new Dictionary<string, (int count, int isLiked)>();
            var dt = SQLiteHelper.Query("SELECT * FROM Dict;");
            if (dt.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    string fanHao = item["Datakey"].ToString();
                    int count = int.Parse(item["DataValue"].ToString());
                    int isLiked = int.Parse(item["IsLiked"].ToString());
                    
                    viewCountsDict.Add(fanHao, (count, isLiked));
                }
            }
            return viewCountsDict;
        }
        public async Task<bool> UpdateAllMovies()
        {
            var list = await this.GetListInternal(null, null);
            return list.Count > 0;

        }
        public async Task<List<v_Movie>> GetList(string sort, string actressName, int? isLiked, int pageIndex, int pageSize)
        {
            //List<v_Movie> list = new List<v_Movie>();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList" + actressName + isLiked);
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetListInternal(actressName, isLiked); });
                return _cache;
            }

            List<v_Movie> list = await this.GetListInternal(actressName, isLiked);

            //排序
            if (sort == SortType.CreateTime.ToString())
            {
                list = list.OrderByDescending(x => x.CreationTime).ToList();
            }
            else if(sort == SortType.ViewCount.ToString())
            {
                list = list.OrderByDescending(y => y.ViewCount).ToList();
            }
            return list;
        }


        public async Task<v_Movie> DetailAsync(int id)
        {
            v_Movie movie = new v_Movie();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            Dictionary<string, (int count, int isLiked)> viewCountsDict = this.GetViewCountDict();
            List<v_Movie> movieLists = new List<v_Movie>();

            if (_cache != null && _cache.Count > 0)
            {
                var detail = _cache.Where(x => x.Id == id).FirstOrDefault();
                int count = 0;
                var dt = SQLiteHelper.Query($"SELECT * FROM Dict  WHERE DataKey='{detail.FanHao}'");
                if (dt.Tables[0].Rows.Count > 0)
                {
                    var dr = dt.Tables[0].Rows[0];
                    count =int.Parse( dr["DataValue"].ToString());
                    int _id = int.Parse(dr["Id"].ToString());
                    int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET DataValue = '{ ++count}' WHERE Id = '{_id}'; ");
                }
                else
                {
                    int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,DataValue) VALUES ('{detail.FanHao}','1');");
                }
                detail.ViewCount = viewCountsDict.Any(x => x.Key == detail.FanHao) ? viewCountsDict[detail.FanHao].count : 0;
                detail.IsLiked = viewCountsDict.Any(x => x.Key == detail.FanHao) ? viewCountsDict[detail.FanHao].isLiked : 0;
                movie = detail;
            }
            return movie;
        }
        public async Task<List<v_Actress>> Actresses(string sort, int? isLiked)
        {
            var _cache = _memoryCache.Get<List<v_Actress>>("ActressTempList");
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetActressesInternal( isLiked); });
                return _cache;
            }
            var list = await this.GetActressesInternal( isLiked); ;
            //排序
            if (sort == SortType.CreateTime.ToString())
            {
                list = list.OrderByDescending(x => x.CreationTime).ToList();
            }
            else if (sort == SortType.ViewCount.ToString())
            {
                list = list.OrderByDescending(y => y.ViewCount).ToList();
            }
            return list;
            

        }
        private async Task<List<v_Actress>> GetActressesInternal( int? isLiked)
        {
            string ApiHost = _configuration.GetValue<string>("ApiHost");
            string apiHostExt = ApiHost + @"/ftp/";
            List<v_Actress> lsActress = new List<v_Actress>();

            var actressDirs = this.GetActressDirs(movieRootPath);
            foreach (var actressItem in actressDirs) //每女优
            {
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = apiHostExt + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover))
                    {
                        actress.Cover = path;
                    }
                }
                lsActress.Add(actress);
            }
            lsActress = lsActress.Where(y => y.IsLiked == isLiked.GetValueOrDefault()).ToList();
            _memoryCache.Set("ActressTempList", lsActress, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return lsActress;
        }
        public async Task<v_Actress> ActressDetails(string actressName)
        {
            v_Actress actress = new v_Actress();
            var _cache = _memoryCache.Get<List<v_Actress>>("ActressTempList");
            Dictionary<string, (int count, int isLiked)> viewCountsDict = this.GetViewCountDict();
            List<v_Movie> movieLists = new List<v_Movie>();

            if (_cache != null && _cache.Count > 0)
            {
                var detail = _cache.Where(x => x.FullName == actressName).FirstOrDefault();
                int count = 0;
                var dt = SQLiteHelper.Query($"SELECT * FROM Dict  WHERE DataKey='{detail.FullName}'");
                if (dt.Tables[0].Rows.Count > 0)
                {
                    var dr = dt.Tables[0].Rows[0];
                    count = int.Parse(dr["DataValue"].ToString());
                    int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET DataValue = '{ ++count}' WHERE DataKey = '{actressName}'; ");
                }
                else
                {
                    int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,DataValue) VALUES ('{actressName}','1');");
                }
                detail.ViewCount = viewCountsDict.Any(x => x.Key == detail.FullName) ? viewCountsDict[detail.FullName].count : 0;
                detail.IsLiked = viewCountsDict.Any(x => x.Key == detail.FullName) ? viewCountsDict[detail.FullName].isLiked : 0;
                actress = detail;
            }
            return actress;
        }
        public async Task<bool> LikingMovie(int id, int isLiked)
        {
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            var m = _cache.Where(y => y.Id == id).FirstOrDefault();
            var dt = SQLiteHelper.Query($"SELECT * FROM Dict  WHERE DataKey='{m.FanHao}'");
            bool isSuccess = false;
            if (dt.Tables[0].Rows.Count > 0)
            {
                var dr = dt.Tables[0].Rows[0];
                
                int _id = int.Parse(dr["Id"].ToString());
                int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET IsLiked ={isLiked}  WHERE DataKey = '{m.FanHao}'; ");
                isSuccess = row > 0;
            }
            else
            {
                int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,IsLiked) VALUES ('{m.FanHao}', {isLiked} );");
                isSuccess = row > 0;
            }
            return isSuccess;
        }
        public async Task<bool> LikingActress(string actressName, int isLiked)
        {
            var dt = SQLiteHelper.Query($"SELECT * FROM Dict  WHERE DataKey='{actressName}'");
            bool isSuccess = false;
            if (dt.Tables[0].Rows.Count > 0)
            {
                var dr = dt.Tables[0].Rows[0];

                int _id = int.Parse(dr["Id"].ToString());
                int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET IsLiked ={isLiked}  WHERE DataKey = '{actressName}'; ");
                isSuccess = row > 0;
            }
            else
            {
                int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,IsLiked) VALUES ('{actressName}', {isLiked} );");
                isSuccess = row > 0;
            }
            return isSuccess;
        }


    }

}
