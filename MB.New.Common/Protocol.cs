namespace MB.New.Common
{
    //通信协议
    public static class Protocol
    {
        public static class OperateProtocol
        {
            /// <summary>UOL:用户上线("UOL+用户id+ip地址")</summary>
            public static string UOL = "UOL";

            /// <summary>UDL:用户下线("UDL+用户id")</summary>
            public static string UDL = "UDL";

            /// <summary>SIM:发送即时消息("SIM+用户id+ClientProtocol.XX")</summary>
            public static string SIM = "SIM";

            /// <summary>GCM:客户端定时请求("GCM+用户Id+ClientProtocol.XX")</summary>
            public static string GCM = "GCM";
        }

        //客户端请求协议
        public static class ClientProtocol
        {
            /// <summary>PUS:计划待提交</summary>
            public static string PUS = "PUS";

            /// <summary>PUC:计划未审核 </summary>
            public static string PUC = "PUC";

            /// <summary>PNS:计划提交确认</summary>
            public static string PNS = "PNS";

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

        //服务器协议
        public static class ServiceProtocol
        {
            /// <summary>PUS:计划未提交</summary>
            public static string PUS = "PUS";

            /// <summary>TUE:工时不足</summary>
            public static string TUE = "TUE";

            /// <summary>POT:计划超时</summary>
            public static string POT = "POT";

            /// <summary>OOT:目标超时</summary>
            public static string OOT = "OOT";
        }
    }
}