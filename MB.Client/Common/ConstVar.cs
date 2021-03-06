﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Client.Common
{
    public static class ConstVar
    {
        //客户端随机请求类型
        public enum RequestType
        {
            /// <summary>1、昨日工时</summary>
            YWorkTime = 1,

            /// <summary>2、本周工时</summary>
            WWorkTime = 2,

            /// <summary>3、计划未审核</summary>
            unCheck = 3,

            /// <summary>4、计划未确认</summary>
            unConfirm = 4
        }
    }
}
