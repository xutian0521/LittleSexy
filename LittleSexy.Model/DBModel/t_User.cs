using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model.DBModel
{
    public class t_User: DBEntityBase
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
