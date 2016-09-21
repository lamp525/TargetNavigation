using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FW.TaskRemindService
{
    [ServiceContract]
    public interface ITaskRemindService
    {
        #region 发送消息到客户端
        /// <summary>
        /// 发送消息到客户端
        /// </summary>      
        [OperationContract]
        void Send(string msg);
        #endregion
    }
}
