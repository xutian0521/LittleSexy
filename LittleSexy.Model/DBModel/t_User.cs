using System;
using System.Collections.Generic;
using System.Text;

namespace LittleSexy.Model.DBModel
{
    public class t_User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string Remark { get; set; }
    }
}
