using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class NodeEditBLL : INodeEditBLL
    {
        #region 变量区域

        /// <summary>流程共通处理 </summary>
        private FlowCommonBLL flowCommon = new FlowCommonBLL();

        #endregion 变量区域

        #region 取得节点字段信息

        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>节点字段信息</returns>
        public List<NodeFieldEditModel> GetNodeFieldList(int? nodeId, int templateId)
        {
            var nodeFieldList = new List<NodeFieldEditModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                var status = (int)ConstVar.nodeControlStatus.readOnly;

                if (nodeId.HasValue)
                {
                    var nodeType = db.tblFlowNode.Where(p => p.nodeId == nodeId.Value && p.templateId == templateId).FirstOrDefault().nodeType;

                    status = nodeType == (int)ConstVar.NodeType.Create ? (int)ConstVar.nodeControlStatus.edit : (int)ConstVar.nodeControlStatus.readOnly;
                }

                nodeFieldList = (from control in db.tblTemplateControl
                                 where control.templateId == templateId
                                 select new NodeFieldEditModel
                                 {
                                     templateId = templateId,
                                     parentControl = control.parentControl,
                                     controlTitle = control.title,
                                     controlDescription = control.description,
                                     controlType = control.controlType,
                                     isDetail = false,
                                     controlId = control.controlId,
                                     status = status
                                 }).ToList();

                var savedControlList = (from field in db.tblNodeField
                                        where field.nodeId == nodeId
                                        select new NodeFieldModel
                                        {
                                            nodeId = field.nodeId,
                                            controlId = field.controlId,
                                            status = field.status
                                        }).ToList();

                foreach (var item in nodeFieldList)
                {
                    var control = savedControlList.Where(p => p.controlId == item.controlId).FirstOrDefault();
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
                            item.controlTitle = flowCommon.GetControlTypeNameByType(item.controlType);
                        }
                        else
                        {
                            item.controlTitle = item.controlDescription;
                        }
                    }

                    //取得当前控件的父控件信息
                    var parentControlInfo = nodeFieldList.Where(p => p.controlId == item.parentControl && p.templateId == templateId).FirstOrDefault();

                    if (parentControlInfo != null)
                    {
                        //类型为明细列表
                        if (parentControlInfo.controlType == (int)ConstVar.ControlType.DetailList)
                        {
                            item.isDetail = true;
                            //控件标题
                            item.controlTitle = parentControlInfo.controlTitle + "-" + item.controlTitle;
                        }
                    }
                }
            }

            return nodeFieldList;
        }

        #endregion 取得节点字段信息

        #region 保存节点信息设置

        /// <summary>
        /// 保存节点信息设置
        /// </summary>
        /// <param name="nodeInfo">节点信息</param>
        /// <param name="deleteNode">需删除的节点ID</param>
        /// <param name="deleteOperateId">需删除的节点操作ID</param>
        /// <returns></returns>
        public void SaveFlowNode(List<NodeInfoModel> nodeInfo, int[] deleteNode, int[] deleteOperateId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //删除节点信息
                DeleteNodeInfo(db, deleteNode);

                //删除节点操作信息
                // DeleteOperateInfo(db, deleteOperateId);

                //更新或插入节点信息
                if (nodeInfo.Count > 0)
                {
                    foreach (var item in nodeInfo)
                    {
                        //节点ID
                        var nodeId = 0;
                        //节点类型
                        var nodeType = item.node.nodeType.Value;

                        //节点ID不为空，更新流程节点
                        if (item.node.nodeId.HasValue)
                        {
                            nodeId = item.node.nodeId.Value;
                            UpdateFlowNode(db, item.node);

                            if (item.nodeField.Count > 0)
                            {
                                foreach (var nodeFieldModel in item.nodeField)
                                {
                                    //更新字段表中已有的字段信息
                                    UpdateNodeField(db, nodeFieldModel);
                                }
                            }
                        }
                        //节点ID为空，插入流程节点
                        else
                        {
                            nodeId = InsertFlowNode(db, item.node);
                            //插入设置的节点字段信息
                            if (item.nodeField.Count > 0)
                            {
                                foreach (var nodeFieldModel in item.nodeField)
                                {
                                    InsertNodeField(db, nodeFieldModel, nodeId);
                                }
                            }
                            //插入默认的节点字段信息
                            else
                            {
                                InsertDefaultField(db, item.node.templateId, nodeId, nodeType);
                            }
                        }

                        //节点操作人
                        if (item.node.operateEditFlag.HasValue && item.node.operateEditFlag.Value)
                        {
                            //插入节点操作信息
                            if (item.nodeOperate.Count > 0)
                            {
                                //取得删除节点ID对应的操作人ID
                                var operateId = db.tblNodeOperate.Where(p => p.nodeId == nodeId).Select(p => p.operateId).ToArray();
                                if (operateId.Length > 0)
                                {
                                    //删除已有的节点操作信息
                                    DeleteOperateInfo(db, operateId);
                                }

                                foreach (var nodeOperateModel in item.nodeOperate)
                                {
                                    //插入节点操作信息
                                    InsertOperateInfo(db, nodeOperateModel, nodeId);
                                }
                            }
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        #endregion 保存节点信息设置

        #region 检查节点删除标志

        /// <summary>
        /// 检查节点删除标志
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        public bool CheckDeleteFlag(int nodeId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var linkNum = db.tblNodeLink.Where(p => p.nodeEntry.Value == nodeId || p.nodeExit.Value == nodeId).ToList().Count();

                if (linkNum == 0)
                    return true;
                else
                    return false;
            }
        }

        #endregion 检查节点删除标志

        #region 私有方法

        #region 更新流程节点信息

        /// <summary>
        /// 更新流程节点信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeModel">流程节点信息</param>
        /// <returns></returns>
        private void UpdateFlowNode(TargetNavigationDBEntities db, NodeModel nodeModel)
        {
            //更新流程节点表
            var node = db.tblFlowNode.Where(f => f.nodeId == nodeModel.nodeId).FirstOrDefault();

            if (node != null)
            {
                //节点名称
                node.nodeName = nodeModel.nodeName;
                //节点类型
                node.nodeType = nodeModel.nodeType;
            }
        }

        #endregion 更新流程节点信息

        #region 插入流程节点信息

        /// <summary>
        ///  插入流程节点信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="flowNodeModel">流程节点信息</param>
        /// <returns>节点ID</returns>
        private int InsertFlowNode(TargetNavigationDBEntities db, NodeModel flowNodeModel)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            //插入流程节点表
            var id = db.prcGetPrimaryKey("tblFlowNode", obj).FirstOrDefault().Value;
            var flowNode = new tblFlowNode
            {
                nodeId = id,
                templateId = flowNodeModel.templateId,
                nodeType = flowNodeModel.nodeType,
                nodeName = flowNodeModel.nodeName
            };

            db.tblFlowNode.Add(flowNode);

            return id;
        }

        #endregion 插入流程节点信息

        #region 更新节点字段信息

        /// <summary>
        /// 更新节点信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeFieldModel">节点字段信息</param>
        /// <returns></returns>
        private void UpdateNodeField(TargetNavigationDBEntities db, NodeFieldModel nodeFieldModel)
        {
            var nodeField = db.tblNodeField.Where(n => n.nodeId == nodeFieldModel.nodeId && n.controlId == nodeFieldModel.controlId).FirstOrDefault();

            if (nodeField != null)
            {
                //更新节点字段信息
                nodeField.status = nodeFieldModel.status;
            }
        }

        #endregion 更新节点字段信息

        #region 插入节点字段信息

        /// <summary>
        ///  插入流程节点信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeFieldModel">节点字段信息</param>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        private void InsertNodeField(TargetNavigationDBEntities db, NodeFieldModel nodeFieldModel, int nodeId)
        {
            //插入节点字段表
            var nodeField = new tblNodeField
            {
                controlId = nodeFieldModel.controlId,
                nodeId = nodeId,
                status = nodeFieldModel.status
            };
            db.tblNodeField.Add(nodeField);
        }

        #endregion 插入节点字段信息

        #region 插入默认的节点字段信息

        /// <summary>
        /// 插入默认的节点字段信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="templateId">模版ID</param>
        /// <param name="nodeId">节点ID</param>
        /// <param name="nodeType">节点类型</param>
        private void InsertDefaultField(TargetNavigationDBEntities db, int templateId, int nodeId, int nodeType)
        {
            var controlIdList = db.tblTemplateControl.Where(p => p.templateId == templateId).Select(p => p.controlId).ToList();
            if (controlIdList.Count > 0)
            {
                foreach (var id in controlIdList)
                {
                    var nodeField = new tblNodeField
                    {
                        nodeId = nodeId,
                        controlId = id,
                        status = nodeType == (int)ConstVar.NodeType.Create ? (int)ConstVar.nodeControlStatus.edit : (int)ConstVar.nodeControlStatus.readOnly
                    };
                    db.tblNodeField.Add(nodeField);
                }
            }
        }

        #endregion 插入默认的节点字段信息

        #region 插入节点操作信息

        /// <summary>
        ///  插入节点操作信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeOperateModel">节点操作信息</param>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        private void InsertOperateInfo(TargetNavigationDBEntities db, NodeOperateModel nodeOperateModel, int nodeId)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);

            //取得节点操作人ID
            var operateId = db.prcGetPrimaryKey("tblNodeOperate", obj).FirstOrDefault().Value;
            //插入节点操作人表
            var nodeOperate = new tblNodeOperate
            {
                operateId = operateId,
                nodeId = nodeId,
                type = nodeOperateModel.type,
                condition = nodeOperateModel.condition,
                countersign = nodeOperateModel.countersign,
                batchType = nodeOperateModel.batchType,
                batchCondition = nodeOperateModel.batchCondition,
                orderNum = nodeOperateModel.orderNum,
                testFlag = false
            };
            db.tblNodeOperate.Add(nodeOperate);

            //插入操作人结果表
            if (nodeOperateModel.targetId.Length > 0)
            {
                foreach (var target in nodeOperateModel.targetId)
                {
                    //取得条件结果ID
                    var id = db.prcGetPrimaryKey("tblOperateResult", obj).FirstOrDefault().Value;

                    var operateResult = new tblOperateResult
                    {
                        resultId = id,
                        operateId = operateId,
                        targetId = target
                    };
                    db.tblOperateResult.Add(operateResult);
                }
            }

            if (nodeOperateModel.batchTargetId.Length > 0)
            {
                //插入批次条件结果表
                foreach (var batchTarget in nodeOperateModel.batchTargetId)
                {
                    //取得批次条件结果ID
                    var batchId = db.prcGetPrimaryKey("tblBatchResult", obj).FirstOrDefault().Value;

                    var batchResult = new tblBatchResult
                    {
                        resultId = batchId,
                        operateId = operateId,
                        targetId = batchTarget
                    };
                    db.tblBatchResult.Add(batchResult);
                }
            }
        }

        #endregion 插入节点操作信息

        #region 删除节点信息

        /// <summary>
        ///  删除节点信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点ID</param>
        /// <returns></returns>
        private void DeleteNodeInfo(TargetNavigationDBEntities db, int[] nodeId)
        {
            if (nodeId.Length > 0)
            {
                //删除流程节点表信息
                var nodeList = (from node in db.tblFlowNode
                                where nodeId.Contains(node.nodeId)
                                select node
                                ).ToList();
                db.tblFlowNode.RemoveRange(nodeList);

                //删除节点字段表信息
                var fieldList = (from field in db.tblNodeField
                                 where nodeId.Contains(field.nodeId)
                                 select field
                                 ).ToList();
                db.tblNodeField.RemoveRange(fieldList);

                //取得删除节点ID对应的操作人ID
                var operateId = (from operate in db.tblNodeOperate
                                 where nodeId.Contains(operate.nodeId.Value)
                                 select operate.operateId
                                  ).ToArray();

                //删除节点操作信息
                this.DeleteOperateInfo(db, operateId);
            }
        }

        #endregion 删除节点信息

        #region 删除节点操作信息

        /// <summary>
        ///  删除节点操作信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="operateId">节点操作人ID</param>
        /// <returns></returns>
        private void DeleteOperateInfo(TargetNavigationDBEntities db, int[] operateId)
        {
            if (operateId.Length > 0)
            {
                //删除节点操作人表信息
                var operateList = (from p in db.tblNodeOperate
                                   where operateId.Contains(p.operateId)
                                   select p
                                    ).ToList();
                db.tblNodeOperate.RemoveRange(operateList);

                //删除操作人结果表信息
                var resultList = (from r in db.tblOperateResult
                                  where operateId.Contains(r.operateId.Value)
                                  select r
                                  ).ToList();
                db.tblOperateResult.RemoveRange(resultList);

                //删除批次条件结果表信息
                var batchResultList = (from b in db.tblBatchResult
                                       where operateId.Contains(b.operateId.Value)
                                       select b
                                  ).ToList();
                db.tblBatchResult.RemoveRange(batchResultList);
            }
        }

        #endregion 删除节点操作信息

        # endregion
    }
}