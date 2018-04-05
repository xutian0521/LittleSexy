using LittleSexy.Common;
using LittleSexy.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using LittleSexy.DAL;
using System.Threading.Tasks;

namespace LittleSexy.Service
{
    [Inject]
    public class UserService
    {
        protected UserDAL _userDAL;
        public UserService(IServiceProvider service)
        {
            _userDAL= service.GetService<UserDAL>();
        }
        public async Task<IEnumerable<t_User>> GetUserListAsync()
        {
            var list = await _userDAL.GetDataUserListAsync();
            return list;
        }
    }
}
