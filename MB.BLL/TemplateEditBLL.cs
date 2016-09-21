using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class TemplateEditBLL : ITemplateEditBLL
    {
        #region 变量

        /// <summary>
        /// 入口/出口节点西悉尼
        /// </summary>
        private class NodeLink
        {
            /// <summary>入口节点ID</summary>
            public int nodeEntryId { get; set; }

            /// <summary>入口节点类型</summary>
            public int? nodeEntryType { get; set; }

            /// <summary>出口节点ID</summary>
            public int nodeExitId { get; set; }

            /// <summary>入口节点类型</summary>
            public int? nodeExitType { get; set; }

            /// <summary>状态</summary>
            public int? status { get; set; }
        }

        #endregion 变量

        #region 模板详情取得

        /// <summary>
        /// 模板详情取得
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns>模板详情</returns>
        public TemplateInfoModel GetTemplateInfoById(int templateId)
        {
            var model = new TemplateInfoModel();
            var controlInfoList = new List<TemplateControlInfoModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                // 取得模板信息
                var template = db.tblTemplate.Where(p => p.templateId == templateId && (p.deleteFlag == null || p.deleteFlag == false)).FirstOrDefault();

                if (template == null)
                {
                    return null;
                }

                var templateModel = new TemplateModel();

                templateModel.templateId = template.templateId;
                templateModel.templateName = template.templateName;
                templateModel.description = template.description;
                templateModel.defaultTitle = template.defaultTitle == false ? 0 : 1;
                templateModel.categoryId = template.categoryId;
                templateModel.status = template.status;
                templateModel.contents = template.contents;
                templateModel.system = template.system.Value;

                model.template = templateModel;

                // 取得模板控件
                var controlList = db.tblTemplateControl.Where(p => p.templateId == templateId).OrderBy(p => p.rowIndex).ThenBy(p => p.columnIndex).ToList();

                foreach (var control in controlList)
                {
                    #region 控件信息

                    var templateControlInfo = new TemplateControlInfoModel();

                    var used = 0;

                    var linkList = (from link in db.tblNodeLink
                                    where link.templateId == templateId
                                    select link.linkId).ToArray();

                    foreach (var linkId in linkList)
                    {
                        var linkModel = db.tblLinkCondition.Where(p => p.linkId == linkId && p.controlId == control.controlId).FirstOrDefault();

                        if (linkModel != null)
                        {
                            used = 1;
                            break;
                        }
                    }

                    // 设置控件信息Model
                    var controlModel = new TemplateControlModel
                    {
                        templateId = control.templateId,
                        controlId = control.controlId,
                        parentControl = control.parentControl,
                        controlType = control.controlType,
                        require = control.require,
                        title = control.title,
                        size = control.size,
                        mutliSelect = control.mutliSelect,
                        vertical = control.vertical,
                        lineType = control.lineType,
                        defaultRowNum = control.defaultRowNum,
                        columnStatistics = control.columnStatistics,
                        color = control.color,
                        description = control.description,
                        linked = control.moneyControl,
                        columnIndex = control.columnIndex,
                        rowIndex = control.rowIndex == null ? int.MaxValue : control.rowIndex,
                        loaded = 1,
                        used = used
                    };

                    // 设置控件信息
                    templateControlInfo.control = controlModel;

                    #endregion 控件信息

                    #region 明细公式

                    // 控件类型为明细子表时，明细公式取得
                    if (controlModel.controlType == (int)ConstVar.ControlType.DetailList)
                    {
                        var formulaModelList = this.GetFormulaList(templateId, control.controlId, db);

                        // 设置明细公式信息
                        templateControlInfo.formula = formulaModelList;
                    }

                    #endregion 明细公式

                    #region 控件项目

                    // 控件类型为单选框，复选框，下拉列表时，取得控件项目
                    if (controlModel.controlType == (int)ConstVar.ControlType.RadioButton ||
                        controlModel.controlType == (int)ConstVar.ControlType.CheckBox ||
                        controlModel.controlType == (int)ConstVar.ControlType.ComboBox)
                    {
                        var itemModelList = this.GetControlItemList(templateId, control.controlId, db);
                        // 设置明细公式信息
                        templateControlInfo.item = itemModelList;
                    }

                    #endregion 控件项目

                    controlInfoList.Add(templateControlInfo);
                }

                model.controlInfo = controlInfoList;
            }

            return model;
        }

        #endregion 模板详情取得

        #region 控件详情取得

        /// <summary>
        /// 控件详情取得
        /// </summary>
        /// <param name="templateId">控件ID</param>
        /// <param name="controlId">控件ID</param>
        /// <returns>控件详情</returns>
        public TemplateControlInfoModel GetControlInfoById(int templateId, string controlId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                #region 控件信息

                // 取得控件信息
                var control = db.tblTemplateControl.Where(p => p.templateId == templateId && p.controlId == controlId).FirstOrDefault();

                if (control == null)
                {
                    return null;
                }

                var templateControlInfo = new TemplateControlInfoModel();

                // 设置控件信息Model
                var controlModel = new TemplateControlModel
                {
                    templateId = control.templateId,
                    controlId = control.controlId,
                    parentControl = control.parentControl,
                    controlType = control.controlType,
                    require = control.require,
                    title = control.title,
                    size = control.size,
                    mutliSelect = control.mutliSelect,
                    vertical = control.vertical,
                    lineType = control.lineType,
                    defaultRowNum = control.defaultRowNum,
                    columnStatistics = control.columnStatistics,
                    color = control.color,
                    description = control.description,
                    linked = control.moneyControl,
                    columnIndex = control.columnIndex,
                    rowIndex = control.rowIndex,
                    loaded = 1
                };

                // 设置控件信息
                templateControlInfo.control = controlModel;

                #endregion 控件信息

                #region 明细公式

                // 控件类型为明细子表时，明细公式取得
                if (controlModel.controlType == (int)ConstVar.ControlType.DetailList)
                {
                    var formulaModelList = this.GetFormulaList(templateId, controlId, db);

                    // 设置明细公式信息
                    templateControlInfo.formula = formulaModelList;
                }

                #endregion 明细公式

                #region 控件项目

                // 控件类型为单选框，复选框，下拉列表时，取得控件项目
                if (controlModel.controlType == (int)ConstVar.ControlType.RadioButton ||
                    controlModel.controlType == (int)ConstVar.ControlType.CheckBox ||
                    controlModel.controlType == (int)ConstVar.ControlType.ComboBox)
                {
                    var itemModelList = this.GetControlItemList(templateId, controlId, db);
                    // 设置明细公式信息
                    templateControlInfo.item = itemModelList;
                }

                #endregion 控件项目

                return templateControlInfo;
            }
        }

        #endregion 控件详情取得

        #region 模板创建

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="templateInfo">模板信息</param>
        /// <param name="userId">用户ID</param>
        public void AddTemplateInfo(TemplateInfoModel templateInfo, int userId)
        {
            if (templateInfo == null || templateInfo.template == null)
            {
                return;
            }

            using (var db = new TargetNavigationDBEntities())
            {
                // 添加模板信息
                var templateId = this.AddTemplate(templateInfo.template, userId, db);

                if (templateInfo.controlInfo == null || templateInfo.controlInfo.Count() == 0)
                {
                    db.SaveChanges();
                    return;
                }

                foreach (var controlInfo in templateInfo.controlInfo)
                {
                    // 添加控件信息
                    this.AddTemplateControl(controlInfo.control, templateId, userId, db);

                    // 添加控件项目
                    if (controlInfo.item != null && controlInfo.item.Count > 0)
                    {
                        controlInfo.item.ForEach(p => this.AddControlItem(p, templateId, controlInfo.control.controlId, db));
                    }

                    // 添加明细公式
                    if (controlInfo.formula != null && controlInfo.formula.Count > 0)
                    {
                        this.AddDetailFormula(controlInfo.formula, templateId, db);
                    }
                }

                db.SaveChanges();
            }
        }

        #endregion 模板创建

        #region 模板编辑

        /// <summary>
        /// 更新模板
        /// </summary>
        /// <param name="templateInfo">模板信息</param>
        /// <param name="userId">用户ID</param>
        public void UpdateTemplateInfo(TemplateInfoModel templateInfo, int userId)
        {
            if (templateInfo == null || templateInfo.template == null)
            {
                return;
            }

            var templateId = templateInfo.template.templateId.Value;

            using (var db = new TargetNavigationDBEntities())
            {
                // 更新模板信息
                this.UpdateTemplate(templateInfo.template, userId, db);

                if (templateInfo.controlInfo != null)
                {
                    foreach (var controlInfo in templateInfo.controlInfo)
                    {
                        // 检查控件是否存在
                        var exist = this.CheckControlExist(templateId, controlInfo.control.controlId, db);

                        // 不存在的场合，添加控件信息
                        if (!exist)
                        {
                            // 添加控件信息
                            this.AddTemplateControl(controlInfo.control, templateId, userId, db);

                            // 添加控件项目
                            if (controlInfo.item != null && controlInfo.item.Count > 0)
                            {
                                controlInfo.item.ForEach(p => this.AddControlItem(p, templateId, controlInfo.control.controlId, db));
                            }

                            // 添加明细公式
                            if (controlInfo.formula != null && controlInfo.formula.Count > 0)
                            {
                                this.AddDetailFormula(controlInfo.formula, templateId, db);
                            }
                        }
                        else
                        {
                            // 更新控件信息
                            this.UpdateTemplateControl(controlInfo.control, templateId, userId, db);

                            // 更新控件项目
                            if (controlInfo.item != null && controlInfo.item.Count > 0)
                            {
                                foreach (var item in controlInfo.item)
                                {
                                    // 项目ID为空时，添加控件项目
                                    if (item.itemId == null)
                                    {
                                        this.AddControlItem(item, templateId, controlInfo.control.controlId, db);
                                    }
                                    else
                                    {
                                        // 更新控件项目
                                        this.UpdateControlItem(item, db);
                                    }
                                }
                            }

                            // 更新明细公式
                            if (controlInfo.formula != null)
                            {
                                // 删除所有明细公式
                                this.DeleteDetailFormula(templateId, new string[] { controlInfo.control.controlId }, db);

                                // 添加明细公式
                                this.AddDetailFormula(controlInfo.formula, templateId, db);
                            }
                        }
                    }
                }

                // 模板控件删除的场合
                if (templateInfo.deleteControl != null)
                {
                    // 根据控件ID，删除模板控件
                    this.DeleteTemplateControlById(templateId, templateInfo.deleteControl, db);

                    // 根据父控件ID，删除子控件
                    string[] controlId = this.DeleteTemplateControlByParent(templateId, templateInfo.deleteControl, db);

                    // 删除子控件项目
                    this.DeleteControlItemByControlId(templateId, controlId, db);

                    // 根据控件ID，删除控件项目
                    this.DeleteControlItemByControlId(templateId, templateInfo.deleteControl, db);

                    // 根据控件ID，删除明细公式
                    this.DeleteDetailFormula(templateId, templateInfo.deleteControl, db);

                    // 根据控件ID，删除节点字段
                    this.DeleteNodeField(templateId, templateInfo.deleteControl, db);

                    // 删除子控件节点字段
                    this.DeleteNodeField(templateId, controlId, db);
                }

                // 控件项目删除的场合
                if (templateInfo.deleteControlItem != null)
                {
                    // 根据项目ID，删除控件项目
                    this.DeleteControlItemById(templateInfo.deleteControlItem, db);
                }

                db.SaveChanges();
            }
        }

        #endregion 模板编辑

        #region 节点设置验证

        /// <summary>
        /// 验证节点设置是否正确
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns></returns>
        public bool CheckNode(int? templateId)
        {
            // 模板ID为空时
            if (templateId == null)
            {
                return false;
            }

            var haveCreateNode = false;
            var haveEndNode = false;

            using (var db = new TargetNavigationDBEntities())
            {
                // 取得节点列表
                var nodeList = db.tblFlowNode.Where(p => p.templateId == templateId);

                foreach (var node in nodeList)
                {
                    // 创建节点
                    if (node.nodeType == (int)ConstVar.NodeType.Create)
                    {
                        haveCreateNode = true;
                    }

                    // 归档节点
                    if (node.nodeType == (int)ConstVar.NodeType.End)
                    {
                        haveEndNode = true;
                    }

                    // 取得节点操作人
                    var operateList = (from operate in db.tblNodeOperate
                                       join result in db.tblOperateResult
                                       on operate.operateId equals result.operateId
                                       where operate.nodeId == node.nodeId
                                       select operate).ToList();

                    // 没有节点操作人
                    if (operateList.Count() == 0)
                    {
                        return false;
                    }

                    // 取得状态为"无"的操作人个数
                    var count = operateList.Where(p => p.countersign == (int)ConstVar.NodeStatus.Approval).ToList().Count();

                    // 操作人个数大于1
                    if (count > 1)
                    {
                        return false;
                    }
                }

                // 没有"创建"类型的节点
                if (!haveCreateNode)
                {
                    return false;
                }

                // 没有"归档"类型的节点
                if (!haveEndNode)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion 节点设置验证

        #region 流程设置验证

        /// <summary>
        /// 验证流程设置是否正确
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <returns></returns>
        public bool CheckFlow(int? templateId)
        {
            // 模板ID为空时
            if (templateId == null)
            {
                return false;
            }

            var haveCreateNode = false;
            var haveEndNode = false;

            using (var db = new TargetNavigationDBEntities())
            {
                // 取得节点流程列表
                var linkList = (from link in db.tblNodeLink
                                join entry in db.tblFlowNode
                                on link.nodeEntry equals entry.nodeId into temp1
                                from entryNode in temp1.DefaultIfEmpty()
                                join exit in db.tblFlowNode
                                on link.nodeExit equals exit.nodeId into temp2
                                from exitNode in temp2.DefaultIfEmpty()
                                select new NodeLink
                                {
                                    nodeEntryId = entryNode.nodeId,
                                    nodeEntryType = entryNode.nodeType,
                                    nodeExitId = exitNode.nodeId,
                                    nodeExitType = exitNode.nodeType,
                                    status = link.status
                                }).ToList();

                foreach (var link in linkList)
                {
                    // 入口为"创建"类型节点
                    if (link.nodeEntryType == (int)ConstVar.NodeType.Create)
                    {
                        haveCreateNode = true;
                    }

                    // 出口为"归档"类型节点
                    if (link.nodeExitType == (int)ConstVar.NodeType.End)
                    {
                        haveEndNode = true;
                    }

                    // 入口为"归档" 或 出口为"创建"
                    if (link.nodeEntryType == (int)ConstVar.NodeType.End || link.nodeExitType == (int)ConstVar.NodeType.Create)
                    {
                        return false;
                    }

                    // 入口节点类型为"提交"或出口节点类型为"归档"时，必须为通过
                    if ((link.nodeEntryType == (int)ConstVar.NodeType.Submit || link.nodeExitType == (int)ConstVar.NodeType.End) && link.status == 0)
                    {
                        return false;
                    }

                    // 入口节点类型为"审批"时，必须同时存在通过和不通过
                    if (link.nodeEntryType == (int)ConstVar.NodeType.Approval)
                    {
                        var status = link.status == 0 ? 1 : 0;
                        var result = linkList.Where(p => p.nodeEntryId == link.nodeEntryId && p.status == status).ToList().Count();

                        if (result == 0)
                        {
                            return false;
                        }
                    }

                    // 除"创建"和"归档"节点外，其他节点必须同时存在于入口和出口节点中
                    if (link.nodeEntryType == (int)ConstVar.NodeType.Submit || link.nodeEntryType == (int)ConstVar.NodeType.Approval)
                    {
                        var result = linkList.Where(p => p.nodeExitId == link.nodeEntryId).ToList().Count();

                        if (result == 0)
                        {
                            return false;
                        }
                    }

                    // 除"创建"和"归档"节点外，其他节点必须同时存在于入口和出口节点中
                    if (link.nodeExitType == (int)ConstVar.NodeType.Submit || link.nodeExitType == (int)ConstVar.NodeType.Approval)
                    {
                        var result = linkList.Where(p => p.nodeEntryId == link.nodeExitId).ToList().Count();

                        if (result == 0)
                        {
                            return false;
                        }
                    }
                }

                // 入口节点中没有"创建"类型的节点
                if (!haveCreateNode)
                {
                    return false;
                }

                // 出口节点中没有"归档"类型的节点
                if (!haveEndNode)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion 流程设置验证

        #region 私有方法

        #region 取得明细公式

        /// <summary>
        /// 取得明细公式
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <returns></returns>
        private List<DetailFormulaModel> GetFormulaList(int templateId, string controlId, TargetNavigationDBEntities db)
        {
            var formulaList = db.tblDetailFormula.Where(p => p.templateId == templateId && p.detailControl == controlId).OrderBy(p => p.orderNum).OrderBy(p => p.formulaId).ToList();

            if (formulaList.Count() > 0)
            {
                var formulaModelList = new List<DetailFormulaModel>();

                formulaList.ForEach(p => formulaModelList.Add(new DetailFormulaModel
                {
                    formulaId = p.formulaId.ToString(),
                    templateId = p.templateId,
                    orderNum = p.orderNum,
                    detailControl = p.detailControl,
                    controlId = p.controlId,
                    displayText = p.displayText,
                    operate = p.operate
                }));

                return formulaModelList;
            }

            return null;
        }

        #endregion 取得明细公式

        #region 取得控件项目

        /// <summary>
        /// 取得控件项目
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db"></param>
        /// <returns></returns>
        private List<ControlItemModel> GetControlItemList(int templateId, string controlId, TargetNavigationDBEntities db)
        {
            var itemList = db.tblControlItem.Where(p => p.templateId == templateId && p.controlId == controlId).OrderBy(p => p.orderNum).ToList();

            if (itemList.Count() > 0)
            {
                var itemModelList = new List<ControlItemModel>();

                itemList.ForEach(p => itemModelList.Add(new ControlItemModel
                {
                    itemId = p.itemId,
                    controlId = p.controlId,
                    templateId = p.templateId,
                    itemText = p.itemText,
                    orderNum = p.orderNum,
                    checkOn = p.checkOn
                }));

                return itemModelList;
            }

            return null;
        }

        #endregion 取得控件项目

        #region 添加模板信息

        /// <summary>
        /// 添加模板信息
        /// </summary>
        /// <param name="template">模板信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        /// <returns>模板ID</returns>"
        private int AddTemplate(TemplateModel template, int userId, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var templateId = db.prcGetPrimaryKey("tblTemplate", obj).FirstOrDefault().Value;
            var templateModel = new tblTemplate
            {
                templateId = templateId,
                templateName = template.templateName,
                description = template.description,
                defaultTitle = template.defaultTitle == 0 ? false : true,
                categoryId = template.categoryId,
                status = template.status,
                contents = template.contents,
                system = template.system,
                createUser = userId,
                createTime = DateTime.Now,
                updateUser = userId,
                updateTime = DateTime.Now,
                deleteFlag = false,
                testFlag = false
            };
            db.tblTemplate.Add(templateModel);

            return templateId;
        }

        #endregion 添加模板信息

        #region 更新模板信息

        /// <summary>
        /// 更新模板信息
        /// </summary>
        /// <param name="template">模板信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        private void UpdateTemplate(TemplateModel template, int userId, TargetNavigationDBEntities db)
        {
            var templateModel = db.tblTemplate.Where(p => p.templateId == template.templateId && p.deleteFlag == false).FirstOrDefault();

            if (templateModel != null)
            {
                // 更新模板信息
                templateModel.templateName = template.templateName;
                templateModel.description = template.description;
                templateModel.defaultTitle = template.defaultTitle == 0 ? false : true;
                //templateModel.categoryId = template.categoryId;
                templateModel.status = template.status;
                templateModel.contents = template.contents;
                //templateModel.system = template.system;
                templateModel.updateUser = userId;
                templateModel.updateTime = DateTime.Now;
                templateModel.testFlag = false;
            }
        }

        #endregion 更新模板信息

        #region 检查控件是否存在

        /// <summary>
        /// 检查控件是否存在
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db">DB</param>
        /// <returns>true：存在  false：不存在</returns>
        private bool CheckControlExist(int templateId, string controlId, TargetNavigationDBEntities db)
        {
            var result = db.tblTemplateControl.Where(p => p.templateId == templateId && p.controlId == controlId).ToList().Count();

            if (result > 0)
            {
                return true;
            }

            return false;
        }

        #endregion 检查控件是否存在

        #region 添加模板控件

        /// <summary>
        /// 添加模板控件
        /// </summary>
        /// <param name="control">模板控件信息</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        private void AddTemplateControl(TemplateControlModel control, int templateId, int userId, TargetNavigationDBEntities db)
        {
            var controlModel = new tblTemplateControl
            {
                templateId = templateId,
                controlId = control.controlId,
                parentControl = control.parentControl,
                controlType = control.controlType,
                require = control.require,
                title = control.title,
                size = control.size,
                mutliSelect = control.mutliSelect,
                vertical = control.vertical,
                lineType = control.lineType,
                defaultRowNum = control.defaultRowNum,
                columnStatistics = control.columnStatistics,
                color = control.color,
                description = control.description,
                moneyControl = control.linked,
                columnIndex = control.columnIndex,
                rowIndex = control.rowIndex == null ? int.MaxValue : control.rowIndex,
                createUser = userId,
                createTime = DateTime.Now,
                updateUser = userId,
                updateTime = DateTime.Now
            };
            db.tblTemplateControl.Add(controlModel);
        }

        #endregion 添加模板控件

        #region 更新模板控件

        /// <summary>
        /// 更新模板控件
        /// </summary>
        /// <param name="control">模板控件信息</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="db">DB</param>
        private void UpdateTemplateControl(TemplateControlModel control, int templateId, int userId, TargetNavigationDBEntities db)
        {
            var controlModel = db.tblTemplateControl.Where(p => p.templateId == templateId && p.controlId == control.controlId).FirstOrDefault();

            if (controlModel != null)
            {
                // 更新控件信息
                controlModel.parentControl = control.parentControl;
                controlModel.controlType = control.controlType;
                controlModel.require = control.require;
                controlModel.title = control.title;
                controlModel.size = control.size;
                controlModel.mutliSelect = control.mutliSelect;
                controlModel.vertical = control.vertical;
                controlModel.lineType = control.lineType;
                controlModel.defaultRowNum = control.defaultRowNum;
                controlModel.columnStatistics = control.columnStatistics;
                controlModel.color = control.color;
                controlModel.description = control.description;
                controlModel.moneyControl = control.linked;
                controlModel.columnIndex = control.columnIndex;
                controlModel.rowIndex = control.rowIndex == null ? int.MaxValue : control.rowIndex;
                controlModel.updateUser = userId;
                controlModel.updateTime = DateTime.Now;
            }
        }

        #endregion 更新模板控件

        #region 添加控件项目

        /// <summary>
        /// 添加控件项目
        /// </summary>
        /// <param name="controlItem">控件项目信息</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db">DB</param>
        private void AddControlItem(ControlItemModel controlItem, int templateId, string controlId, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var itemId = db.prcGetPrimaryKey("tblControlItem", obj).FirstOrDefault().Value;
            var itemModel = new tblControlItem
            {
                itemId = itemId,
                controlId = controlId,
                templateId = templateId,
                itemText = controlItem.itemText,
                orderNum = controlItem.orderNum
            };
            db.tblControlItem.Add(itemModel);
        }

        #endregion 添加控件项目

        #region 更新控件项目

        /// <summary>
        /// 更新控件项目
        /// </summary>
        /// <param name="controlItem">控件项目信息</param>
        /// <param name="db">DB</param>
        private void UpdateControlItem(ControlItemModel controlItem, TargetNavigationDBEntities db)
        {
            var itemModel = db.tblControlItem.Where(p => p.itemId == controlItem.itemId).FirstOrDefault();

            if (itemModel != null)
            {
                // 更新控件项目信息
                itemModel.itemText = controlItem.itemText;
                itemModel.orderNum = controlItem.orderNum;
            }
        }

        #endregion 更新控件项目

        #region 添加明细公式

        /// <summary>
        /// 添加明细公式
        /// </summary>
        /// <param name="formulaList">明细公式信息</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="db">DB</param>
        private void AddDetailFormula(List<DetailFormulaModel> formulaList, int templateId, TargetNavigationDBEntities db)
        {
            List<tblDetailFormula> formulaModelList = null;
            var formulaId = 0;

            foreach (var formula in formulaList)
            {
                if (formula.orderNum == 1)
                {
                    if (formulaModelList != null && formulaModelList.Count() > 0)
                    {
                        db.tblDetailFormula.AddRange(formulaModelList);
                    }

                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    formulaId = db.prcGetPrimaryKey("tblDetailFormula", obj).FirstOrDefault().Value;

                    formulaModelList = new List<tblDetailFormula>();
                }

                formulaModelList.Add(new tblDetailFormula
                {
                    formulaId = formulaId,
                    templateId = templateId,
                    orderNum = formula.orderNum,
                    detailControl = formula.detailControl,
                    controlId = formula.controlId,
                    displayText = formula.displayText,
                    operate = formula.operate
                });
            }

            if (formulaModelList != null)
            {
                db.tblDetailFormula.AddRange(formulaModelList);
            }
        }

        #endregion 添加明细公式

        #region 删除模板控件

        /// <summary>
        /// 根据控件ID删除模板控件
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db">DB</param>
        private void DeleteTemplateControlById(int templateId, string[] controlId, TargetNavigationDBEntities db)
        {
            var controlList = (from control in db.tblTemplateControl
                               where control.templateId == templateId && controlId.Contains(control.controlId)
                               select control).ToList();

            db.tblTemplateControl.RemoveRange(controlList);
        }

        /// <summary>
        /// 根据父控件ID删除模板控件
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="parentId">父控件ID</param>
        /// <param name="db">DB</param>
        /// <returns>子控件ID</returns>
        private string[] DeleteTemplateControlByParent(int templateId, string[] parentId, TargetNavigationDBEntities db)
        {
            List<string> controlId = new List<string>();

            var controlList = (from control in db.tblTemplateControl
                               where control.templateId == templateId && parentId.Contains(control.parentControl)
                               select control).ToList();

            controlList.ForEach(p => controlId.Add(p.controlId));

            db.tblTemplateControl.RemoveRange(controlList);

            return controlId.ToArray();
        }

        #endregion 删除模板控件

        #region 删除控件项目

        /// <summary>
        /// 根据项目ID删除控件项目
        /// </summary>
        /// <param name="itemId">项目ID</param>
        /// <param name="db">DB</param>
        private void DeleteControlItemById(int[] itemId, TargetNavigationDBEntities db)
        {
            var itemList = (from item in db.tblControlItem
                            where itemId.Contains(item.itemId)
                            select item).ToList();

            db.tblControlItem.RemoveRange(itemList);
        }

        /// <summary>
        /// 根据控件ID删除控件项目
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db">DB</param>
        private void DeleteControlItemByControlId(int templateId, string[] controlId, TargetNavigationDBEntities db)
        {
            var itemList = (from item in db.tblControlItem
                            where item.templateId == templateId && controlId.Contains(item.controlId)
                            select item).ToList();

            db.tblControlItem.RemoveRange(itemList);
        }

        #endregion 删除控件项目

        #region 删除明细公式

        /// <summary>
        /// 根据控件ID删除明细公式
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        /// <param name="db">DB</param>
        private void DeleteDetailFormula(int templateId, string[] controlId, TargetNavigationDBEntities db)
        {
            var formulaList = (from formula in db.tblDetailFormula
                               where formula.templateId == templateId && controlId.Contains(formula.detailControl)
                               select formula).ToList();

            db.tblDetailFormula.RemoveRange(formulaList);
        }

        #endregion 删除明细公式

        #region 删除节点字段

        /// <summary>
        /// 删除节点字段信息
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="controlId">控件ID</param>
        private void DeleteNodeField(int templateId, string[] controlId, TargetNavigationDBEntities db)
        {
            foreach (var id in controlId)
            {
                var nodeIdList = (from node in db.tblFlowNode
                                  where node.templateId == templateId
                                  select node.nodeId).ToArray();

                foreach (var nodeId in nodeIdList)
                {
                    var model = db.tblNodeField.Where(p => p.nodeId == nodeId && p.controlId == id).FirstOrDefault();

                    if (model != null)
                    {
                        db.tblNodeField.Remove(model);
                    }
                }
            }
        }

        #endregion 删除节点字段

        #endregion 私有方法
    }
}