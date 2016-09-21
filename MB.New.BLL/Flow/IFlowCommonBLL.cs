using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL
{
    public interface IFlowCommonBLL
    {
        /// <summary>
        /// 取得节点设置信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        List<NodeInfoModel> GetNodeInfo(TargetNavigationDBEntities db, int templateId);

        /// <summary>
        /// 取得流程节点信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        List<NodeModel> GetFlowNodeInfo(TargetNavigationDBEntities db, int templateId);

        /// <summary>
        /// 根据控件类型取得控件类型名称
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        string GetControlTypeNameByType(EnumDefine.ControlType controlType);
    }
}