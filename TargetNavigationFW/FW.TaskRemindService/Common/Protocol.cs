using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FW.TaskRemindService.Common
{
    public static class Protocol
    {
        public static class OperateProtocol
        {
            /// <summary>UOL:用户上线("UOL+用户名+密码+ip地址")</summary>
            public static string UOL = "UOL";

            /// <summary>UDL:用户下线("UDL+用户id")</summary>
            public static string UDL = "UDL";

            /// <summary>SIM:即时提醒("SIM+用户id+ClientProtocol.XX")</summary>
            public static string SIM = "SIM";

            /// <summary>URL:客户端在其他电脑上登陆("URL+用户Id")</summary>
            public static string URL = "URL";

            /// <summary>CVU:版本更新("CVU+用户Id+版本号") </summary>
            public static string CVU = "CVU";

            /// <summary>OWR:17:00的请求</summary>
            public static string OWR = "OWR";

            /// <summary>CAM:客户端随机请求("CAM+用户Id+类型")</summary>
            public static string CAM = "CAM";
        }

        //客户端首次登录请求协议
        public static class ClientLoginProtocol 
        {
            /// <summary>PUB:今天未创建计划</summary>
            public static string PUB = "PUB";

            /// <summary>PHC:今天已经存在计划</summary>
            public static string PHC = "PHC";
        }

        //客户端下线前的提醒协议
        public static class ClientBeforeOLProtocol
        {
            /// <summary>TUE:工时不足八小时 </summary>
            public static string TUE = "TUE";

            /// <summary>THE:工时大于八小时小于十二小时</summary>
            public static string THE = "THE";

            /// <summary>TOE:工时大于十二小时</summary>
            public static string TOE = "TOE";

            /// <summary>PNC:有计划无工时</summary>
            public static string PNC = "PNC";

            /// <summary>PNB:计划未创建</summary>
            public static string PNB = "PNB";
        }

        //客户端随机提醒协议
        public static class ClientRandomProtocol
        {
            /// <summary>TYZ:昨日工时为零</summary>
            public static string TYZ = "TYZ";

            /// <summary>TYN:昨日工时不足</summary>
            public static string TYN = "TYN";

            /// <summary>TWZ:本周工时为零</summary>
            public static string TWZ = "TWZ";

            /// <summary>TWN:本周工时不足 </summary>
            public static string TWN = "TWN";

            /// <summary>PUC:计划未审核</summary>
            public static string PUC = "PUC";

            /// <summary>PUA:计划未确认</summary>
            public static string PUA = "PUA";
        }

        //客户端实时提醒协议
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
