using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Api.Models
{
    public class v_Movie
    {
        public int Id { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        /// <value></value>
        public string Title { get; set; }
        /// <summary>
        /// 详情页跳转地址
        /// </summary>
        /// <value></value>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 电影预览图
        /// </summary>
        public List<string> Preview { get; set; } = new List<string>();
        /// <summary>
        /// 源
        /// </summary>
        /// <value></value>
        public List<string> Sources { get; set; } = new List<string>();
        /// <summary>
        /// 电影时长raw
        /// </summary>
        public TimeSpan Duration { get; set; }
        /// <summary>
        /// 电影时长format
        /// </summary>
        public string TotalTime { get; set; }
        /// <summary>
        /// 视频高度px
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 视频宽度px
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 显示分辨率 (1920 x 1080)
        /// </summary>
        public string DisPlayResolution { get; set; }

        /// <summary>
        /// 番号
        /// </summary>
        /// <value></value>
        public string FanHao { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string PostedDate { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public string Date { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 上次访问时间
        /// </summary>
        public DateTime LastAccessTime { get; set; }
        public string LastAccess { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        /// <value></value>
        public string Cover {get;set;}
        /// <summary>
        /// 播放量
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 是否喜欢
        /// </summary>
        public int IsLiked { get; set; }
        /// <summary>
        /// 女优
        /// </summary>
        public v_Actress Actress { get; set; } = new v_Actress();

    }
}
