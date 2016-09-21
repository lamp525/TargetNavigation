using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FW.TaskRemindService.Model
{
    public class UserLoginModel
    {
        /// <summary>用户Id</summary>
        public int userId { get; set; }

        /// <summary>用户名</summary>
        public string userName { get; set; }

        /// <summary>密码</summary>
        public string password { get; set; }

        /// <summary>随机数</summary>
        public string randomKey { get; set; }
    }
}
