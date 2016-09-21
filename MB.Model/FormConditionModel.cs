using System.Collections.Generic;

namespace MB.Model
{
    //流程首页搜索
    public class FormConditionModel
    {
        //状态
        public int[] status { get; set; }

        //创建时间
        public string[] time { get; set; }

        //创建人
        public int[] person { get; set; }

        //创建部门
        public int[] department { get; set; }

        //流程分类
        public int? type { get; set; }

        //排序
        public List<Sort> sort { get; set; }
    }
}