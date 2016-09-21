using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL
{
    public class FlowEditBLL : IFlowEditBLL
    {
        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name=""></param>
        /// <param name="templateId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<NodeFieldEditModel> GetNodeFieldList(TargetNavigationDBEntities db, int templateId, int? nodeId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            IFlowCommonBLL FlowCommon = new FlowCommonBLL();

            var result = new List<NodeFieldEditModel>();

            var status = EnumDefine.NodeControlStatus.ReadOnly;

            if (nodeId.HasValue)
            {
                var node = db.tblFlowNode.Where(x => x.nodeId == nodeId.Value && x.templateId == templateId).FirstOrDefault();
                if (node != null)
                    status = node.nodeType == (int)EnumDefine.NodeType.Create ? EnumDefine.NodeControlStatus.Edit : EnumDefine.NodeControlStatus.ReadOnly;
            }

            //取得模版控件信息
            result = db.tblTemplateControl.Where(x => x.templateId == templateId)
                .Select(x => new NodeFieldEditModel
                {
                    templateId = x.templateId,
                    parentControl = x.parentControl,
                    controlTitle = x.title,
                    controlDescription = x.description,
                    controlType = (EnumDefine.ControlType)x.controlType,
                    isDetail = false,
                    controlId = x.controlId,
                    status = status
                }).ToList();

            //取得已保存的节点字段控件信息
            var saveControllInfo = db.tblNodeField.Where(x => x.nodeId == nodeId)
                .Select(x => new NodeFieldModel
                {
                    nodeId = x.nodeId,
                    controlId = x.controlId,
                    status = (EnumDefine.NodeControlStatus)x.status
                }).ToList();

            foreach (var item in result)
            {
                var control = saveControllInfo.Where(x => x.controlId == item.controlId).FirstOrDefault();
                if (control != null)
                {
                    item.nodeId = control.nodeId;
                    item.status = control.status;
                }

                //控件标题为空设置为控件描述，控件描述也为空则设置为控件类型名
                if (string.IsNullOrEmpty(item.controlTitle))
                {
                    if (string.IsNullOrEmpty(item.controlDescription))
                    {
                        item.controlTitle = FlowCommon.GetControlTypeNameByType(item.controlType);
                    }
                    else
                    {
                        item.controlTitle = item.controlDescription;
                    }
                }

                //取得当前控件的父控件信息
                var parentControlInfo = result.Where(p => p.controlId == item.parentControl && p.templateId == templateId).FirstOrDefault();

                if (parentControlInfo != null)
                {
                    //类型为明细列表
                    if (parentControlInfo.controlType == EnumDefine.ControlType.DetailList)
                    {
                        item.isDetail = true;
                        //控件标题
                        item.controlTitle = parentControlInfo.controlTitle + "-" + item.controlTitle;
                    }
                }
            }

            return result;
        }
    }
}