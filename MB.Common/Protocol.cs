using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Common
{
    //通信协议
    public static class Protocol
    {
        public static class OperateProtocol
        {
            /// <summary>SIM:发送即时消息("SIM+用户id+ClientProtocol.XX")</summary>
            public static string SIM = "SIM";
        }

        //客户端请求协议
        public static class ClientActualTimeProtocol
        {
            /// <summary>PUS:计划待提交</summary>
            public static string PUS = "PUS";

            /// <summary>PUC:计划未审核 </summary>
            public static string PUC = "PUC";

            /// <summary>PNS:计划提交确认</summary>
            public static string PNS = "PNS";

            /// <summary>PAU:申请修改</summary>
            public static string PAU = "PAU";

            /// <summary>PAS:申请中止</summary>
            public static string PAS = "PAS";

            /// <summary>PUA:计划未确认</summary>
            public static string PUA = "PUA";

            /// <summary>OUS:目标待提交</summary>
            public static string OUS = "OUS";

            /// <summary>OUC:目标未审核</summary>
            public static string OUC = "OUC";

            /// <summary>ONS:目标提交确认</summary>
            public static string ONS = "ONS";

            /// <summary>OUA:目标未确认</summary>
            public static string OUA = "OUA";

            /// <summary>FUS流程未提交</summary>
            public static string FUS = "FUS";

            /// <summary>FUC:流程未审核</summary>
            public static string FUC = "FUC";

            /// <summary>IUR:IM未读消息</summary>
            public static string IUR = "IUR";
        }
    }
}
