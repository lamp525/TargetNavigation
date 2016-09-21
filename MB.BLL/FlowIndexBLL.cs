using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    //流程首页
    public class FlowIndexBLL : IFlowIndexBLL
    {
        #region 变量区域

        private FlowCommonBLL commonBll = new FlowCommonBLL();

        #endregion 变量区域

        #region 获取模板分类列表

        /// <summary>
        /// 获取模板分类列表
        /// </summary>
        /// <returns>模板分类列表</returns>
        public List<TemplateCategoryModel> GetTemplateCategoryList()
        {
            var templateCategoryList = new List<TemplateCategoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                templateCategoryList = (from t in db.tblTemplateCategory
                                        where !t.deleteFlag.Value
                                        select new TemplateCategoryModel
                                        {
                                            categoryId = t.categoryId,
                                            categoryName = t.categoryName
                                        }).ToList();
            }
            return templateCategoryList;
        }

        #endregion 获取模板分类列表

        #region 获取模板列表

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="categoryId">模板分类Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>模板列表</returns>
        public List<TemplateSimpleModel> GetTemplateList(int categoryId, int userId)
        {
            var templateList = new List<TemplateSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                templateList = (from c in db.tblTemplateCategory
                                where !c.deleteFlag.Value && c.categoryId == categoryId
                                select new TemplateSimpleModel
                                {
                                    categoryId = c.categoryId,
                                    categoryName = c.categoryName
                                }).ToList();
                if (templateList.Count > 0)
                {
                    templateList.ForEach(p =>
                    {
                        var thisTemplateList = (from t in db.tblTemplate
                                                where !t.deleteFlag && t.categoryId == p.categoryId && t.status == (int)ConstVar.TemplateStatus.used
                                                select new TemplateModel
                                                {
                                                    templateId = t.templateId,
                                                    templateName = t.templateName,
                                                    description = t.description == null ? "" : t.description
                                                }).ToList();
                        if (thisTemplateList.Count > 0)
                        {
                            p.templateList = new List<TemplateModel>();
                            //筛选有表单创建节点操作人包含登录用户的表单
                            thisTemplateList.ForEach(a =>
                            {
                                //获取当前模板的创建节点
                                var flowNode = db.tblFlowNode.Where(s => s.templateId == a.templateId && s.nodeType == (int)(ConstVar.NodeType.Create)).FirstOrDefault();
                                if (flowNode != null)
                                {
                                    //获取表单创建节点的操作人信息
                                    var operateList = this.GetNextOperateUserList(db, flowNode.nodeId, 0, userId);
                                    if (operateList[0].type == (int)ConstVar.NodeOperatorType.All)
                                    {
                                        p.templateList.Add(a);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < operateList.Count; i++)
                                        {
                                            if (operateList[i].userIds.Contains(userId))
                                            {
                                                p.templateList.Add(a);
                                                break;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    });
                }
            }
            return templateList;
        }

        #endregion 获取模板列表

        #region 获取模板html

        /// <summary>
        /// 获取模板html
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <returns>模板html</returns>
        public TemplateModel GetTemplateHtml(int templateId)
        {
            var templateModel = new TemplateModel();
            using (var db = new TargetNavigationDBEntities())
            {
                templateModel = (from t in db.tblTemplate
                                 where !t.deleteFlag && t.templateId == templateId
                                 select new TemplateModel
                                 {
                                     defaultTitle = t.defaultTitle == false ? 0 : 1,
                                     contents = t.contents
                                 }).FirstOrDefault();
            }
            return templateModel;
        }

        #endregion 获取模板html

        #region 模板详情取得

        /// <summary>
        /// 模板详情取得
        /// </summary>
        /// <param name="templateId">模板ID</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="flag">1、新建时用到 2、详情时用到</param>
        /// <param name="operateStatus">控制详情已提交节点的权限</param>
        /// <returns>模板详情</returns>
        public TemplateInfoModel GetTemplateInfoById(int templateId, int? nodeId, int? formId, int flag, int? operateStatus)
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
                var controlList = db.tblTemplateControl.Where(p => p.templateId == templateId).OrderBy(p => p.rowIndex).ToList();

                //流程节点表和节点流程表关联查出当前节点
                if (flag == 1)
                {
                    var flowNode = (from f in db.tblFlowNode
                                    join n in db.tblNodeLink on f.nodeId equals n.nodeEntry
                                    where f.templateId == templateId && f.nodeType.Value == (int)(ConstVar.NodeType.Create)
                                    select n).FirstOrDefault();
                    //获取控件的显示权限
                    if (flowNode != null)
                    {
                        nodeId = flowNode.nodeEntry;
                    }
                }
                foreach (var control in controlList)
                {
                    #region 控件信息

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
                    //获取表单创建者设置的明细列表行数
                    if (controlModel.controlType == (int)ConstVar.ControlType.DetailList && flag == 2)
                    {
                        var childControl = db.tblTemplateControl.Where(p => p.parentControl == controlModel.controlId).FirstOrDefault();
                        if (childControl != null)
                        {
                            var rowList = db.tblFormDetail.Where(p => p.controlId == childControl.controlId && p.formId == formId);
                            if (rowList.Count() > 0)
                            {
                                var row = rowList.OrderByDescending(a => a.rowNumber).FirstOrDefault();
                                if (row != null && row.rowNumber != null)
                                {
                                    controlModel.defaultRowNum = row.rowNumber.Value;
                                }
                            }
                        }
                    }
                    var nodeFieldModel = db.tblNodeField.Where(p => p.nodeId == nodeId && p.controlId == control.controlId).FirstOrDefault();
                    if (nodeFieldModel == null) continue;
                    controlModel.status = nodeFieldModel.status;
                    if (flag == 2 && controlModel.status == (int)ConstVar.nodeControlStatus.edit && (operateStatus == (int)ConstVar.FormFlowStatusId.hasSubmited || operateStatus == (int)ConstVar.FormFlowStatusId.neverChecked))
                    {
                        controlModel.status = (int)ConstVar.nodeControlStatus.readOnly;
                    }
                    // 设置控件信息,过滤掉当前节点下需要隐藏的控件
                    if (controlModel.status != 0)
                    {
                        templateControlInfo.control = controlModel;
                    }

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

                    if (templateControlInfo.control != null)
                    {
                        controlInfoList.Add(templateControlInfo);
                    }
                }

                model.controlInfo = controlInfoList;
            }

            return model;
        }

        #endregion 模板详情取得

        #region 新建流程

        /// <summary>
        /// 新建流程
        /// </summary>
        /// <param name="flowModel">流程模型</param>
        /// <param name="flag">1：保存 2：提交</param>
        public ReturnConfirm AddFlow(AddFormContentModel flowModel, int flag)
        {
            var returnUsers = new ReturnConfirm();
            returnUsers.confirmUser=0;
            returnUsers.result=1;
            using (var db = new TargetNavigationDBEntities())
            {
                //1、流程节点表和节点流程表关联查出当前节点
                var flowNode = (from f in db.tblFlowNode
                                join n in db.tblNodeLink on f.nodeId equals n.nodeEntry
                                where f.templateId == flowModel.templateId && f.nodeType.Value == (int)(ConstVar.NodeType.Create)
                                select n).FirstOrDefault();
                if (flowNode == null)
                {
                    returnUsers.result = 0; 
                    return returnUsers;
                } 
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var formId = db.prcGetPrimaryKey("tblUserForm", obj).FirstOrDefault().Value;
                flowModel.currentNode = flowNode.nodeEntry.Value;
                returnUsers.confirmUser=flowModel.createUser;
                //2、向用户表单表插入数据
                var flow = new tblUserForm
                {
                    formId = formId,
                    templateId = flowModel.templateId,
                    organizationId = flowModel.organizationId,
                    stationId = flowModel.stationId,
                    title = flowModel.title,
                    urgency = flowModel.urgency,
                    status = Convert.ToInt32(ConstVar.userFormStatus.unSubmit),
                    currentNode = flowModel.currentNode,
                    archive = false,
                    createUser = flowModel.createUser,
                    createTime = flowModel.createTime,
                    updateUser = flowModel.createUser,
                    updateTime = DateTime.Now,
                    deleteFlag = false
                };
                db.tblUserForm.Add(flow);
                //3、向表单内容表中插入数据
                if (flowModel.controlValue.Count > 0)
                {
                    foreach (var item in flowModel.controlValue)
                    {
                        if (item.rowNumberList.Count > 0)
                        {
                            foreach (var model in item.rowNumberList)
                            {
                                //向表单详细表中插入数据
                                System.Data.Entity.Core.Objects.ObjectParameter ran = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                                var detailId = db.prcGetPrimaryKey("tblFormDetail", ran).FirstOrDefault().Value;
                                var detailModel = new tblFormDetail
                                {
                                    detailId = detailId,
                                    formId = formId,
                                    controlId = item.controlId,
                                    rowNumber = model.rowNumber
                                };
                                db.tblFormDetail.Add(detailModel);
                                if (model.detailValue.Length > 0)
                                {
                                    foreach (var detail in model.detailValue)
                                    {
                                        //向表单内容表中插入数据
                                        var contentModel = new tblFormContent();
                                        if (detail.IndexOf('*') >= 0)
                                        {
                                            var array = detail.Split('*').ToArray();
                                            if (array.Length >= 3 && array[2] == "9" && array[3] == item.controlId)    //上传文件
                                            {
                                                System.Data.Entity.Core.Objects.ObjectParameter content = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                                                var contentId = db.prcGetPrimaryKey("tblFormContent", content).FirstOrDefault().Value;
                                                contentModel.contentId = contentId;
                                                contentModel.detailId = detailId;
                                                contentModel.controlValue = array[0];
                                                contentModel.saveName = array[1];
                                                db.tblFormContent.Add(contentModel);
                                            }
                                        }
                                        else
                                        {
                                            System.Data.Entity.Core.Objects.ObjectParameter content = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                                            var contentId = db.prcGetPrimaryKey("tblFormContent", content).FirstOrDefault().Value;
                                            contentModel.contentId = contentId;
                                            contentModel.detailId = detailId;
                                            contentModel.controlValue = detail;
                                            db.tblFormContent.Add(contentModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
                if (flag == 2)//提交操作
                {
                    returnUsers = this.SubmitFlow(flowModel.templateId, formId, flowNode.nodeEntry.Value, flowModel.createUser);

                    //获取出口节点信息
                    //var nodelOperateModel = this.GetNodeOperateList(db, flowModel.templateId, formId, flowNode.nodeEntry.Value, flowModel.createUser, ConstVar.LinkStatus.Pass).FirstOrDefault();
                    // if (nodelOperateModel!=null)
                    //{
                }
            }
            return returnUsers;
        }

        #endregion 新建流程

        #region 保存控件的值

        /// <summary>
        /// 保存控件的值
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <param name="valueList">控件值集合</param>
        /// <param name="flag">1、新建 2、流程中</param>
        public bool SaveControlValue(int formId, AddFormContentModel formInfo, int flag)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                if (flag == 1)
                {
                    //修改用户表单表中的数据
                    var userForm = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                    if (userForm == null) return false;
                    userForm.title = formInfo.title;
                    userForm.urgency = formInfo.urgency;
                    userForm.organizationId = formInfo.organizationId;
                    userForm.stationId = formInfo.stationId;
                    userForm.createTime = formInfo.createTime;
                    userForm.updateTime = DateTime.Now;
                }
                var valueList = formInfo.controlValue;
                if (valueList.Count <= 0) return true;
                //1、删除表单详细表和内容表中的值
                var controlDataList = db.tblFormDetail.Where(p => p.formId == formId);
                if (controlDataList.Count() > 0)
                {
                    db.tblFormDetail.RemoveRange(controlDataList);
                    if (controlDataList.Count() > 0)
                    {
                        foreach (var control in controlDataList)
                        {
                            var deleteContent = db.tblFormContent.Where(p => p.detailId == control.detailId);
                            if (deleteContent.Count() > 0)
                            {
                                db.tblFormContent.RemoveRange(deleteContent);
                            }
                        }
                    }
                }

                //插入最新的值
                foreach (var item in valueList)
                {
                    if (item.rowNumberList.Count > 0)
                    {
                        foreach (var model in item.rowNumberList)
                        {
                            //向表单详细表中插入数据
                            System.Data.Entity.Core.Objects.ObjectParameter ran = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                            var detailId = db.prcGetPrimaryKey("tblFormDetail", ran).FirstOrDefault().Value;
                            var detailModel = new tblFormDetail
                            {
                                detailId = detailId,
                                formId = formId,
                                controlId = item.controlId,
                                rowNumber = model.rowNumber
                            };
                            db.tblFormDetail.Add(detailModel);
                            if (model.detailValue.Length > 0)
                            {
                                foreach (var detail in model.detailValue)
                                {
                                    //向表单内容表中插入数据
                                    var contentModel = new tblFormContent();
                                    if (detail.IndexOf('*') >= 0)
                                    {
                                        var array = detail.Split('*').ToArray();
                                        if (array.Length >= 3 && array[2] == "9" && array[3] == item.controlId)    //上传文件
                                        {
                                            System.Data.Entity.Core.Objects.ObjectParameter content = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                                            var contentId = db.prcGetPrimaryKey("tblFormContent", content).FirstOrDefault().Value;
                                            contentModel.contentId = contentId;
                                            contentModel.detailId = detailId;
                                            contentModel.controlValue = array[0];
                                            contentModel.saveName = array[1];
                                            db.tblFormContent.Add(contentModel);
                                        }
                                    }
                                    else
                                    {
                                        System.Data.Entity.Core.Objects.ObjectParameter content = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                                        var contentId = db.prcGetPrimaryKey("tblFormContent", content).FirstOrDefault().Value;
                                        contentModel.contentId = contentId;
                                        contentModel.detailId = detailId;
                                        contentModel.controlValue = detail;
                                        db.tblFormContent.Add(contentModel);
                                    }
                                }
                            }
                        }
                    }
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 保存控件的值

        #region 获取待提交或已提交的流程列表

        /// <summary>
        /// 获取待提交或已提交的流程列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">拼接的sql</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <param name="type">流程类别</param>
        /// <param name="status">流程状态：1、待提交 2、已提交</param>
        /// <param name="admin">1、管理员 2、非管理员</param>
        /// <returns>流程列表</returns>
        public List<UserFormModel> GetUnSubmitFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int status, int admin)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (admin == 1 && status == 1)   //管理员且只取待提交的数据
                {
                    flowList = (from u in db.tblUserForm
                                join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                from t in group1.DefaultIfEmpty()
                                join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                from f in group2.DefaultIfEmpty()
                                join user in db.tblUser on u.createUser equals user.userId into group3
                                from user in group3.DefaultIfEmpty()
                                join org in db.tblOrganization on u.organizationId equals org.organizationId into group4
                                from org in group4.DefaultIfEmpty()
                                where !u.deleteFlag && u.status == status && (type == null ? true : (t.categoryId == type))
                                select new UserFormModel
                                {
                                    formId = u.formId,
                                    templateId = u.templateId,
                                    templateName = t.templateName,
                                    organizationId = u.organizationId,
                                    organizationName = org.organizationName,
                                    title = u.title,
                                    defaultTitle = t.defaultTitle,
                                    urgency = u.urgency,
                                    status = u.status,
                                    currentNode = u.currentNode,
                                    nodeName = f.nodeName == null ? "" : f.nodeName,
                                    archive = u.archive,
                                    archiveTime = u.archiveTime,
                                    createUser = u.createUser,
                                    createUserName = user.userName,
                                    admin = user.admin,
                                    createTime = u.createTime,
                                    updateTime = u.updateTime,
                                    img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                    operateStatus = status
                                }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                }
                else           //非管理员
                {
                    flowList = (from u in db.tblUserForm
                                join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                from t in group1.DefaultIfEmpty()
                                join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                from f in group2.DefaultIfEmpty()
                                join user in db.tblUser on u.createUser equals user.userId into group3
                                from user in group3.DefaultIfEmpty()
                                join org in db.tblOrganization on u.organizationId equals org.organizationId into group4
                                from org in group4.DefaultIfEmpty()
                                where !u.deleteFlag && u.createUser == userId && u.status == status && (type == null ? true : (t.categoryId == type))
                                select new UserFormModel
                                {
                                    formId = u.formId,
                                    templateId = u.templateId,
                                    templateName = t.templateName,
                                    organizationId = u.organizationId,
                                    organizationName = org.organizationName,
                                    title = u.title,
                                    defaultTitle = t.defaultTitle,
                                    urgency = u.urgency,
                                    status = u.status,
                                    currentNode = u.currentNode,
                                    nodeName = f.nodeName == null ? "" : f.nodeName,
                                    archive = u.archive,
                                    archiveTime = u.archiveTime,
                                    createUser = u.createUser,
                                    createUserName = user.userName,
                                    admin = user.admin,
                                    createTime = u.createTime,
                                    updateTime = u.updateTime,
                                    img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                    operateStatus = status
                                }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                }

                //获取当前未操作人集合
                if (flowList.Count > 0)
                {
                    if (status == 1)//待提交
                    {
                        foreach (var item in flowList)
                        {
                            item.operate = (from form in db.tblUserForm
                                            join u in db.tblUser on form.createUser equals u.userId
                                            where !u.deleteFlag && !form.deleteFlag && form.formId == item.formId
                                            select new User
                                            {
                                                id = u.userId,
                                                name = u.userName
                                            }).ToList();
                        }
                    }
                    else  //已提交
                    {
                        foreach (var item in flowList)
                        {
                            //item.operate = (from u in db.tblUserForm
                            //                join d in db.tblFormDuplicate on u.formId equals d.formId
                            //                join user in db.tblUser on d.userId equals user.userId
                            //                where (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit)) && d.formId == item.formId && d.nodeId == item.currentNode && !u.deleteFlag.Value
                            //                select new User
                            //                {
                            //                    id = d.userId,
                            //                    name = user.userName
                            //                }).ToList();
                            item.operate = GetOperateUserInfo(db, item);
                            var nodeType = (from n in db.tblNodeLink
                                            join f in db.tblFlowNode on n.nodeEntry equals f.nodeId
                                            where n.nodeExit == item.currentNode
                                            select f.nodeType).FirstOrDefault();
                            if (nodeType != null && nodeType == (int)ConstVar.NodeType.Create)
                            {
                                item.operateStatus = (int)ConstVar.FormFlowStatusId.neverChecked;
                            }
                        }
                    }
                }
            }
            return flowList;
        }

        #endregion 获取待提交或已提交的流程列表

        #region 获取计划日程化我的日程流程列表

        /// <summary>
        /// 获取计划日程化我的日程流程列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">拼接的sql</param>
        /// <param name="start">筛选开始时间</param>
        /// <param name="end">筛选结束时间</param>
        /// <returns>流程列表</returns>
        public List<UserFormModel> GetCalerdarUnSubmitFlowList(int userId, string conditionString, DateTime start, DateTime end)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                flowList = (from u in db.tblUserForm
                            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                            from t in group1.DefaultIfEmpty()
                            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                            from f in group2.DefaultIfEmpty()
                            join user in db.tblUser on u.createUser equals user.userId into group3
                            from user in group3.DefaultIfEmpty()
                            join org in db.tblOrganization on u.organizationId equals org.organizationId into group4
                            from org in group4.DefaultIfEmpty()
                            where !u.deleteFlag && u.createUser == userId && u.status == (int)ConstVar.userFormStatus.unSubmit
                            select new UserFormModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                templateName = t.templateName,
                                organizationId = u.organizationId,
                                organizationName = org.organizationName,
                                title = u.title,
                                defaultTitle = t.defaultTitle,
                                urgency = u.urgency,
                                status = u.status,
                                currentNode = u.currentNode,
                                nodeName = f.nodeName == null ? "" : f.nodeName,
                                archive = u.archive,
                                archiveTime = u.archiveTime,
                                createUser = u.createUser,
                                createUserName = user.userName,
                                admin = user.admin,
                                createTime = u.createTime,
                                updateTime = u.updateTime,
                                img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                operateStatus = (int)ConstVar.userFormStatus.unSubmit
                            }).Where(conditionString).Where("updateTime >=@0 And updateTime <@1", start, end).ToList();

                //获取当前未操作人集合
                if (flowList.Count > 0)
                {
                    flowList.ForEach(p =>
                    {
                        p.operate = (from form in db.tblUserForm
                                     join u in db.tblUser on form.createUser equals u.userId
                                     where !u.deleteFlag && !form.deleteFlag && form.formId == p.formId
                                     select new User
                                     {
                                         id = u.userId,
                                         name = u.userName
                                     }).ToList();
                    });
                }
            }
            return flowList;
        }

        #endregion 获取计划日程化我的日程流程列表

        #region 获取待审批状态列表

        /// <summary>
        /// 获取待审批状态列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程分类</param>
        /// <returns>待处理状态列表</returns>
        public List<UserFormModel> GetUnCheckFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、查询不带委托的待审核列表
                flowList = (from u in db.tblUserForm
                            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                            from t in group1.DefaultIfEmpty()
                            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                            from f in group2.DefaultIfEmpty()
                            join user in db.tblUser on u.createUser equals user.userId into group3
                            from user in group3.DefaultIfEmpty()
                            join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId } into group4
                            from d in group4.DefaultIfEmpty()
                            join operateUser in db.tblUser on d.userId equals operateUser.userId into group5
                            from operateUser in group5.DefaultIfEmpty()
                            join org in db.tblOrganization on u.organizationId equals org.organizationId into group6
                            from org in group6.DefaultIfEmpty()
                            where !u.deleteFlag && d.userId == userId && u.status == (int)(ConstVar.userFormStatus.flowing) && (type == null ? true : (t.categoryId == type)) && (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign))
                            select new UserFormModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                templateName = t.templateName,
                                organizationId = u.organizationId,
                                organizationName = org.organizationName,
                                title = u.title,
                                defaultTitle = t.defaultTitle,
                                urgency = u.urgency,
                                status = u.status,
                                currentNode = u.currentNode,
                                nodeName = f.nodeName == null ? "" : f.nodeName,
                                archive = u.archive,
                                archiveTime = u.archiveTime,
                                createUser = u.createUser,
                                createUserName = user.userName,
                                createTime = u.createTime,
                                updateTime = u.updateTime,
                                img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                alreadyRead = d.alreadyRead.Value,
                                operateStatus = (int)(d.alreadyRead == 1 ? ConstVar.FormFlowStatusId.submitMsg : ConstVar.FormFlowStatusId.unExamine)
                            }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                //筛选出已经有委托操作的流程
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        var entrust = from r in db.tblEntrustResult
                                      join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                      where r.templateId == item.templateId && !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.entrustUser == userId
                                      select e;
                        if (entrust.Count() > 0)
                        {
                            item.operateStatus = (int)ConstVar.FormFlowStatusId.FlowEntrust;
                        }
                    }
                }
                //3、查询委托的待处理列表
                var newFlowList = (from u in db.tblUserForm
                                   join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                   from t in group1.DefaultIfEmpty()
                                   join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                   from f in group2.DefaultIfEmpty()
                                   join user in db.tblUser on u.createUser equals user.userId into group3
                                   from user in group3.DefaultIfEmpty()
                                   join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId } into group4
                                   from d in group4.DefaultIfEmpty()
                                   join r in db.tblEntrustResult on u.templateId equals r.templateId into group5
                                   from r in group5.DefaultIfEmpty()
                                   join e in db.tblFlowEntrust on r.entrustId equals e.entrustId into group6
                                   from e in group6.DefaultIfEmpty()
                                   join mUser in db.tblUser on e.mandataryUser equals mUser.userId into group7
                                   from mUser in group7.DefaultIfEmpty()
                                   join org in db.tblOrganization on u.organizationId equals org.organizationId into group8
                                   from org in group8.DefaultIfEmpty()
                                   where !u.deleteFlag && e.entrustUser == d.userId && e.mandataryUser == userId && u.status == (int)(ConstVar.userFormStatus.flowing) && (type == null ? true : (t.categoryId == type)) && (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign)) && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && !e.deleteFlag
                                   select new UserFormModel
                                   {
                                       formId = u.formId,
                                       templateId = u.templateId,
                                       templateName = t.templateName,
                                       organizationId = u.organizationId,
                                       organizationName = org.organizationName,
                                       title = u.title,
                                       defaultTitle = t.defaultTitle,
                                       urgency = u.urgency,
                                       status = u.status,
                                       currentNode = u.currentNode,
                                       nodeName = f.nodeName == null ? "" : f.nodeName,
                                       archive = u.archive,
                                       archiveTime = u.archiveTime,
                                       createUser = u.createUser,
                                       createUserName = user.userName,
                                       createTime = u.createTime,
                                       updateTime = u.updateTime,
                                       img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                       alreadyRead = d.alreadyRead.Value,
                                       operateStatus = (int)(d.alreadyRead == 1 ? ConstVar.FormFlowStatusId.submitMsg : ConstVar.FormFlowStatusId.unExamine),
                                       isEntruct = 1
                                   }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).Distinct().ToList();
                if (newFlowList.Count > 0)
                {
                    //去除最新数据
                    newFlowList.ForEach(p =>
                    {
                        if (newFlowList.Where(s => s.formId == p.formId).Count() > 1)
                        {
                            if (flowList.Where(a => a.formId == p.formId).Count() <= 0)
                            {
                                flowList.Add(p);
                            }
                        }
                        else
                        {
                            flowList.Add(p);
                        }
                    });
                }

                //获取节点未操作人信息
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        item.operate = GetOperateUserInfo(db, item);
                    }
                }
            }

            return flowList;
        }

        #endregion 获取待审批状态列表

        #region 获取计划日程化待审批状态列表

        /// <summary>
        /// 获取计划日程化待审批状态列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程分类</param>
        /// <returns>待处理状态列表</returns>
        public List<UserFormModel> GetCalendarUnCheckFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //1、查询不带委托的待审核列表
                flowList = (from u in db.tblUserForm
                            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                            from t in group1.DefaultIfEmpty()
                            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                            from f in group2.DefaultIfEmpty()
                            join user in db.tblUser on u.createUser equals user.userId into group3
                            from user in group3.DefaultIfEmpty()
                            join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId } into group4
                            from d in group4.DefaultIfEmpty()
                            join operateUser in db.tblUser on d.userId equals operateUser.userId into group5
                            from operateUser in group5.DefaultIfEmpty()
                            join org in db.tblOrganization on u.organizationId equals org.organizationId into group6
                            from org in group6.DefaultIfEmpty()
                            where !u.deleteFlag && d.userId == userId && u.status == (int)(ConstVar.userFormStatus.flowing) && (type == null ? true : (t.categoryId == type)) && (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign))
                            select new UserFormModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                templateName = t.templateName,
                                organizationId = u.organizationId,
                                organizationName = org.organizationName,
                                title = u.title,
                                defaultTitle = t.defaultTitle,
                                urgency = u.urgency,
                                status = u.status,
                                currentNode = u.currentNode,
                                nodeName = f.nodeName == null ? "" : f.nodeName,
                                archive = u.archive,
                                archiveTime = u.archiveTime,
                                createUser = u.createUser,
                                createUserName = user.userName,
                                createTime = u.createTime,
                                updateTime = u.updateTime,
                                img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                alreadyRead = d.alreadyRead.Value,
                                operateStatus = (int)(d.alreadyRead == 1 ? ConstVar.FormFlowStatusId.submitMsg : ConstVar.FormFlowStatusId.unExamine)
                            }).Where(conditionString).Where("updateTime >=@0 And updateTime <@1", start, end).ToList();
                //筛选出已经有委托操作的流程
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        var entrust = from r in db.tblEntrustResult
                                      join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                      where r.templateId == item.templateId && !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.entrustUser == userId
                                      select e;
                        if (entrust.Count() > 0)
                        {
                            item.operateStatus = (int)ConstVar.FormFlowStatusId.FlowEntrust;
                        }
                    }
                }
                //3、查询委托的待处理列表
                var newFlowList = (from u in db.tblUserForm
                                   join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                   from t in group1.DefaultIfEmpty()
                                   join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                   from f in group2.DefaultIfEmpty()
                                   join user in db.tblUser on u.createUser equals user.userId into group3
                                   from user in group3.DefaultIfEmpty()
                                   join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId } into group4
                                   from d in group4.DefaultIfEmpty()
                                   join r in db.tblEntrustResult on u.templateId equals r.templateId into group5
                                   from r in group5.DefaultIfEmpty()
                                   join e in db.tblFlowEntrust on r.entrustId equals e.entrustId into group6
                                   from e in group6.DefaultIfEmpty()
                                   join mUser in db.tblUser on e.mandataryUser equals mUser.userId into group7
                                   from mUser in group7.DefaultIfEmpty()
                                   join org in db.tblOrganization on u.organizationId equals org.organizationId into group8
                                   from org in group8.DefaultIfEmpty()
                                   where !u.deleteFlag && e.entrustUser == d.userId && e.mandataryUser == userId && u.status == (int)(ConstVar.userFormStatus.flowing) && (type == null ? true : (t.categoryId == type)) && (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign)) && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && !e.deleteFlag
                                   select new UserFormModel
                                   {
                                       formId = u.formId,
                                       templateId = u.templateId,
                                       templateName = t.templateName,
                                       organizationId = u.organizationId,
                                       organizationName = org.organizationName,
                                       title = u.title,
                                       defaultTitle = t.defaultTitle,
                                       urgency = u.urgency,
                                       status = u.status,
                                       currentNode = u.currentNode,
                                       nodeName = f.nodeName == null ? "" : f.nodeName,
                                       archive = u.archive,
                                       archiveTime = u.archiveTime,
                                       createUser = u.createUser,
                                       createUserName = user.userName,
                                       createTime = u.createTime,
                                       updateTime = u.updateTime,
                                       img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                       alreadyRead = d.alreadyRead.Value,
                                       operateStatus = (int)(d.alreadyRead == 1 ? ConstVar.FormFlowStatusId.submitMsg : ConstVar.FormFlowStatusId.unExamine),
                                       isEntruct = 1
                                   }).Where(conditionString).Where("updateTime >=@0 And updateTime <@1", start, end).Distinct().ToList();
                if (newFlowList.Count > 0)
                {
                    //去除最新数据
                    newFlowList.ForEach(p =>
                    {
                        if (newFlowList.Where(s => s.formId == p.formId).Count() > 1)
                        {
                            if (flowList.Where(a => a.formId == p.formId).Count() <= 0)
                            {
                                flowList.Add(p);
                            }
                        }
                        else
                        {
                            flowList.Add(p);
                        }
                    });
                }

                //获取节点未操作人信息
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        item.operate = GetOperateUserInfo(db, item);
                    }
                }
            }

            return flowList;
        }

        #endregion 获取计划日程化待审批状态列表

        #region 获取待查阅状态列表

        /// <summary>
        /// 获取待查阅状态列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程分类</param>
        /// <returns>待处理状态列表</returns>
        public List<UserFormModel> GetUnReadFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //获取待查阅列表
                flowList = (from u in db.tblUserForm
                            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                            from t in group1.DefaultIfEmpty()
                            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                            from f in group2.DefaultIfEmpty()
                            join user in db.tblUser on u.createUser equals user.userId into group3
                            from user in group3.DefaultIfEmpty()
                            join d in db.tblFormDuplicate on u.formId equals d.formId into group4
                            from d in group4.DefaultIfEmpty()
                            join operateUser in db.tblUser on d.userId equals operateUser.userId into group5
                            from operateUser in group5.DefaultIfEmpty()
                            join org in db.tblOrganization on u.organizationId equals org.organizationId into group6
                            from org in group6.DefaultIfEmpty()
                            where !u.deleteFlag && d.userId == userId && (type == null ? true : (t.categoryId == type)) && d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Ready)
                            select new UserFormModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                templateName = t.templateName,
                                organizationId = u.organizationId,
                                organizationName = org.organizationName,
                                title = u.title,
                                defaultTitle = t.defaultTitle,
                                urgency = u.urgency,
                                status = u.status,
                                currentNode = u.currentNode,
                                nodeName = f.nodeName == null ? "" : f.nodeName,
                                archive = u.archive,
                                archiveTime = u.archiveTime,
                                createUser = u.createUser,
                                createUserName = user.userName,
                                createTime = u.createTime,
                                updateTime = u.updateTime,
                                img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                alreadyRead = d.alreadyRead.Value,
                                operateStatus = (int)(ConstVar.FormFlowStatusId.unRead)
                            }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).Distinct().ToList();

                //获取节点未操作人信息
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        item.operate = GetOperateUserInfo(db, item);
                    }
                }
            }

            return flowList;
        }

        #endregion 获取待查阅状态列表

        #region 获取已处理状态列表

        /// <summary>
        /// 获取已处理状态列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程分类</param>
        /// <returns>已处理状态列表</returns>
        public List<UserFormModel> GetCheckedFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type)
        {
            var flowList = new List<UserFormModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                #region 注释的代码

                //不存在委托的已处理列表
                //flowList = (from u in db.tblUserForm
                //            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                //            from t in group1.DefaultIfEmpty()
                //            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                //            from f in group2.DefaultIfEmpty()
                //            join user in db.tblUser on u.createUser equals user.userId into group3
                //            from user in group3.DefaultIfEmpty()
                //            join d in db.tblFormDuplicate on u.formId equals d.formId into group4
                //            from d in group4.DefaultIfEmpty()
                //            where !u.deleteFlag.Value && d.userId == userId && u.status == 2 && t.categoryId == type && d.alreadyRead == 3 && u.currentNode == d.nodeId
                //            select new UserFormModel
                //            {
                //                formId = u.formId,
                //                templateId = u.templateId,
                //                organizationId = u.organizationId,
                //                title = u.title,
                //                urgency = u.urgency,
                //                status = u.status,
                //                currentNode = u.currentNode,
                //                nodeName = f.nodeName,
                //                archive = u.archive,
                //                archiveTime = u.archiveTime,
                //                createUser = u.createUser,
                //                createUserName = user.userName,
                //                createTime = u.createTime,
                //                updateTime = u.updateTime,
                //                img = user.smallImage,
                //                alreadyRead = d.alreadyRead.Value

                //            }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();

                #endregion 注释的代码

                //获取已处理的流程列表(包括委托的情况)
                var newFlowList = (from u in db.tblUserForm
                                   join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                   from t in group1.DefaultIfEmpty()
                                   join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                   from f in group2.DefaultIfEmpty()
                                   join user in db.tblUser on u.createUser equals user.userId into group3
                                   from user in group3.DefaultIfEmpty()
                                   join l in db.tblFormFlow on u.formId equals l.formId into group5
                                   from l in group5.DefaultIfEmpty()
                                   join org in db.tblOrganization on u.organizationId equals org.organizationId into group6
                                   from org in group6.DefaultIfEmpty()
                                   orderby l.createTime descending
                                   where !u.deleteFlag && !l.deleteFlag && u.status == (int)ConstVar.userFormStatus.flowing && (l.entrustUser == userId || l.createUser == userId) && (type == null ? true : (t.categoryId == type)) && (l.result == (int)(ConstVar.FormOperateType.Pass) || l.result == (int)(ConstVar.FormOperateType.Return) || l.result == (int)(ConstVar.FormOperateType.Revoke) || l.result == (int)(ConstVar.FormOperateType.Read))
                                   select new UserFormModel
                                   {
                                       formId = u.formId,
                                       templateId = u.templateId,
                                       templateName = t.templateName,
                                       organizationId = u.organizationId,
                                       organizationName = org.organizationName,
                                       title = u.title,
                                       defaultTitle = t.defaultTitle,
                                       urgency = u.urgency,
                                       status = u.status,
                                       currentNode = u.currentNode,
                                       nodeName = f.nodeName == null ? "" : f.nodeName,
                                       archive = u.archive,
                                       archiveTime = u.archiveTime,
                                       createUser = u.createUser,
                                       createUserName = user.userName,
                                       createTime = u.createTime,
                                       updateTime = u.updateTime,
                                       img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                       operateStatus = (int)(l.result == 2 ? ConstVar.FormFlowStatusId.hasExamine : ConstVar.FormFlowStatusId.hasRead)
                                   }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();

                if (newFlowList.Count > 0)
                {
                    //去除最新数据
                    newFlowList.ForEach(p =>
                    {
                        if (newFlowList.Where(s => s.formId == p.formId).Count() > 1)
                        {
                            if (flowList.Where(a => a.formId == p.formId).Count() <= 0)
                            {
                                flowList.Add(p);
                            }
                        }
                        else
                        {
                            flowList.Add(p);
                        }
                    });
                }
                //获取节点未操作人信息
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        //item.operate = (from u in db.tblUserForm
                        //                join d in db.tblFormDuplicate on u.formId equals d.formId
                        //                join user in db.tblUser on d.userId equals user.userId
                        //                where !u.deleteFlag.Value && d.nodeId == item.currentNode && d.formId == item.formId
                        //                select new User
                        //                {
                        //                    id = d.userId,
                        //                    name = user.userName
                        //                }).ToList();
                        item.operate = GetOperateUserInfo(db, item);
                    }
                }
            }

            #region 注释的代码

            //获取委托人的信息
            //if (newFlowList.Count > 0)
            //{
            //foreach (var item in newFlowList)
            //{
            //    if (flowList.Where(p => p.formId == item.formId).Count() <= 0)
            //    {
            //        flowList.Add(item);
            //    }
            //}
            //    newFlowList.ForEach(p =>
            //    {
            //        p.operate = new List<User>{
            //            new User{
            //                id=userId,
            //                name=db.tblUser.Where(a=>a.userId==userId).FirstOrDefault()==null?"":db.tblUser.Where(a=>a.userId==userId).FirstOrDefault().userName
            //            }
            //        };
            //    });
            //    flowList.AddRange(newFlowList);
            //}
            //if (flowList.Count > 0)
            //{
            //    //排序
            //    flowList = GetFlowListOrderByCustom(sort, flowList);
            //}

            #endregion 注释的代码

            return flowList;
        }

        #endregion 获取已处理状态列表

        #region 获取已办结状态列表

        /// <summary>
        /// 获取已办结状态列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程分类</param>
        /// <param name="admin">1、管理员 2、非管理员</param>
        /// <returns>已办结状态列表</returns>
        public List<UserFormModel> GetCompletedFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int admin)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (admin == 1)   //管理员
                {
                    flowList = (from u in db.tblUserForm
                                join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                from t in group1.DefaultIfEmpty()
                                join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                from f in group2.DefaultIfEmpty()
                                join user in db.tblUser on u.createUser equals user.userId into group3
                                from user in group3.DefaultIfEmpty()
                                join org in db.tblOrganization on u.organizationId equals org.organizationId into group4
                                from org in group4.DefaultIfEmpty()
                                where !u.deleteFlag && u.status == (int)(ConstVar.userFormStatus.hasCompleted) && (type == null ? true : (t.categoryId == type))
                                select new UserFormModel
                                {
                                    formId = u.formId,
                                    templateId = u.templateId,
                                    templateName = t.templateName,
                                    organizationId = u.organizationId,
                                    organizationName = org.organizationName,
                                    title = u.title,
                                    defaultTitle = t.defaultTitle,
                                    urgency = u.urgency,
                                    status = u.status,
                                    currentNode = u.currentNode,
                                    nodeName = f.nodeName == null ? "" : f.nodeName,
                                    archive = u.archive,
                                    archiveTime = u.archiveTime,
                                    createUser = u.createUser,
                                    createUserName = user.userName,
                                    createTime = u.createTime,
                                    updateTime = u.updateTime,
                                    img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                    alreadyRead = u.status.Value,
                                    operateStatus = (int)(ConstVar.FormFlowStatusId.hasCompleted)
                                }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                }
                else         //非管理员
                {
                    //1、查询登录用户创建的已办结列表
                    flowList = (from u in db.tblUserForm
                                join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                from t in group1.DefaultIfEmpty()
                                join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                from f in group2.DefaultIfEmpty()
                                join user in db.tblUser on u.createUser equals user.userId into group3
                                from user in group3.DefaultIfEmpty()
                                join org in db.tblOrganization on u.organizationId equals org.organizationId into group4
                                from org in group4.DefaultIfEmpty()
                                where !u.deleteFlag && u.createUser == userId && u.status == (int)(ConstVar.userFormStatus.hasCompleted) && (type == null ? true : (t.categoryId == type))
                                select new UserFormModel
                                {
                                    formId = u.formId,
                                    templateId = u.templateId,
                                    templateName = t.templateName,
                                    organizationId = u.organizationId,
                                    organizationName = org.organizationName,
                                    title = u.title,
                                    defaultTitle = t.defaultTitle,
                                    urgency = u.urgency,
                                    status = u.status,
                                    currentNode = u.currentNode,
                                    nodeName = f.nodeName == null ? "" : f.nodeName,
                                    archive = u.archive,
                                    archiveTime = u.archiveTime,
                                    createUser = u.createUser,
                                    createUserName = user.userName,
                                    createTime = u.createTime,
                                    updateTime = u.updateTime,
                                    img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                    alreadyRead = u.status.Value,
                                    operateStatus = (int)(ConstVar.FormFlowStatusId.hasCompleted)
                                }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();
                    //2、查询被委托人是登录用户或者操作人是登录用户的已办结列表
                    var entrustList = (from u in db.tblUserForm
                                       join t in db.tblTemplate on u.templateId equals t.templateId into group1
                                       from t in group1.DefaultIfEmpty()
                                       join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                                       from f in group2.DefaultIfEmpty()
                                       join user in db.tblUser on u.createUser equals user.userId into group3
                                       from user in group3.DefaultIfEmpty()
                                       join l in db.tblFormFlow on u.formId equals l.formId into group4
                                       from l in group4.DefaultIfEmpty()
                                       join org in db.tblOrganization on u.organizationId equals org.organizationId into group5
                                       from org in group5.DefaultIfEmpty()
                                       where !u.deleteFlag && !l.deleteFlag && (l.entrustUser == userId || l.createUser == userId) && u.status == (int)(ConstVar.userFormStatus.hasCompleted) && (type == null ? true : (t.categoryId == type))
                                       select new UserFormModel
                                       {
                                           formId = u.formId,
                                           templateId = u.templateId,
                                           templateName = t.templateName,
                                           organizationId = u.organizationId,
                                           organizationName = org.organizationName,
                                           title = u.title,
                                           defaultTitle = t.defaultTitle,
                                           urgency = u.urgency,
                                           status = u.status,
                                           currentNode = u.currentNode,
                                           nodeName = f.nodeName == null ? "" : f.nodeName,
                                           archive = u.archive,
                                           archiveTime = u.archiveTime,
                                           createUser = u.createUser,
                                           createUserName = user.userName,
                                           createTime = u.createTime,
                                           updateTime = u.updateTime,
                                           img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                           alreadyRead = u.status.Value,
                                           operateStatus = (int)(ConstVar.FormFlowStatusId.hasCompleted)
                                       }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).ToList();

                    //去除重复项
                    if (entrustList.Count > 0)
                    {
                        entrustList.ForEach(p =>
                        {
                            if (flowList.Where(a => a.formId == p.formId).Count() <= 0)
                            {
                                flowList.Add(p);
                            }
                        });
                    }
                }
            }
            return flowList;
        }

        #endregion 获取已办结状态列表

        #region 管理员：获取流程中的列表

        /// <summary>
        /// 管理员：获取流程中的列表
        /// </summary>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">类型</param>
        /// <returns>流程中的列表</returns>
        public List<UserFormModel> GetFlowingUserFormList(string conditionString, DateTime start, DateTime end, int? type)
        {
            var flowList = new List<UserFormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                flowList = (from u in db.tblUserForm
                            join t in db.tblTemplate on u.templateId equals t.templateId into group1
                            from t in group1.DefaultIfEmpty()
                            join f in db.tblFlowNode on u.currentNode equals f.nodeId into group2
                            from f in group2.DefaultIfEmpty()
                            join user in db.tblUser on u.createUser equals user.userId into group3
                            from user in group3.DefaultIfEmpty()
                            join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId } into group4
                            from d in group4.DefaultIfEmpty()
                            join operateUser in db.tblUser on d.userId equals operateUser.userId into group5
                            from operateUser in group5.DefaultIfEmpty()
                            join org in db.tblOrganization on u.organizationId equals org.organizationId into group6
                            from org in group6.DefaultIfEmpty()
                            where !u.deleteFlag && u.status == (int)(ConstVar.userFormStatus.flowing) && (type == null ? true : (t.categoryId == type))
                            select new UserFormModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                templateName = t.templateName,
                                organizationId = u.organizationId,
                                organizationName = org.organizationName,
                                title = u.title,
                                defaultTitle = t.defaultTitle,
                                urgency = u.urgency,
                                status = u.status,
                                currentNode = u.currentNode,
                                nodeName = f.nodeName == null ? "" : f.nodeName,
                                archive = u.archive,
                                archiveTime = u.archiveTime,
                                createUser = u.createUser,
                                createUserName = user.userName,
                                createTime = u.createTime,
                                updateTime = u.updateTime,
                                img = (user.bigImage == null || user.bigImage == "") ? "../../Images/common/portrait.png" : ("/HeadImage/" + user.bigImage),
                                operateStatus = (int)ConstVar.FormFlowStatusId.flowing
                            }).Where(conditionString).Where("createTime >=@0 And createTime <@1", start, end).Distinct().ToList();
                //if (newFlowList.Count>0)
                //{
                //    newFlowList.ForEach(p => {
                //        if (newFlowList.Where(a=>a.formId==p.formId).Count()<=1)
                //        {
                //            flowList.Add(p);
                //        }
                //        else
                //        {
                //        }
                //    });
                //}
                if (flowList.Count > 0)
                {
                    foreach (var item in flowList)
                    {
                        //item.operate = (from u in db.tblUserForm
                        //                join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId }
                        //                join user in db.tblUser on d.userId equals user.userId
                        //                where (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit)) && d.formId == item.formId && !u.deleteFlag.Value
                        //                select new User
                        //                {
                        //                    id = d.userId,
                        //                    name = user.userName
                        //                }).ToList();
                        item.operate = GetOperateUserInfo(db, item);
                    }
                }
            }
            return flowList;
        }

        #endregion 管理员：获取流程中的列表

        #region 获取所有状态的流程首页列表

        /// <summary>
        /// 获取所有状态的流程首页列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="conditionString">筛选条件</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="type">流程类别</param>
        /// <param name="admin">1、管理员 2、非管理员</param>
        /// <returns>流程列表</returns>
        public List<UserFormModel> GetAllFlowList(int userId, string conditionString, DateTime start, DateTime end, int? type, int admin)
        {
            var flowList = new List<UserFormModel>();
            //待提交的列表
            var unSubmitFlowList = GetUnSubmitFlowList(userId, conditionString, start, end, type, (int)ConstVar.FormFlowStatusId.unSubmit, admin);
            if (unSubmitFlowList.Count > 0) flowList.AddRange(unSubmitFlowList);
            if (admin == 1) //管理员
            {
                var flowingList = this.GetFlowingUserFormList(conditionString, start, end, type);
                if (flowingList.Count > 0) flowList.AddRange(flowingList);
            }
            else
            {
                //已提交的列表
                var hasSubmitFlowList = GetUnSubmitFlowList(userId, conditionString, start, end, type, (int)ConstVar.FormFlowStatusId.hasSubmited, admin);
                if (hasSubmitFlowList.Count > 0) flowList.AddRange(hasSubmitFlowList);
                //待审批的列表
                var uncheckedFlowList = GetUnCheckFlowList(userId, conditionString, start, end, type);
                if (uncheckedFlowList.Count > 0) flowList.AddRange(uncheckedFlowList);
                //待查阅的列表
                var unReadFlowList = GetUnReadFlowList(userId, conditionString, start, end, type);
                if (unReadFlowList.Count > 0) flowList.AddRange(unReadFlowList);
                //已处理的列表
                var hasCheckedList = GetCheckedFlowList(userId, conditionString, start, end, type);
                if (hasCheckedList.Count > 0) flowList.AddRange(hasCheckedList);
            }

            //已办结的列表
            var hasCompleteFlowList = GetCompletedFlowList(userId, conditionString, start, end, type, admin);
            if (hasCompleteFlowList.Count > 0) flowList.AddRange(hasCompleteFlowList);
            //去除重复项
            //if (flowList.Count > 0)
            //{
            //    flowList.ForEach(p =>
            //    {
            //        if (flowList.Where(a => a.formId == p.formId).Count() <= 1)
            //        {
            //            lasFlowList.Add(p);
            //        }
            //    });
            //}
            return flowList;
        }

        #endregion 获取所有状态的流程首页列表

        #region 查询常用联系人

        /// <summary>
        /// 根据用户Id查询上级，自己以及下属的用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>人员列表</returns>
        public List<UserInfo> GetUserIdListByUserId(int userId)
        {
            var userList = new List<UserInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                //用户自己的信息
                var userModel = new UserInfo();
                userModel.id = userId;
                userModel.name = db.tblUser.Where(p => p.userId == userId && !p.deleteFlag && p.workStatus == 1).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId && !p.deleteFlag && p.workStatus == 1).FirstOrDefault().userName;
                userModel.img = db.tblUser.Where(p => p.userId == userId && !p.deleteFlag && p.workStatus == 1).FirstOrDefault() == null ? "" : db.tblUser.Where(p => p.userId == userId && !p.deleteFlag && p.workStatus == 1).FirstOrDefault().smallImage;
                //查询自己的职位Id
                var meStation = db.tblUserStation.Where(p => p.userId == userId).FirstOrDefault();
                var stationId = meStation == null ? 0 : meStation.stationId;
                //查询直接上级信息
                var parentUser = new UserInfo();
                //查询上级职位Id
                var parentStationId = db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault() == null ? 0 : db.tblStation.Where(p => p.stationId == stationId && !p.deleteFlag).FirstOrDefault().parentStation;
                parentUser.id = db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault() == null ? 0 : db.tblUserStation.Where(p => p.stationId == parentStationId).FirstOrDefault().userId;
                parentUser.name = db.tblUser.Where(p => p.userId == parentUser.id && !p.deleteFlag && p.workStatus == 1).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == parentUser.id && !p.deleteFlag && p.workStatus == 1).FirstOrDefault().userName;
                parentUser.img = db.tblUser.Where(p => p.userId == parentUser.id && !p.deleteFlag && p.workStatus == 1).FirstOrDefault() == null ? "无" : db.tblUser.Where(p => p.userId == parentUser.id && !p.deleteFlag && p.workStatus == 1).FirstOrDefault().smallImage;
                if (parentUser != null)
                {
                    userList.Add(parentUser);
                }
                userList.Add(userModel);
                //获取下属用户信息
                var underStationIds = db.tblStation.Where(p => p.parentStation == stationId && !p.deleteFlag);
                if (underStationIds.Count() > 0)
                {
                    foreach (var item in underStationIds)
                    {
                        var underUsers = from user in db.tblUser
                                         join userstation in db.tblUserStation on user.userId equals userstation.userId
                                         where userstation.stationId == item.stationId && !user.deleteFlag && user.workStatus == 1
                                         select new UserInfo
                                         {
                                             id = user.userId,
                                             name = user.userName,
                                             img = user.smallImage
                                         };
                        foreach (var user in underUsers)
                        {
                            userList.Add(user);
                        }
                    }
                }
            }
            var path = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();
            userList.ForEach(p =>
            {
                p.img = string.IsNullOrEmpty(p.img) ? "/Images/common/portrait.png" : "/" + path + "/" + p.img;
            });

            return userList;
        }

        #endregion 查询常用联系人

        #region 人员模糊查询

        /// <summary>
        /// 输入任意数字进行模糊查询
        /// </summary>
        /// <param name="word">筛选字段</param>
        /// <param name="hasImage">是否需要返回头像</param>
        /// <returns></returns>
        public List<UserInfo> SelectUserList(string word, bool hasImage)
        {
            string path = ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString();
            TargetNavigationDBEntities db = new TargetNavigationDBEntities();
            var userList = new List<UserInfo>();
            userList = (from us in db.tblUser
                        where us.userName.IndexOf(word) != -1 && !us.deleteFlag && us.workStatus == 1
                        select new UserInfo
                        {
                            id = us.userId,
                            name = us.userName,
                            img = string.IsNullOrEmpty(us.smallImage) ? "/Images/common/portrait.png" : "/" + path + "/" + us.smallImage
                        }).ToList();

            return userList;
        }

        #endregion 人员模糊查询

        #region 获取流程详情

        /// <summary>
        /// 获取流程详情
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <returns>流程详情模型</returns>
        public FormDetailModel GetFlowDetailListById(int formId, int nodeId)
        {
            var formModel = new FormDetailModel();
            using (var db = new TargetNavigationDBEntities())
            {
                formModel = (from u in db.tblUserForm
                             join t in db.tblTemplate on u.templateId equals t.templateId into group1
                             from t in group1.DefaultIfEmpty()
                             join user in db.tblUser on u.createUser equals user.userId into group2
                             from user in group2.DefaultIfEmpty()
                             join o in db.tblOrganization on u.organizationId equals o.organizationId into group3
                             from o in group3.DefaultIfEmpty()
                             join s in db.tblStation on u.stationId equals s.stationId into group4
                             from s in group4.DefaultIfEmpty()
                             where !u.deleteFlag && u.formId == formId
                             select new FormDetailModel
                             {
                                 templateId = u.templateId,
                                 templateName = t.templateName,
                                 organizationId = u.organizationId,
                                 organizationName = o.organizationName,
                                 stationId = s.stationId,
                                 stationName = s.stationName,
                                 defaultTitle = t.defaultTitle,
                                 title = u.title,
                                 urgency = u.urgency.Value,
                                 status = u.status.Value,
                                 createUser = u.createUser,
                                 createUserName = user.userName,
                                 createTime = u.createTime
                             }).FirstOrDefault();
                if (formModel != null)
                {
                    var controlValueList = (from f in db.tblFormDetail
                                            join t in db.tblTemplateControl on f.controlId equals t.controlId into group1
                                            from t in group1.DefaultIfEmpty()
                                            join n in db.tblNodeField on f.controlId equals n.controlId into group2
                                            from n in group2.DefaultIfEmpty()
                                            where f.formId == formId && t.templateId == formModel.templateId && n.nodeId == nodeId && n.status != 0
                                            select new ControlNewModel
                                            {
                                                detailId = f.detailId,
                                                controlId = f.controlId,
                                                parentControl = t.parentControl,
                                                controlType = t.controlType,
                                                require = t.require,
                                                title = t.title,
                                                description = t.description,
                                                status = n.status,
                                                rowNumber = f.rowNumber
                                            }).ToList();
                    //添加控件值
                    if (controlValueList.Count > 0)
                    {
                        foreach (var item in controlValueList)
                        {
                            item.detailValue = (from c in db.tblFormContent
                                                where c.detailId == item.detailId
                                                select c.controlValue).ToArray();
                            item.saveName = (from c in db.tblFormContent
                                             where c.detailId == item.detailId
                                             select c.saveName).ToArray();
                        }
                        formModel.controlValue = new List<ControlNewModel>();
                        formModel.controlValue.AddRange(controlValueList);
                    }
                }
            }
            return formModel;
        }

        #endregion 获取流程详情

        #region 退回操作

        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="nodeId">当前节点</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="suggest">意见</param>
        /// <param name="userId">用户Id</param>
        /// <param name="isEntruct">是否委托：1、是委托 0、不是委托</param>
        public bool TurnBack(int nodeId, int templateId, int formId, string suggest, int userId, int isEntruct)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var entructUser = 0;
                if (isEntruct == 1)  //委托的情况
                {
                    var thisUserList = (from r in db.tblEntrustResult
                                        join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                        where r.templateId == templateId && !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.mandataryUser == userId
                                        select e.entrustUser).ToList();
                    if (thisUserList.Count > 0)
                    {
                        foreach (var item in thisUserList)
                        {
                            var duplicateModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == item && p.nodeId == nodeId).FirstOrDefault();
                            if (duplicateModel != null) entructUser = duplicateModel.userId;
                        }
                    }
                }
                //1、找到出口节点
                var exitNode = this.GetNodeOperateId(db, templateId, formId, nodeId, ConstVar.LinkStatus.Deny, userId);
                if (exitNode == 0) return false;
                //找出该节点的类型
                var nodeFlow = db.tblFlowNode.Where(p => p.nodeId == exitNode && p.templateId == templateId).FirstOrDefault();
                if (nodeFlow == null) return false;
                var nodeType = nodeFlow.nodeType;
                var status = nodeType == Convert.ToInt32(ConstVar.NodeType.Create) ? Convert.ToInt32(ConstVar.userFormStatus.unSubmit) : ((nodeType == Convert.ToInt32(ConstVar.NodeType.Submit) || nodeType == Convert.ToInt32(ConstVar.NodeType.Approval)) ? Convert.ToInt32(ConstVar.userFormStatus.flowing) : Convert.ToInt32(ConstVar.userFormStatus.hasCompleted));
                //2、更新用户表单
                this.UpdateUserForm(db, exitNode, formId, status, userId);
                //3、删除表单抄送表退回前节点的数据
                if (nodeType == (int)ConstVar.NodeType.Create)
                {
                    var deleList = db.tblFormDuplicate.Where(p => p.formId == formId);
                    db.tblFormDuplicate.RemoveRange(deleList);
                    var formModel = db.tblUserForm.Where(p => p.formId == formId).FirstOrDefault();
                    if (formModel == null) return false;
                    var createUser = formModel.createUser;
                    AddFormDuplicate(db, new List<NodeOperateModel> { new NodeOperateModel { userIds = new int?[] { createUser } } }, formId, exitNode);
                }
                else
                {
                    var formDuplicateModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == exitNode).OrderBy(a => a.createTime).FirstOrDefault();  //该节点的最早创建时间
                    if (formDuplicateModel != null)
                    {
                        var minCreateTime = formDuplicateModel.createTime;
                        var deleteList = db.tblFormDuplicate.Where(p => p.formId == formId && p.createTime >= minCreateTime);
                        if (deleteList.Count() > 0)
                        {
                            db.tblFormDuplicate.RemoveRange(deleteList);
                        }
                    }
                    //4、向表单抄送表中插入最新的数据
                    var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                    if (formModel == null) return false;
                    var nodeOperateList = this.GetNextOperateUserList(db, exitNode, formId, formModel.createUser);
                    if (nodeOperateList.Count <= 0) return false;
                    if (!checkNodeOperateList(nodeOperateList)) return false;
                    AddFormDuplicate(db, nodeOperateList, formId, exitNode);
                }
                //5、向表单流程表中插入数据
                AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Return), suggest, userId, entructUser);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 退回操作

        #region 撤回提交

        /// <summary>
        /// 撤回提交
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>true：操作成功 false：操作失败</returns>
        public bool CancelSubmit(int templateId, int formId, int nodeId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //1、找到创建节点，更新用户表单表
                var createNode = (from f in db.tblFlowNode
                                  where f.nodeType == (int)ConstVar.NodeType.Create && f.templateId == templateId
                                  select f.nodeId).FirstOrDefault();
                var userForm = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (userForm == null) return false;
                userForm.currentNode = createNode;
                userForm.status = (int)ConstVar.userFormStatus.unSubmit;
                userForm.updateTime = DateTime.Now;
                //2、删除表单抄送表中的所有数据
                var dupList = db.tblFormDuplicate.Where(p => p.formId == formId);
                if (dupList.Count() > 0)
                {
                    db.tblFormDuplicate.RemoveRange(dupList);
                }

                //抄送表中插入一条提交的记录
                this.AddFormDuplicate(db, new List<NodeOperateModel> { new NodeOperateModel { userIds = new int?[] { userId } } }, formId, createNode);
                //3、删除表单流程表中的所有数据
                var flowList = db.tblFormFlow.Where(p => p.formId == formId && !p.deleteFlag);
                if (flowList.Count() > 0)
                {
                    foreach (var item in flowList)
                    {
                        item.deleteFlag = true;
                    }
                }
                //4、添加一条撤销的流程记录
                AddUserFormFlow(db, formId, nodeId, (int)ConstVar.FormOperateType.Revoke, string.Empty, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 撤回提交

        #region 撤回操作

        /// <summary>
        /// 撤回操作
        /// </summary>
        /// <param name="nodeId">节点Id</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="userId">用户Id</param>
        public bool BackFirstNode(int nodeId, int templateId, int formId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //1、找到表单流程的初始节点
                var firstNodeModel = db.tblFlowNode.Where(p => p.templateId == templateId && p.nodeType == (int)ConstVar.NodeType.Create).FirstOrDefault();
                if (firstNodeModel == null) return false;
                var firstNode = firstNodeModel.nodeId;
                //2、更新用户表单表
                UpdateUserForm(db, firstNode, formId, (int)ConstVar.userFormStatus.unSubmit, userId);
                //3、删除表单抄送表中的所有该表单的数据
                var duplicateList = db.tblFormDuplicate.Where(p => p.formId == formId);
                if (duplicateList.Count() > 0)
                {
                    db.tblFormDuplicate.RemoveRange(duplicateList);
                }
                //抄送表中插入一条提交的记录
                var userForm = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (userForm == null) return false;
                var createUser = userForm.createUser;
                this.AddFormDuplicate(db, new List<NodeOperateModel> { new NodeOperateModel { userIds = new int?[] { createUser } } }, formId, firstNode);
                //4、删除表单流程表中该表单的所有数据
                var flowList = db.tblFormFlow.Where(p => p.formId == formId && !p.deleteFlag);
                if (flowList.Count() > 0)
                {
                    foreach (var item in flowList)
                    {
                        item.deleteFlag = true;
                    }
                    //db.tblFormFlow.RemoveRange(flowList);
                }
                //5、向表单流程表中插入数据
                AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Revoke), string.Empty, userId);
                db.SaveChanges();
            }
            return true;
        }

        #endregion 撤回操作

        #region 表单抄送

        /// <summary>
        /// 表单抄送
        /// </summary>
        /// <param name="duplicateId">被抄送人Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="userId">用户Id</param>
        public bool DuplicateForm(int[] duplicateId, int formId, int nodeId, int templateId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in duplicateId)
                {
                    var formDup = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == item && p.nodeId == nodeId).FirstOrDefault();
                    if (formDup != null) return false;
                    var formDuplicateModel = new tblFormDuplicate
                    {
                        formId = formId,
                        userId = item,
                        nodeId = nodeId,
                        alreadyRead = Convert.ToInt32(ConstVar.FormDuplicateStatus.Ready),
                        createTime = DateTime.Now
                    };
                    db.tblFormDuplicate.Add(formDuplicateModel);
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 表单抄送

        #region 被抄送人填写意见

        /// <summary>
        /// 被抄送人填写意见
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="contents">意见</param>
        /// <param name="userId">用户Id</param>
        public void AddContents(int formId, int nodeId, string contents, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var dupModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == userId && p.alreadyRead == (int)ConstVar.FormDuplicateStatus.Ready).FirstOrDefault();
                if (dupModel != null)
                {
                    db.tblFormDuplicate.Remove(dupModel);
                }
                //表单流程表中插入数据
                var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formModel != null)
                {
                    //if (formModel.status == (int)ConstVar.userFormStatus.hasCompleted)
                    //{
                    //    AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Archive), contents, userId);
                    //}
                    //else
                    //{
                    AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Read), contents, userId);
                    // }
                }
                db.SaveChanges();
            }
        }

        #endregion 被抄送人填写意见

        #region 同意操作

        /// <summary>
        /// 同意操作
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="suggest">意见</param>
        /// <param name="isEtruct">是否委托：1、是委托 0、不是委托</param>
        /// <param name="userId">用户Id</param>
        public bool AgreeFormFlow(int templateId, int formId, int nodeId, string suggest, int userId, int isEtruct)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var entructUser = 0;
                if (isEtruct == 1)  //委托的情况
                {
                    var thisUserList = (from r in db.tblEntrustResult
                                        join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                        where r.templateId == templateId && !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.mandataryUser == userId
                                        select e.entrustUser).ToList();
                    if (thisUserList.Count > 0)
                    {
                        foreach (var item in thisUserList)
                        {
                            var duplicateModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == item && p.nodeId == nodeId).FirstOrDefault();
                            if (duplicateModel != null) entructUser = duplicateModel.userId;
                        }
                    }
                }
                var operateUsers = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId).Select(a => a.userId).ToArray();
                var operateType = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId).FirstOrDefault();
                if (operateType != null && operateType.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign && operateUsers.Length > 1)  //会签的情况，该节点还需要其他人的审核
                {
                    var userFormModel = db.tblUserForm.Where(p => p.formId == formId).FirstOrDefault();
                    if (userFormModel == null) return false;
                    userFormModel.updateUser = userId;
                    userFormModel.updateTime = DateTime.Now;
                    if (isEtruct == 1)  //委托的情况
                    {
                        //删除表单抄送表中的数据
                        var duplicateModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == entructUser && p.nodeId == nodeId).FirstOrDefault();
                        if (duplicateModel != null) db.tblFormDuplicate.Remove(duplicateModel);
                    }
                    else
                    {
                        //删除抄送表中的这条记录
                        var duplicateModel = db.tblFormDuplicate.Where(p => p.formId == formId && p.userId == userId && p.nodeId == nodeId).FirstOrDefault();
                        if (duplicateModel == null) return false;
                        db.tblFormDuplicate.Remove(duplicateModel);
                    }
                    //4、表单流程表中插入数据
                    AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Pass), suggest, userId, entructUser);
                }
                else
                {
                    if (operateType != null && operateType.alreadyRead == (int)ConstVar.FormDuplicateStatus.Approval && operateUsers.Length > 1) //审批节点，但是有多个人审批的情况
                    {
                        if (isEtruct == 1)  //委托的情况
                        {
                            var readList = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId != entructUser);
                            foreach (var duplicate in readList)
                            {
                                duplicate.alreadyRead = (int)ConstVar.FormDuplicateStatus.Ready;
                            }
                        }
                        else
                        {
                            var duplicateList = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId != userId);
                            foreach (var duplicate in duplicateList)
                            {
                                duplicate.alreadyRead = (int)ConstVar.FormDuplicateStatus.Ready;
                            }
                        }
                    }
                    //1、从节点流程表中找出出口节点,状态通过
                    var exitNode = this.GetNodeOperateId(db, templateId, formId, nodeId, ConstVar.LinkStatus.Pass, userId);
                    if (exitNode == 0) return false;
                    //找出该节点的类型
                    var nodeType = db.tblFlowNode.Where(p => p.nodeId == exitNode && p.templateId == templateId).FirstOrDefault().nodeType;
                    var status = nodeType == (int)(ConstVar.NodeType.Create) ? (int)(ConstVar.userFormStatus.unSubmit) : ((nodeType == (int)(ConstVar.NodeType.Submit) || nodeType == (int)(ConstVar.NodeType.Approval)) ? (int)(ConstVar.userFormStatus.flowing) : (int)(ConstVar.userFormStatus.hasCompleted));
                    //2、更新用户表单
                    this.UpdateUserForm(db, exitNode, formId, status, userId);
                    //3、表单流程表中插入同意数据
                    AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Pass), suggest, userId, entructUser);
                    //4、表单抄送表中插入数据
                    var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                    if (formModel == null) return false;
                    var nodeOperateList = this.GetNextOperateUserList(db, exitNode, formId, formModel.createUser);
                    if (nodeOperateList.Count <= 0) return false;
                    if (!checkNodeOperateList(nodeOperateList)) return false;
                    if (nodeType == (int)ConstVar.NodeStatus.Archive)  //下一节点类型为归档,流程表中插入归档数据
                    {
                        foreach (var nodeOperate in nodeOperateList)
                        {
                            if (nodeOperate.countersign == (int)ConstVar.NodeStatus.Archive) //操作人类型：归档
                            {
                                foreach (var user in nodeOperate.userIds)
                                {
                                    AddUserFormFlow(db, formId, exitNode, Convert.ToInt32(ConstVar.FormOperateType.Archive), string.Empty, user.Value);
                                }
                            }
                            else        //操作人类型：抄送
                            {
                                AddFormDuplicate(db, new List<NodeOperateModel> { nodeOperate }, formId, exitNode);
                            }
                        }

                        //foreach (var nodeOperate in nodeOperateList)
                        //{
                        //    if (nodeOperate.userIds.Length > 0)
                        //    {
                        //        foreach (var user in nodeOperate.userIds)
                        //        {
                        //            AddUserFormFlow(db, formId, exitNode, Convert.ToInt32(ConstVar.FormOperateType.Archive), string.Empty, user.Value);
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        AddFormDuplicate(db, nodeOperateList, formId, exitNode);
                    }
                }

                db.SaveChanges();
            }
            return true;
        }

        #endregion 同意操作

        #region 提交操作

        /// <summary>
        /// 提交操作
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="userId">用户Id</param>
        public ReturnConfirm SubmitFlow(int templateId, int formId, int nodeId, int userId)
        {
            var opreate = new ReturnConfirm();
            opreate.result = 1;
            using (var db = new TargetNavigationDBEntities())
            {
                //1、从节点流程表中找出出口节点,状态通过
                var exitNode = this.GetNodeOperateId(db, templateId, formId, nodeId, ConstVar.LinkStatus.Pass, userId);
                if (exitNode == 0)
                {
                    opreate.result = 0;
                    return opreate;
                }
                //找出该节点的类型
                var nodeType = db.tblFlowNode.Where(p => p.nodeId == exitNode && p.templateId == templateId).FirstOrDefault().nodeType;
                var status = nodeType == Convert.ToInt32(ConstVar.NodeType.Create) ? Convert.ToInt32(ConstVar.userFormStatus.unSubmit) : ((nodeType == Convert.ToInt32(ConstVar.NodeType.Submit) || nodeType == Convert.ToInt32(ConstVar.NodeType.Approval)) ? Convert.ToInt32(ConstVar.userFormStatus.flowing) : Convert.ToInt32(ConstVar.userFormStatus.hasCompleted));
                //2、更新用户表单
                this.UpdateUserForm(db, exitNode, formId, status, userId);
                //3、表单抄送表中插入数据
                var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formModel == null)
                {
                    opreate.result = 0;
                    return opreate;
                }
                var nodeOperateList = this.GetNextOperateUserList(db, exitNode, formId, formModel.createUser);
                if (nodeOperateList.Count <= 0)
                {
                    opreate.result = 0;
                    return opreate;
                }
                if (!checkNodeOperateList(nodeOperateList)) {
                    opreate.result = 0;
                    return opreate;
                } 
               
                foreach (var item in nodeOperateList)
                {
                    if (item.userIds != null && item.userIds.Length > 0)
                    {
                        foreach (var user in item.userIds)
                        {
                            opreate.opreateUser.Add(user.Value);
                            
                        }
                    }
                }
                AddFormDuplicate(db, nodeOperateList, formId, exitNode);

                //4、表单流程表中插入数据
                AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Submit), string.Empty, userId);
                db.SaveChanges();
            }
            return opreate;
        }

        #endregion 提交操作

        #region 删除操作

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public bool deleteUserForm(int formId, int userId)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formModel == null) return false;
                formModel.deleteFlag = true;
                formModel.updateUser = userId;
                formModel.updateTime = DateTime.Now;

                //var dupList = db.tblFormDuplicate.Where(p => p.formId == formId);
                //if (dupList.Count()>0)
                //{
                //    db.tblFormDuplicate.RemoveRange(dupList);
                //}
                //var flowList = db.tblFormFlow.Where(p => p.formId == formId);
                //if (flowList.Count() > 0)
                //{
                //    db.tblFormFlow.RemoveRange(flowList);
                //}
                db.SaveChanges();
            }
            return true;
        }

        #endregion 删除操作

        #region 流程中的提交操作

        /// <summary>
        /// 流程中的提交操作
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="suggest">意见</param>
        /// <param name="userId">用户Id</param>
        /// <param name="isEntruct">是否委托：1、是委托 2、不是委托</param>
        public bool SubmitInFlow(int templateId, int formId, int nodeId, string suggest, int userId, int isEntruct)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var entructUser = 0;
                var operateUsers = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId).Select(a => a.userId).ToArray();
                if (isEntruct == 1)  //委托的情况
                {
                    var thisUserList = (from r in db.tblEntrustResult
                                        join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                        where r.templateId == templateId && !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.mandataryUser == userId
                                        select e.entrustUser).ToList();
                    if (thisUserList.Count > 0)
                    {
                        foreach (var item in thisUserList)
                        {
                            var duplicateList = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId == item);
                            if (duplicateList.Count() > 0)
                            {
                                entructUser = item;
                            }
                        }
                    }
                }
                if (operateUsers.Length > 1)  //多个人提交的情况
                {
                    if (isEntruct == 1)  //委托的情况
                    {
                        var duplicateList = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId != entructUser);
                        foreach (var duplicate in duplicateList)
                        {
                            duplicate.alreadyRead = (int)ConstVar.FormDuplicateStatus.Ready;
                        }
                    }
                    else
                    {
                        var duplicateList = db.tblFormDuplicate.Where(p => p.formId == formId && p.nodeId == nodeId && p.userId != userId);
                        foreach (var duplicate in duplicateList)
                        {
                            duplicate.alreadyRead = (int)ConstVar.FormDuplicateStatus.Ready;
                        }
                    }
                }
                //1、从节点流程表中找出出口节点,状态通过
                var exitNode = this.GetNodeOperateId(db, templateId, formId, nodeId, ConstVar.LinkStatus.Pass, userId);
                if (exitNode == 0) return false;
                //找出该节点的类型
                var nodeType = db.tblFlowNode.Where(p => p.nodeId == exitNode && p.templateId == templateId).FirstOrDefault().nodeType;
                var status = nodeType == Convert.ToInt32(ConstVar.NodeType.Create) ? Convert.ToInt32(ConstVar.userFormStatus.unSubmit) : ((nodeType == Convert.ToInt32(ConstVar.NodeType.Submit) || nodeType == Convert.ToInt32(ConstVar.NodeType.Approval)) ? Convert.ToInt32(ConstVar.userFormStatus.flowing) : Convert.ToInt32(ConstVar.userFormStatus.hasCompleted));
                //2、更新用户表单
                this.UpdateUserForm(db, exitNode, formId, status, userId);
                //3、表单流程表中插入数据
                AddUserFormFlow(db, formId, nodeId, Convert.ToInt32(ConstVar.FormOperateType.Pass), suggest, userId, entructUser);
                //4、表单抄送表中插入数据
                var formModel = db.tblUserForm.Where(p => p.formId == formId && !p.deleteFlag).FirstOrDefault();
                if (formModel == null) return false;
                var nodeOperateList = this.GetNextOperateUserList(db, exitNode, formId, formModel.createUser);
                if (nodeOperateList.Count <= 0) return false;
                if (!checkNodeOperateList(nodeOperateList)) return false;
                if (nodeType == (int)ConstVar.NodeStatus.Archive)  //下一节点类型为归档,流程表中插入归档数据
                {
                    foreach (var nodeOperate in nodeOperateList)
                    {
                        if (nodeOperate.countersign == (int)ConstVar.NodeStatus.Archive) //操作人类型：归档
                        {
                            foreach (var user in nodeOperate.userIds)
                            {
                                AddUserFormFlow(db, formId, exitNode, Convert.ToInt32(ConstVar.FormOperateType.Archive), string.Empty, user.Value);
                            }
                        }
                        else        //操作人类型：抄送
                        {
                            AddFormDuplicate(db, new List<NodeOperateModel> { nodeOperate }, formId, exitNode);
                        }
                    }
                }
                else
                {
                    AddFormDuplicate(db, nodeOperateList, formId, exitNode);
                }
                db.SaveChanges();
            }
            return true;
        }

        #endregion 流程中的提交操作

        #region 获取饼图统计数量

        /// <summary>
        /// 获取饼图统计数量
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="userId">用户Id</param>
        /// <param name="admin">1、管理员 2、非管理员</param>
        /// <returns>流程数据统计集合</returns>
        public List<FlowProcessModel> GetFlowProcessList(int year, int month, int userId, int admin)
        {
            var processList = new List<FlowProcessModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (admin == 1)  //管理员
                {
                    var unSubmitModel = new FlowProcessModel();
                    unSubmitModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.unSubmit);
                    unSubmitModel.text = "待提交";
                    unSubmitModel.count = db.tblUserForm.Where(p => p.status == (int)(ConstVar.userFormStatus.unSubmit) && !p.deleteFlag && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(unSubmitModel);
                    var flowingModel = new FlowProcessModel();
                    flowingModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.flowing);
                    flowingModel.text = "流程中";
                    flowingModel.count = db.tblUserForm.Where(p => p.status == (int)(ConstVar.userFormStatus.flowing) && !p.deleteFlag && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(flowingModel);
                    var completedModel = new FlowProcessModel();
                    completedModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.hasCompleted);
                    completedModel.text = "已办结";
                    completedModel.count = db.tblUserForm.Where(p => p.status == (int)(ConstVar.userFormStatus.hasCompleted) && !p.deleteFlag && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(completedModel);
                }
                else        //非管理员
                {
                    //1、统计待提交的流程数据
                    var unSubmitModel = new FlowProcessModel();
                    unSubmitModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.unSubmit);
                    unSubmitModel.text = "待提交";
                    unSubmitModel.count = db.tblUserForm.Where(p => p.createUser == userId && p.status == (int)(ConstVar.userFormStatus.unSubmit) && !p.deleteFlag && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(unSubmitModel);
                    //2、统计已提交的流程数据
                    var SubmitedModel = new FlowProcessModel();
                    SubmitedModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.hasSubmited);
                    SubmitedModel.text = "已提交";
                    SubmitedModel.count = db.tblUserForm.Where(p => p.createUser == userId && p.status == (int)(ConstVar.userFormStatus.flowing) && !p.deleteFlag && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(SubmitedModel);
                    //3、统计待查阅的数据
                    var unReadModel = new FlowProcessModel();
                    unReadModel.text = "待查阅";
                    unReadModel.count = (from u in db.tblUserForm
                                         join d in db.tblFormDuplicate on u.formId equals d.formId
                                         where d.userId == userId && d.alreadyRead.Value == (int)ConstVar.FormDuplicateStatus.Ready && u.createTime.Year == year && u.createTime.Month == month && !u.deleteFlag
                                         select u).Count();
                    processList.Add(unReadModel);
                    //4、统计待审批的流程数据
                    var unCheckedModel = new FlowProcessModel();
                    unCheckedModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.unCheck);
                    unCheckedModel.text = "待审批";
                    unCheckedModel.count = (from u in db.tblUserForm
                                            join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId }
                                            where d.userId == userId && (d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.countersign)) && u.createTime.Year == year && u.createTime.Month == month && !u.deleteFlag && u.status == (int)ConstVar.userFormStatus.flowing
                                            select u).Count();
                    var entructList = (from r in db.tblEntrustResult
                                       join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                                       where !e.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now && e.mandataryUser == userId
                                       select e).Distinct().ToList();
                    if (entructList.Count > 0)
                    {
                        entructList.ForEach(p =>
                        {
                            unCheckedModel.count += (from u in db.tblUserForm
                                                     join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId }
                                                     where d.userId == p.entrustUser && (d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead.Value == (int)(ConstVar.FormDuplicateStatus.countersign)) && u.createTime.Year == year && u.createTime.Month == month && !u.deleteFlag && u.status == (int)ConstVar.userFormStatus.flowing
                                                     select u).Count();
                        });
                    }
                    processList.Add(unCheckedModel);
                    //unCheckedModel.count = db.tblFormDuplicate.Where(p => p.userId == userId && (p.alreadyRead.Value == 0 || p.alreadyRead.Value == 2) && p.createTime.Year == year && p.createTime.Month == month).Count();

                    //5、统计已处理的流程数据
                    var checkedModel = new FlowProcessModel();
                    checkedModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.hasChecked);
                    checkedModel.text = "已处理";
                    //从表单流程表中取出已处理的数据
                    var checkedList = (from u in db.tblUserForm
                                       join f in db.tblFormFlow on u.formId equals f.formId
                                       where !u.deleteFlag && !f.deleteFlag && u.status == (int)ConstVar.userFormStatus.flowing && f.createUser == userId && f.createTime.Year == year && f.createTime.Month == month
                                        && (f.result == (int)(ConstVar.FormOperateType.Pass) || f.result == (int)(ConstVar.FormOperateType.Read)
                                        || f.result == (int)(ConstVar.FormOperateType.Return) || f.result == (int)(ConstVar.FormOperateType.Revoke))
                                       select f).ToList();
                    var newCheckList = new List<tblFormFlow>();
                    if (checkedList.Count > 0)
                    {
                        //去除重复数据
                        checkedList.ForEach(p =>
                        {
                            if (checkedList.Where(s => s.formId == p.formId).Count() > 1)
                            {
                                if (newCheckList.Where(a => a.formId == p.formId).Count() <= 0)
                                {
                                    newCheckList.Add(p);
                                }
                            }
                            else
                            {
                                newCheckList.Add(p);
                            }
                        });
                    }
                    checkedModel.count = newCheckList.Count;
                    //checkedModel.count = db.tblFormDuplicate.Where(p => p.userId == userId && p.alreadyRead.Value == 3 && p.createTime.Year == year && p.createTime.Month == month).Count();
                    processList.Add(checkedModel);
                    //6、统计已办结的流程数据
                    var completeModel = new FlowProcessModel();
                    completeModel.id = Convert.ToInt32(ConstVar.FormFlowStatusId.hasCompleted);
                    completeModel.text = "已办结";
                    //创建的流程
                    //completeModel.count = (from u in db.tblUserForm
                    //                       where !u.deleteFlag.Value && u.status == (int)(ConstVar.userFormStatus.hasCompleted) && u.createTime.Value.Year == year && u.createTime.Value.Month == month && u.createUser == userId
                    //                       select u).Count();
                    //操作过的流程
                    var completedList = (from u in db.tblUserForm
                                         join d in db.tblFormFlow on u.formId equals d.formId
                                         where !u.deleteFlag && !d.deleteFlag && u.createTime.Year == year && u.createTime.Month == month && d.createUser == userId && u.status == (int)ConstVar.userFormStatus.hasCompleted
                                         select u).ToList();
                    var newCompletedList = new List<tblUserForm>();
                    if (completedList.Count > 0)
                    {
                        //去除重复数据
                        completedList.ForEach(p =>
                        {
                            if (completedList.Where(s => s.formId == p.formId).Count() > 1)
                            {
                                if (newCompletedList.Where(a => a.formId == p.formId).Count() <= 0)
                                {
                                    newCompletedList.Add(p);
                                }
                            }
                            else
                            {
                                newCompletedList.Add(p);
                            }
                        });
                        completeModel.count = newCompletedList.Count;
                    }
                    processList.Add(completeModel);
                }
            }
            return processList;
        }

        #endregion 获取饼图统计数量

        #region 获取表单创建人部门信息

        /// <summary>
        /// 获取表单创建人部门信息
        /// </summary>
        /// <param name="formId">表单Id</param>
        /// <returns>部门列表</returns>
        public List<OrganizationModel> GetorganizationList(int formId)
        {
            var orgList = new List<OrganizationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                orgList = (from f in db.tblUserForm
                           join us in db.tblUserStation on f.createUser equals us.userId
                           join s in db.tblStation on us.stationId equals s.stationId
                           join o in db.tblOrganization on s.organizationId equals o.organizationId
                           where !f.deleteFlag && !o.deleteFlag && f.formId == formId
                           select new OrganizationModel
                           {
                               id = o.organizationId,
                               name = o.organizationName
                           }).ToList();
            }
            return orgList;
        }

        #endregion 获取表单创建人部门信息

        #region 绑定登录用户的信息

        /// <summary>
        /// 绑定登录用户的信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>用户的信息</returns>
        public FlowIndexUserInfo GetUserOrganizationList(int userId)
        {
            var userInfo = new FlowIndexUserInfo();
            using (var db = new TargetNavigationDBEntities())
            {
                userInfo = (from u in db.tblUser
                            where !u.deleteFlag && u.userId == userId
                            select new FlowIndexUserInfo
                            {
                                userId = u.userId,
                                userName = u.userName,
                                createTime = DateTime.Now
                            }).FirstOrDefault();
                if (userInfo != null)
                {
                    userInfo.orgList = (from us in db.tblUserStation
                                        join s in db.tblStation on us.stationId equals s.stationId
                                        join o in db.tblOrganization on s.organizationId equals o.organizationId
                                        where !o.deleteFlag && us.userId == userId
                                        select new OrganizationModel
                                        {
                                            id = o.organizationId,
                                            name = o.organizationName
                                        }).Distinct().ToList();
                }
            }
            return userInfo;
        }

        #endregion 绑定登录用户的信息

        #region 绑定创建用户的岗位信息

        /// <summary>
        /// 绑定创建用户的岗位信息
        /// </summary>
        /// <param name="orgId">创建用户选择的部门</param>
        /// <param name="userId">创建用户Id</param>
        /// <returns>岗位列表</returns>
        public List<StationModel> GetUserStationList(int orgId, int userId)
        {
            var stationList = new List<StationModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                stationList = (from us in db.tblUserStation
                               join s in db.tblStation
                               on us.stationId equals s.stationId
                               where us.userId == userId && s.organizationId == orgId
                               select new StationModel
                               {
                                   id = s.stationId,
                                   name = s.stationName
                               }).Distinct().ToList();
            }
            return stationList;
        }

        #endregion 绑定创建用户的岗位信息

        #region 获取表单详情

        /// <summary>
        /// 获取表单详情
        /// </summary>
        /// <param name="templateId">模板Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <returns>模板及空间信息</returns>
        public TemplateInfoModel GetFormDetailInfo(int templateId, int formId, int nodeId)
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
                var controlList = db.tblTemplateControl.Where(p => p.templateId == templateId).ToList();

                foreach (var control in controlList)
                {
                    #region 控件信息

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

                    controlModel.status = db.tblNodeField.Where(p => p.nodeId == nodeId && p.controlId == control.controlId).FirstOrDefault().status;
                    // 设置控件信息,过滤掉当前节点下需要隐藏的控件
                    if (controlModel.status != 0)
                    {
                        templateControlInfo.control = controlModel;
                    }

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

        #endregion 获取表单详情

        #region 自定义排序

        /// <summary>
        /// 自定义排序
        /// </summary>
        /// <param name="orderList">排序的列表集合</param>
        /// <param name="planList">流程列表</param>
        /// <returns>流程列表集合</returns>
        public List<UserFormModel> GetFlowListOrderByCustom(List<Sort> orderList, List<UserFormModel> flowList)
        {
            if (flowList.Count() > 0)
            {
                foreach (var item in orderList)
                {
                    switch (item.type)
                    {
                        //默认
                        case 0:
                            flowList = flowList.OrderByDescending(p => p.urgency).OrderByDescending(p => p.createTime).ToList();
                            break;
                        //紧急度排序
                        case 1:
                            flowList = (item.direct == 1 ? flowList.OrderBy(p => p.urgency) : flowList.OrderByDescending(p => p.urgency)).ToList();
                            break;
                        //创建时间排序
                        case 2:
                            flowList = (item.direct == 1 ? flowList.OrderBy(p => p.createTime) : flowList.OrderByDescending(p => p.createTime)).ToList();
                            break;
                        //创建部门排序
                        case 3:
                            flowList = (item.direct == 1 ? flowList.OrderBy(p => p.organizationId) : flowList.OrderByDescending(p => p.organizationId)).ToList();
                            break;
                        //创建人排序
                        case 4:
                            flowList = (item.direct == 1 ? flowList.OrderBy(p => p.createUser) : flowList.OrderByDescending(p => p.createUser)).ToList();
                            break;
                        //流程分类排序
                        case 5:
                            flowList = (item.direct == 1 ? flowList.OrderBy(p => p.categoryId) : flowList.OrderByDescending(p => p.categoryId)).ToList();
                            break;
                    }
                }
            }
            return flowList;
        }

        #endregion 自定义排序

        #region 获取登录用户未完成流程数量

        /// <summary>
        /// 获取登录用户未完成流程数量
        /// </summary>
        /// <returns></returns>
        public int GetUserUnCompleteFlowCount(int userId)
        {
            var result = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                result = db.tblUserForm.Where(p => !p.deleteFlag && p.createUser == userId && (p.status == (int)ConstVar.userFormStatus.unSubmit || p.status == (int)ConstVar.userFormStatus.flowing)).Count();
            }
            return result;
        }

        #endregion 获取登录用户未完成流程数量

        #region 获取登录用户未完成流程列表

        /// <summary>
        /// 获取登录用户未完成流程列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>用户未完成流程列表</returns>
        public List<FormSimpleModel> GetUserUnCompleteList(int userId)
        {
            var formList = new List<FormSimpleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                formList = (from u in db.tblUserForm
                            orderby u.createTime descending
                            where !u.deleteFlag && u.createUser == userId && u.status != (int)ConstVar.userFormStatus.hasCompleted
                            select new FormSimpleModel
                            {
                                formId = u.formId,
                                templateId = u.templateId,
                                nodeId = u.currentNode,
                                status = u.status,
                                title = u.title,
                                createTime = u.createTime
                            }).Take(9).ToList();
            }
            return formList;
        }

        #endregion 获取登录用户未完成流程列表

        #region 私有方法

        #region 更新用户表单表

        /// <summary>
        /// 更新用户表单表
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="userId">用户Id</param>
        private void UpdateUserForm(TargetNavigationDBEntities db, int nodeId, int formId, int status, int userId)
        {
            var userForm = db.tblUserForm.Where(p => p.formId == formId).FirstOrDefault();
            if (userForm != null)
            {
                userForm.currentNode = nodeId;
                userForm.updateTime = DateTime.Now;
                userForm.updateUser = userId;
                userForm.status = status;
            }
        }

        #endregion 更新用户表单表

        #region 表单抄送表中插入数据

        /// <summary>
        /// 表单抄送表中插入数据
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="userId">节点操作人Id</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        private void AddFormDuplicate(TargetNavigationDBEntities db, List<NodeOperateModel> operateList, int formId, int nodeId)
        {
            if (operateList.Count > 0)
            {
                foreach (var operate in operateList)
                {
                    if (operate.userIds.Length > 0)
                    {
                        foreach (var item in operate.userIds)
                        {
                            var duplicateModel = new tblFormDuplicate
                            {
                                formId = formId,
                                userId = item.Value,
                                nodeId = nodeId,
                                alreadyRead = operate.countersign == (int)ConstVar.NodeStatus.Approval ? (int)ConstVar.FormDuplicateStatus.Approval : (operate.countersign == (int)ConstVar.NodeStatus.Archive ? (int)ConstVar.FormDuplicateStatus.Archive : (operate.countersign == (int)ConstVar.NodeStatus.Countersign ? (int)ConstVar.FormDuplicateStatus.countersign : (operate.countersign == (int)ConstVar.NodeStatus.Duplicate ? (int)ConstVar.FormDuplicateStatus.Ready : (int)ConstVar.FormDuplicateStatus.Submit))),
                                createTime = DateTime.Now
                            };
                            db.tblFormDuplicate.Add(duplicateModel);
                        }
                    }
                }
            }
        }

        #endregion 表单抄送表中插入数据

        #region 获取下一节点的Id

        /// <summary>
        /// 获取下一节点的Id
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="templateId">模板ID</param>
        /// <param name="formId">当前表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="userId">节点操作人Id</param>
        /// <param name="isPass">是否通过</param>
        /// <returns>下一节点Id</returns>
        private int GetNodeOperateId(TargetNavigationDBEntities db, int templateId, int formId, int nodeId, ConstVar.LinkStatus isPass, int userId)
        {
            var nextNode = commonBll.GetNextOperateNodeId(db, nodeId, formId, isPass, userId);
            return nextNode;
        }

        #endregion 获取下一节点的Id

        #region 取当前节点的操作人

        /// <summary>
        /// 取当前节点的操作人
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="userId">表单创建人Id</param>
        /// <returns>节点操作人信息列表</returns>
        private List<NodeOperateModel> GetNextOperateUserList(TargetNavigationDBEntities db, int nodeId, int formId, int userId)
        {
            var operateList = commonBll.GetNodeOperateList(db, nodeId, userId, formId);
            return operateList;
        }

        #endregion 取当前节点的操作人

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

        #region 插表单流程表中的数据

        /// <summary>
        /// 插表单流程表中的数据
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="formId">表单Id</param>
        /// <param name="nodeId">节点Id</param>
        /// <param name="result">操作类型</param>
        /// <param name="contents">意见</param>
        /// <param name="userId">用户Id</param>
        private void AddUserFormFlow(TargetNavigationDBEntities db, int formId, int nodeId, int result, string contents, int userId, int entrustUser = 0)
        {
            var obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var formFlowId = db.prcGetPrimaryKey("tblFormFlow", obj).FirstOrDefault().Value;
            var formFlowModel = new tblFormFlow
            {
                formFlowId = formFlowId,
                formId = formId,
                nodeId = nodeId,
                result = result,
                contents = contents,
                entrustUser = entrustUser,
                createUser = userId,
                updateUser = userId,
                createTime = DateTime.Now,
                updateTime = DateTime.Now,
                deleteFlag = false
            };
            db.tblFormFlow.Add(formFlowModel);
        }

        #endregion 插表单流程表中的数据

        #region 检查操作人个数

        /// <summary>
        /// 检查操作人个数
        /// </summary>
        /// <param name="nodeOperateList">节点操作人</param>
        /// <returns>检查操作人个数</returns>
        private bool checkNodeOperateList(List<NodeOperateModel> nodeOperateList)
        {
            var userList = new List<int>();
            foreach (var item in nodeOperateList)
            {
                if (item.userIds != null && item.userIds.Length > 0)
                {
                    foreach (var user in item.userIds)
                    {
                        userList.Add(user.Value);
                    }
                }
            }
            return userList.Count <= 0 ? false : true;
        }

        #endregion 检查操作人个数

        #region 获取表单当前未操作人信息

        /// <summary>
        /// 获取表单当前未操作人信息
        /// </summary>
        /// <param name="form">表单数据</param>
        /// <returns>表单当前未操作人信息</returns>
        private List<User> GetOperateUserInfo(TargetNavigationDBEntities db, UserFormModel form)
        {
            var operate = new List<User>();
            var operateList = (from u in db.tblUserForm
                               join d in db.tblFormDuplicate on new { id = u.formId, node = u.currentNode.Value } equals new { id = d.formId, node = d.nodeId }
                               join user in db.tblUser on d.userId equals user.userId
                               where u.status == (int)(ConstVar.userFormStatus.flowing) && (d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Approval) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.Submit) || d.alreadyRead == (int)(ConstVar.FormDuplicateStatus.countersign)) && !u.deleteFlag && d.formId == form.formId
                               select new User
                               {
                                   id = d.userId,
                                   name = user.userName
                               }).ToList();
            var entrust = (from r in db.tblEntrustResult
                           join e in db.tblFlowEntrust on r.entrustId equals e.entrustId
                           join u in db.tblUser on e.entrustUser equals u.userId into group1
                           from u in group1.DefaultIfEmpty()
                           join mUser in db.tblUser on e.mandataryUser equals mUser.userId into group2
                           from mUser in group2.DefaultIfEmpty()
                           where r.templateId == form.templateId && !e.deleteFlag && !u.deleteFlag && e.startTime <= DateTime.Now && e.endTime >= DateTime.Now
                           select new User
                           {
                               id = u.userId,
                               name = u.userName,
                               mandataryUser = mUser.userId,
                               mandataryUserName = mUser.userName
                           }).Distinct().ToList();
            if (entrust.Count > 0)
            {
                if (operateList.Count > 0)
                {
                    entrust.ForEach(p =>
                    {
                        if (operateList.Where(a => a.id == p.id).Count() > 0)
                        {
                            operate.Add(new User { id = p.id, name = p.name, mandataryUser = p.mandataryUser.Value, mandataryUserName = p.mandataryUserName });
                        }
                    });
                }
            }
            if (operateList.Count > 0)
            {
                operateList.ForEach(p =>
                {
                    if (operate.Where(a => a.id == p.id).Count() <= 0)
                    {
                        operate.Add(p);
                    }
                });
            }
            return operate;
        }

        #endregion 获取表单当前未操作人信息

        #endregion 私有方法
    }
}