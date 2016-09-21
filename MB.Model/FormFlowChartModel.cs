using System.Collections.Generic;

namespace MB.Model
{
    public class FormFlowChartModel
    {
        //用户头像
        public string smallImage { get; set; }

        //节点操作信息
        public List<OperateInfoModel> operateInfo { get; set; }
    }

    ///<summary>
    ///节点操作信息
    ///</summary>
    public class OperateInfoModel
    {
        //操作时间
        public string operateTime { get; set; }

        //操作类型
        public int? result { get; set; }

        //节点意见
        public string contents { get; set; }

        //操作信息
        public string operateInfo { get; set; }
    }
}