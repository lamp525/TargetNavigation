using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Model.UserIndexModels
{
    public class UserIndexWorkTimeModel
    {
        //本周有效工时
        public decimal? weekTotalWorkTime { get; set; }
        //本月总有效工时
        public decimal? monthTotalWorkTime { get; set; }
        //昨日有效工时
        public decimal? yesterdayWorkTime { get; set; }
        //本周平均工时
        public decimal? weekAvgWorkTIme { get; set; }
        //头像地址
        public string imgUrl { get; set; }
    }
}
