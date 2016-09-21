using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class FlowEditBLL : IFlowEditBLL
    {
        #region 变量区域

        /// <summary>条件插入序列</summary>
        private class SerialModel
        {
            //序列号
            public int? serialNumber { get; set; }

            //条件ID
            public int conditionId { get; set; }
        }

        #endregion 变量区域

        #region 保存流程信息设置

        /// <summary>
        /// 保存流程信息设置
        /// </summary>
        /// <param name="linkInfo">流程信息</param>
        /// <param name="deleteLinkId">删除的流程信息</param>
        /// <param name="deleteConditionId">删除的流程条件</param>
        /// <returns></returns>
        public void SaveNodeLink(List<NodeLinkInfoModel> linkInfo, int[] deleteLinkId, int[] deleteConditionId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //删除节点流程信息
                DeleteLinkInfo(db, deleteLinkId);

                //删除流程条件信息
                DeleteConditionInfo(db, deleteConditionId);

                //更新或插入节点流程信息
                if (linkInfo.Count > 0)
                {
                    foreach (var item in linkInfo)
                    {
                        //节点出口ID
                        var linkId = 0;

                        //条件插入序列信息
                        var serialList = new List<SerialModel>();

                        //节点出口ID不为空
                        if (item.nodeLinkMode.linkId.HasValue)
                        {
                            linkId = item.nodeLinkMode.linkId.Value;
                            //更新节点流程
                            UpdateNodeLink(db, item.nodeLinkMode);
                            //删除条件公式
                            DeleteFormulaInfo(db, linkId);
                        }
                        //节点出口ID为空，插入节点流程
                        else
                        {
                            linkId = InsertNodeLink(db, item.nodeLinkMode);
                        }

                        //插入流程条件信息
                        if (item.linkConditionList.Count > 0)
                        {
                            foreach (var linkCondition in item.linkConditionList)
                            {
                                var serialModel = new SerialModel();

                                //设置节点出口ID
                                linkCondition.linkId = linkId;
                                serialModel.conditionId = InsertLinkCondition(db, linkCondition);
                                serialModel.serialNumber = linkCondition.serialNumber;

                                serialList.Add(serialModel);
                            }
                        }

                        //插入流程条件公式信息
                        if (item.linkFormulaList.Count > 0)
                        {
                            foreach (var linkFormula in item.linkFormulaList)
                            {
                                //设置节点出口ID
                                linkFormula.linkId = linkId;
                                //若操作符为空，设置流程条件ID
                                if (linkFormula.serialNumber.HasValue && !linkFormula.conditionId.HasValue)
                                {
                                    var serialInfo = serialList.Where(p => p.serialNumber == linkFormula.serialNumber).FirstOrDefault();
                                    if (serialInfo != null)
                                    {
                                        linkFormula.conditionId = serialInfo.conditionId;
                                    }
                                    //linkFormula.conditionId = serialList.Where(p => p.serialNumber == linkFormula.serialNumber).FirstOrDefault().conditionId;
                                }
                                InsertLinkFormula(db, linkFormula);
                            }
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        #endregion 保存流程信息设置

        #region 私有方法

        #region 更新节点流程

        /// <summary>
        /// 更新节点流程
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeLinkModel">节点流程</param>
        /// <returns></returns>
        private void UpdateNodeLink(TargetNavigationDBEntities db, NodeLinkModel nodeLinkModel)
        {
            //更新节点流程表
            var link = db.tblNodeLink.Where(n => n.templateId == nodeLinkModel.templateId && n.linkId == nodeLinkModel.linkId).FirstOrDefault();

            if (link != null)
            {
                //入口节点ID
                link.nodeEntry = nodeLinkModel.nodeEntryId;

                //出口节点ID
                link.nodeExit = nodeLinkModel.nodeExitId;

                //状态
                link.status = nodeLinkModel.status;
            }
        }

        #endregion 更新节点流程

        #region 插入节点流程

        /// <summary>
        /// 插入节点流程
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeLinkModel">节点流程</param>
        /// <returns>节点出口ID</returns>
        private int InsertNodeLink(TargetNavigationDBEntities db, NodeLinkModel nodeLinkModel)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            //插入节点流程
            var id = db.prcGetPrimaryKey("tblNodeLink", obj).FirstOrDefault().Value;
            var link = new tblNodeLink
            {
                linkId = id,
                templateId = nodeLinkModel.templateId,
                nodeEntry = nodeLinkModel.nodeEntryId,
                nodeExit = nodeLinkModel.nodeExitId,
                status = nodeLinkModel.status,
                testFlag = false
            };

            db.tblNodeLink.Add(link);
            return id;
        }

        #endregion 插入节点流程

        #region 插入流程条件信息

        /// <summary>
        /// 插入流程条件信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeLinkModel">流程条件</param>
        /// <returns>条件ID</returns>
        private int InsertLinkCondition(TargetNavigationDBEntities db, LinkConditionModel linkCondition)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            //插入流程条件表
            var id = db.prcGetPrimaryKey("tblLinkCondition", obj).FirstOrDefault().Value;
            var conditionModel = new tblLinkCondition
            {
                conditionId = id,
                linkId = linkCondition.linkId,
                type = linkCondition.type,
                controlId = linkCondition.controlId,
                condition = linkCondition.condition,
                value = linkCondition.value,
                testFlag = false
            };
            db.tblLinkCondition.Add(conditionModel);

            //插入流程条件结果表
            if (linkCondition.targetId != null)
            {
                if (linkCondition.targetId.Length > 0)
                {
                    foreach (var target in linkCondition.targetId)
                    {
                        var result = db.prcGetPrimaryKey("tblLinkResult", obj).FirstOrDefault().Value;
                        var resultModel = new tblLinkResult
                        {
                            resultId = result,
                            conditionId = id,
                            targetId = target
                        };
                        db.tblLinkResult.Add(resultModel);
                    }
                }
            }

            return id;
        }

        #endregion 插入流程条件信息

        #region 插入流程条件公式

        /// <summary>
        /// 插入流程条件公式
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkFormula">流程条件公式</param>
        /// <returns></returns>
        private void InsertLinkFormula(TargetNavigationDBEntities db, LinkFormulaModel linkFormula)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            //插入流程条件公式表
            var id = db.prcGetPrimaryKey("tblLinkFormula", obj).FirstOrDefault().Value;
            var formulaModel = new tblLinkFormula
            {
                formulaId = id,
                linkId = linkFormula.linkId,
                conditionId = linkFormula.conditionId,
                displayText = linkFormula.displayText,
                operate = linkFormula.operate,
                orderNum = linkFormula.orderNum
            };

            db.tblLinkFormula.Add(formulaModel);
        }

        #endregion 插入流程条件公式

        #region 删除节点流程信息

        /// <summary>
        /// 删除节点流程信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkId">节点出口ID</param>
        /// <returns></returns>
        private void DeleteLinkInfo(TargetNavigationDBEntities db, int[] linkId)
        {
            if (linkId.Length > 0)
            {
                //删除节点流程表
                var nodeLink = (from link in db.tblNodeLink
                                where linkId.Contains(link.linkId)
                                select link
                                 ).ToList();

                db.tblNodeLink.RemoveRange(nodeLink);

                //删除流程条件公式表
                var formulaList = (from formula in db.tblLinkFormula
                                   where linkId.Contains(formula.linkId.Value)
                                   select formula
                                   ).ToList();
                db.tblLinkFormula.RemoveRange(formulaList);

                //取得节点操作人ID对应的条件ID
                var conditionId = (from lc in db.tblLinkCondition
                                   where linkId.Contains(lc.linkId.Value)
                                   select lc.conditionId
                                   ).ToArray();

                //删除流程条件信息
                this.DeleteConditionInfo(db, conditionId);
            }
        }

        #endregion 删除节点流程信息

        #region 删除流程公式信息

        /// <summary>
        /// 删除流程公式信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="linkId">节点出口ID</param>
        /// <returns></returns>
        private void DeleteFormulaInfo(TargetNavigationDBEntities db, int linkId)
        {
            //删除流程条件公式表
            var formulaList = (from formula in db.tblLinkFormula
                               where formula.linkId == linkId
                               select formula
                               ).ToList();

            db.tblLinkFormula.RemoveRange(formulaList);
        }

        #endregion 删除流程公式信息

        #region 删除流程条件信息

        /// <summary>
        /// 删除流程条件信息
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="conditionId">流程条件ID</param>
        /// <returns></returns>
        private void DeleteConditionInfo(TargetNavigationDBEntities db, int[] conditionId)
        {
            if (conditionId.Length > 0)
            {
                //删除流程条件表
                var conditionList = (from lc in db.tblLinkCondition
                                     where conditionId.Contains(lc.conditionId)
                                     select lc
                                 ).ToList();

                db.tblLinkCondition.RemoveRange(conditionList);

                //删除流程条件结果表
                var resultList = (from result in db.tblLinkResult
                                  where conditionId.Contains(result.conditionId.Value)
                                  select result
                                  ).ToList();
                db.tblLinkResult.RemoveRange(resultList);
            }
        }

        #endregion 删除流程条件信息

        #endregion 私有方法
    }
}