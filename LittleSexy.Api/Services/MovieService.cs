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
using CXuesong.Shims.VisualBasic.Devices;

namespace LittleSexy.Api.Services
{

    public class MovieService
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        string movieRootPath {get { return _configuration.GetValue<string>("moviePath"); } }


        public IConfiguration _configuration { get; }
        public MovieService(IServiceProvider service,IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, DataValue> GetViewCountDict()
        {
            Dictionary<string, DataValue> dict 
                = new Dictionary<string, DataValue>();
            var dt = SQLiteHelper.Query("SELECT * FROM Dict;");
            if (dt.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    DataValue v = new DataValue();
                    string fanHao = item["Datakey"].ToString();
                    v.count = int.Parse(item["DataValue"].ToString());
                    v.isLiked = int.Parse(item["IsLiked"].ToString());
                    v.LastAccessTime = DateTime.Parse(item["LastAccessTime"].ToString());
                    v.Height = int.Parse( item["Height"].ToString());
                    v.Width = int.Parse(item["Width"].ToString());
                    v.TotalTime =item["TotalTime"].ToString();
                    dict.Add(fanHao, v);
                }
            }
            return dict;
        }

        public  bool UpdateAllMovies()
        {
            var list = this.GetListInternal(null, null);
            return list.Count > 0;

        }
        public List<v_Movie> GetList(string sort, string actressName, int? isLiked, int pageIndex, int pageSize)
        {
            //List<v_Movie> list = new List<v_Movie>();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList" + actressName + isLiked) ;
            if (_cache != null && _cache.Count > 0)
            {
                Task.Run(() => { this.GetListInternal(actressName, isLiked); });
                if (sort == SortType.CreateTime.ToString())
                {
                    _cache = _cache.OrderByDescending(x => x.CreationTime).ToList();
                }
                else if(sort == SortType.ViewCount.ToString())
                {
                    _cache = _cache.OrderByDescending(y => y.ViewCount).ToList();
                }
                return _cache.Skip(pageSize*(pageIndex -1)).Take(pageSize).ToList();
            }

            List<v_Movie> list = this.GetListInternal(actressName, isLiked).Skip(pageSize*(pageIndex -1)).Take(pageSize).ToList();

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
        private string FormatPostedDate(DateTime time)
        {
            TimeSpan diff = DateTime.Now - time;
            string format = "";
            if (diff.TotalDays > 30 * 12) //年
            {
                int years = (int)diff.TotalDays / (30 * 12);
                format = years + " months ago";
            }
            else if (diff.TotalDays > 30 && diff.TotalDays < 30 * 12) //月
            {
                int months = (int)diff.TotalDays / 30;
                format = months + " months ago";
            }
            else if (diff.TotalDays < 30 && diff.TotalDays > 7) //星期
            {
                int week = (int)diff.TotalDays / 7;
                format = week + " week ago";
            }
            else if (diff.TotalDays < 7 && diff.TotalDays > 1) //天
            {
                int days = (int)diff.TotalDays;
                format = days + " days ago";
            }
            else if (diff.TotalDays < 1 && diff.TotalHours > 1) //小时
            {
                int hours = (int)diff.TotalHours;
                format = hours + " hours ago";
            }
            else if (diff.TotalHours < 1 && diff.TotalMinutes > 1) //分钟
            {
                int minutes = (int)diff.TotalMinutes;
                format = minutes + " minutes ago";
            }
            else if (diff.TotalMinutes < 1 && diff.TotalSeconds > 0) //秒
            {
                int seconds = (int)diff.TotalSeconds;
                format = seconds + " seconds ago";
            }
            return format;
        }
        private List<v_Movie> GetListInternal(string actressName, int? isLiked)
        {

            //播放量字典
            Dictionary<string, DataValue> dict = this.GetViewCountDict();
            string movieServer = _configuration.GetValue<string>("movieServer");

            List<v_Movie> LsMovies = new List<v_Movie>();
            var dirs = new DirectoryInfo(movieRootPath).GetDirectories();
            var actressDirs = string.IsNullOrEmpty(actressName) ? dirs : dirs.Where(y => y.Name.Contains(actressName));
            foreach (var actressItem in actressDirs) //每女优
            {
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = movieServer + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover))
                    {
                        actress.Cover = path;
                    }
                }
                var fanHaoDirs = actressItem.GetDirectories(); //所有番号 文件夹
                foreach (var fanHaoItem in fanHaoDirs)
                {
                    TimeSpan _totalTime = new TimeSpan();
                    var fanHaoFiles = fanHaoItem.GetFiles();
                    string FanHaoRaw = fanHaoItem.Name;
                    string FanHao = fanHaoItem.Name.TrimEnd("-C".ToCharArray());
                    v_Movie movie = new v_Movie();
                    foreach (var item in fanHaoFiles)
                    {
                        var ext = item.Extension;
                        if (ext == ".mp4" || ext == ".webm") //视频
                        {
                            var Source = movieServer + item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");
                            movie.Sources.Add(Source);
                            VideoEncoder.VideoFile videoFile = null;
                            if(!dict.Keys.Any(x => x == FanHao ))
                            {
                                int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,DataValue) VALUES ('{FanHao}','0');");
                                dict.Add(FanHao, new DataValue(){ LastAccessTime = DateTime.Now});
                            }
                            bool isffmpeg = dict[FanHao].TotalTime == "0";
                            if(isffmpeg)
                            {
                                VideoEncoder.Encoder enc = new VideoEncoder.Encoder();
                                //ffmpeg.exe的路径，程序会在执行目录（....FFmpeg测试\bin\Debug）下找此文件，
                                enc.FFmpegPath = "ffmpeg.exe";
                                //视频路径
                                videoFile = new VideoEncoder.VideoFile(item.FullName);
                                enc.GetVideoInfo(videoFile);
                                _totalTime = _totalTime.Add(videoFile.Duration); //总时长
                                int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET TotalTime='{string.Format("{0:00}:{1:00}:{2:00}", (int)_totalTime.TotalHours, _totalTime.Minutes, _totalTime.Seconds)}' WHERE DataKey = '{FanHao}'; ");
                            }
                            else
                            {
                                if(dict.Any(x => x.Key == FanHao) )
                                {
                                    TimeSpan.TryParse( dict[FanHao].TotalTime, out TimeSpan t);
                                    _totalTime = t;
                                }
                                else
                                {
                                    _totalTime = new TimeSpan();
                                }
                                
                            }
                            if (!LsMovies.Contains(movie))
                            {
                                movie.Id = LsMovies.Count + 1;
                                movie.FanHao = FanHao;
                                movie.Date = item.CreationTime.ToString("yyyy-MM-dd HH:mm");
                                movie.CreationTime = item.CreationTime;
                                movie.PostedDate = this.FormatPostedDate(item.CreationTime);
                                movie.LastAccessTime = dict.Any(x => x.Key == FanHao) ? dict[FanHao].LastAccessTime :DateTime.Now;
                                movie.LastAccess = movie.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss");
                                movie.ViewCount = dict.Any(x => x.Key == FanHao) ? dict[FanHao].count : 0;
                                movie.IsLiked = dict.Any(x => x.Key == FanHao) ? dict[FanHao].isLiked : 0;
                                movie.Actress = actress;
                                if(isffmpeg)
                                {
                                    //mpeg信息
                                    movie.Height = videoFile.Height;
                                    movie.Width = videoFile.Width;
                                    movie.DisPlayResolution = videoFile.Width + "x" + videoFile.Height;
                                    int row = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET Height='{videoFile.Height}',Width='{videoFile.Width}' WHERE DataKey = '{FanHao}'; ");

                                }
                                else
                                {
                                    movie.Height = dict[FanHao].Height;
                                    movie.Width = dict[FanHao].Width;
                                    movie.DisPlayResolution = movie.Width + "x" + movie.Height;
                                }
                                LsMovies.Add(movie);
                                
                            }
                            movie.TotalTime = string.Format("{0:00}:{1:00}:{2:00}", (int)_totalTime.TotalHours, _totalTime.Minutes, _totalTime.Seconds);
                            movie.Duration = _totalTime;

                            // //FileInfo fi = new FileInfo(item.FullName); //xx/xx/aa.rar
                            // var rnj= fanHaoFiles.Where(x =>Path.GetExtension( x.Name) == ".jpg" && x.Name.Contains(FanHao)).FirstOrDefault();
                            // //fi.MoveTo(Path.GetFileNameWithoutExtension(rnj.FullName) + ".mp4"); //xx/xx/xx.rar
                            // if(rnj != null)
                            // {
                            //     if(item.FullName.EndsWith("-A.mp4") || item.FullName.EndsWith("-B.mp4"))
                            //     {
                            //         Console.WriteLine( "$AB集$  " + item.FullName );
                            //     }
                            //     else
                            //     {
                            //         if(FanHaoRaw.EndsWith("-C"))
                            //         {
                            //             Computer MyComputer = new Computer();
                            //             string newName = FanHaoRaw + Path.GetFileNameWithoutExtension(rnj.FullName).Replace(FanHaoRaw, "").Replace(FanHao, "") + ".mp4";
                            //             MyComputer.FileSystem.RenameFile(item.FullName, newName);
                            //             string newJpgName = newName.Replace(".mp4", "") + ".jpg";
                            //             MyComputer.FileSystem.RenameFile(rnj.FullName, newJpgName);
                            //             Console.WriteLine( "#中文#  " + item.FullName + " => " + newName);
                            //             Console.WriteLine( "#中文图片#  " + rnj.FullName + " => " + newJpgName);
                            //         }
                            //         else
                            //         {
                            //             Computer MyComputer = new Computer();
                            //             string newName = Path.GetFileNameWithoutExtension(rnj.FullName) + ".mp4";
                            //             MyComputer.FileSystem.RenameFile(item.FullName, newName);
                            //             Console.WriteLine( item.FullName + " => " + newName);
                            //         }
                            //     }
                                

                            // }
                            // else
                            // {
                            //     Console.WriteLine("[失败]" + item.FullName);
                            // }


                        }
                        if (ext == ".jpg" || ext == ".png") //图片
                        {
                            string picRelativePath = item.FullName.Replace(movieRootPath, "")?.Replace("\\", "/");

                            if (item.Name.Contains(FanHao))
                            {
                                movie.Title = Path.GetFileNameWithoutExtension(item.Name);
                                movie.Cover = movieServer + picRelativePath;
                            }
                            else
                            {
                                movie.Preview.Add(movieServer + picRelativePath);
                            }
                        }
                    }
                }
            }
            //默认值
            foreach (var item in LsMovies)
            {
                if (item.Cover == null)
                {
                    continue;
                }
                if (!item.Cover.Contains(".jpg")) //视频不包含图片添加默认图片
                {
                    item.Cover = "../images/default.jpg";
                }
            }
            //是否喜欢条件
            LsMovies = isLiked == null ? LsMovies : LsMovies.Where(x => x.IsLiked == isLiked.GetValueOrDefault()).ToList();
            _memoryCache.Set("movieTempList", LsMovies, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return LsMovies;
        }

        public v_Movie DetailAsync(int id)
        {
            v_Movie movie = new v_Movie();
            var _cache = _memoryCache.Get<List<v_Movie>>("movieTempList");
            Dictionary<string, DataValue> dict = this.GetViewCountDict();
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
                    int row2 = SQLiteHelper.ExecuteNonQuery($"UPDATE Dict SET LastAccessTime = '{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE Id = '{_id}'; ");
                }
                else
                {
                    int row = SQLiteHelper.ExecuteNonQuery($"INSERT INTO Dict ( DataKey,DataValue) VALUES ('{detail.FanHao}','1');");
                }
                detail.LastAccessTime = dict.Any(x => x.Key == detail.FanHao) ? dict[detail.FanHao].LastAccessTime : DateTime.Now;
                detail.LastAccess = detail.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss");
                detail.ViewCount = dict.Any(x => x.Key == detail.FanHao) ? dict[detail.FanHao].count : 0;
                detail.IsLiked = dict.Any(x => x.Key == detail.FanHao) ? dict[detail.FanHao].isLiked : 0;
                movie = detail;
            }
            return movie;
        }

        public List<v_Actress> Actresses(string sort, int? isLiked)
        {
            var _cache = _memoryCache.Get<List<v_Actress>>("ActressTempList");
            if (_cache != null && _cache.Count > 0)
            {
                //Task.Run(() => { this.GetActressesInternal( isLiked); });
                return _cache;
            }
            var list = this.GetActressesInternal( isLiked); ;
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

        private List<v_Actress> GetActressesInternal( int? isLiked)
        {
            //播放量字典
            Dictionary<string, DataValue> dict = this.GetViewCountDict();
            string movieServer = _configuration.GetValue<string>("movieServer");
            List<v_Actress> lsActress = new List<v_Actress>();

            var actressDirs = new DirectoryInfo(movieRootPath).GetDirectories();
            foreach (var actressItem in actressDirs) //每女优
            {
                var fanHaoDirs = actressItem.GetDirectories();
                v_Actress actress = new v_Actress();
                actress.FullName = actressItem.Name;
                actress.MovieCount = fanHaoDirs.Count();
                int _totalViewCount = 0;
                foreach (var item in fanHaoDirs)
                {
                    string _fanHao= item.Name.TrimEnd("-C".ToCharArray());
                    _totalViewCount += dict.Any(x => x.Key == _fanHao) ? dict[_fanHao].count : 0;
                }
                actress.TotalViewCount = _totalViewCount;
                var actressFiles = actressItem.GetFiles();// 获取女优图片
                foreach (var photoItem in actressFiles)
                {
                    string picRelativePath = photoItem.FullName.Replace(movieRootPath, "");
                    string path = movieServer + picRelativePath?.Replace("\\", "/");
                    actress.Portraits.Add(path);
                    if (string.IsNullOrEmpty(actress.Cover) && photoItem.FullName.ToLower().Contains("cover"))
                    {
                        actress.Cover = path;
                    }
                    if (string.IsNullOrEmpty(actress.Background) && photoItem.FullName.ToLower().Contains("background"))
                    {
                        actress.Background = path;
                    }
                }
                if (string.IsNullOrEmpty( actress.Cover) && actress.Portraits.Count > 0 )
                {
                    actress.Cover = actress.Portraits.FirstOrDefault();
                }
                lsActress.Add(actress);
            }
            lsActress = lsActress.Where(y => y.IsLiked == isLiked.GetValueOrDefault()).ToList();
            _memoryCache.Set("ActressTempList", lsActress, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)));
            return lsActress;
        }

        public v_Actress ActressDetails(string actressName)
        {
            v_Actress actress = new v_Actress();
            var _cache = _memoryCache.Get<List<v_Actress>>("ActressTempList");
            Dictionary<string, DataValue> dict = this.GetViewCountDict();
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
                detail.ViewCount = dict.Any(x => x.Key == detail.FullName) ? dict[detail.FullName].count : 0;
                detail.IsLiked = dict.Any(x => x.Key == detail.FullName) ? dict[detail.FullName].isLiked : 0;
                actress = detail;
            }
            return actress;
        }

        public bool LikingMovie(int id, int isLiked)
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

        public bool LikingActress(string actressName, int isLiked)
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
