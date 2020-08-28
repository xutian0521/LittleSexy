using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LittleSexy.Api.Util
{
    public enum SortType
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        CreateTime = 0,
        /// <summary>
        /// 最后访问时间
        /// </summary>
        LastAccessTime = 1,
        /// <summary>
        /// 观看次数
        /// </summary>
        ViewCount = 2,
        /// <summary>
        /// 热门
        /// </summary>
        Hot = 3,
        /// <summary>
        /// 精选
        /// </summary>
        Featured = 4, 

    }
}
