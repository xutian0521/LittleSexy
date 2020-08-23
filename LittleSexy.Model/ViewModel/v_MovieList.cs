using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model.ViewModel
{
    public class v_Movie
    {
        public long Id { get; set; }
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
        /// 源
        /// </summary>
        /// <value></value>
        public string Source { get; set; }
        /// <summary>
        /// 番号
        /// </summary>
        /// <value></value>
        public string FanHao { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        /// <value></value>
        public string Date { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        /// <value></value>
        public string Cover {get;set;}
        /// <summary>
        /// 播放量
        /// </summary>
        public int ViewCount { get; set; }

    }
}
