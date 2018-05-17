using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Threading.Tasks;

namespace LittleSexy.DAL
{
    [Inject]
    public class UserDAL
    {
        public async Task<IEnumerable<t_User>> GetDataUserListAsync()
        {
            //var row= await DB.Conn().ExecuteAsync("insert into LittleSexy.t_User(UserName,PassWord,IsDeleted,CreateDateTime,Remark) values('zs','123',0,'2018-4-6 00:37:53','zs11'); ");
            var listData= await DB.Conn().QueryAsync<t_User>("SELECT * FROM LittleSexy.t_User;");
            return listData;
        }
    }
}
