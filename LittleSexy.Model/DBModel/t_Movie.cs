using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model.DBModel
{
    public class t_Movie : DBEntityBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        /// <value></value>
        public string Title { get; set; }

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
        /// 封面
        /// </summary>
        /// <value></value>
        public string Cover {get;set;}
        /// <summary>
        /// 详情
        /// </summary>
        /// <value></value>
        public string Details {get;set;}
        /// <summary>
        /// 电影文件创建时间
        /// </summary>
        /// <value></value>
        public DateTime CreationTime { get; set; }

    }
}
