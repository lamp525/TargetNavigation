using MB.New.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.New.Model
{
    public class HolidayInfoModel
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime holiday { get; set; }

        /// <summary>
        /// 日期类型
        /// 1：工作日
        /// 2：节假日
        /// </summary>
        public EnumDefine.DateType type { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        public int updateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
}
