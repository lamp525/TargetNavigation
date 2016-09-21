using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class TemplateBLL : ITemplateBLL
    {
        /// <summary>
        /// 获取系统表单
        /// </summary>
        /// <param name="categoryid">分类Id</param>
        /// <param name="system">系统表单标示</param>
        /// <returns></returns>
        public List<TemplateManageModel> GetTemplateList(int? categoryid, bool system)
        {
            List<TemplateManageModel> templateList = new List<TemplateManageModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (categoryid != null)
                {
                    templateList = (from t in db.tblTemplate
                                    where t.system == system && t.categoryId == categoryid && t.deleteFlag != true
                                    select new TemplateManageModel
                                    {
                                        templateId = t.templateId,
                                        templateName = t.templateName,
                                        categoryId = t.categoryId,
                                        description = t.description,
                                        status = t.status,
                                        system = t.system,

                                        testFlag = t.testFlag
                                    }).ToList();
                }
                else
                {
                    templateList = (from t in db.tblTemplate
                                    where t.system == system && t.deleteFlag != true
                                    select new TemplateManageModel
                                    {
                                        templateId = t.templateId,
                                        templateName = t.templateName,
                                        categoryId = t.categoryId,
                                        description = t.description,
                                        status = t.status,
                                        system = t.system,
                                        testFlag = t.testFlag
                                    }).ToList();
                }
                foreach (var item in templateList)
                {
                    var used = db.tblUserForm.Where(p => p.templateId == item.templateId).ToList();
                    if (used.Count > 0)
                    {
                        item.IsUse = true;
                    }
                }
            }
            return templateList;
        }

        /// <summary>
        /// 获取表单分类
        /// </summary>
        /// <param name="system">是否是系统表单分类</param>
        /// <returns></returns>
        public List<TemplateCategoryModel> GetTemplateCategoryList(bool system)
        {
            List<TemplateCategoryModel> CategoryList = new List<TemplateCategoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                CategoryList = (from t in db.tblTemplateCategory
                                where t.system == system && t.deleteFlag != true
                                select new TemplateCategoryModel
                                {
                                    categoryId = t.categoryId,
                                    categoryName = t.categoryName,
                                    system = t.system,
                                    id = t.categoryId,
                                    name = t.categoryName
                                }).ToList();
            }
            return CategoryList;
        }

        #region 表单复制

        public bool CopyTemplate(int tempId, int[] categoryid, bool system)
        {
            var flag = false;

            using (var db = new TargetNavigationDBEntities())
            {
                foreach (int cateid in categoryid)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var copyTemId = db.prcGetPrimaryKey("tblTemplate", obj).FirstOrDefault().Value;
                    var oldTemId = tempId;
                    var oldtemdata = db.tblTemplate.Where(p => p.templateId == oldTemId).FirstOrDefault();
                    //表单模板复制
                    if (oldtemdata != null)
                    {
                        var newCopyTem = new tblTemplate
                        {
                            categoryId = cateid,
                            contents = oldtemdata.contents,
                            createTime = DateTime.Now,
                            createUser = oldtemdata.createUser,
                            defaultTitle = oldtemdata.defaultTitle,
                            deleteFlag = oldtemdata.deleteFlag,
                            description = oldtemdata.description,
                            status = oldtemdata.status,
                            system = false,
                            templateId = copyTemId,
                            templateName = oldtemdata.templateName,
                            updateTime = oldtemdata.updateTime,
                            updateUser = oldtemdata.updateUser,
                        };
                        db.tblTemplate.Add(newCopyTem);
                    }
                    //表单控件复制
                    var oldtemControl = db.tblTemplateControl.Where(p => p.templateId == oldTemId).ToList();
                    List<int> itemlist = new List<int>();//明细表单Id个数
                    if (oldtemControl != null)
                    {
                        foreach (var item in oldtemControl)
                        {
                            var newcopyControl = new tblTemplateControl
                            {
                                templateId = copyTemId,
                                color = item.color,
                                columnStatistics = item.columnStatistics,
                                controlId = item.controlId,
                                controlType = item.controlType,
                                createTime = item.createTime,
                                createUser = item.createUser,
                                defaultRowNum = item.defaultRowNum,
                                description = item.description,
                                lineType = item.lineType,
                                mutliSelect = item.mutliSelect,
                                parentControl = item.parentControl,
                                require = item.require,
                                size = item.size,
                                title = item.title,
                                updateTime = item.updateTime,
                                updateUser = item.updateUser,
                                vertical = item.vertical,
                                columnIndex = item.columnIndex,
                                moneyControl = item.moneyControl
                            };
                            db.tblTemplateControl.Add(newcopyControl);
                        }
                    }
                    //明细公式复制

                    var oldtemdetailList = db.tblDetailFormula.Where(p => p.templateId == oldTemId).ToList();
                    if (oldtemdetailList != null)
                    {
                        foreach (var item in oldtemdetailList)
                        {
                            var copydetailId = db.prcGetPrimaryKey("tblDetailFormula", obj).FirstOrDefault().Value;
                            var copyderai = new tblDetailFormula
                            {
                                controlId = item.controlId,
                                detailControl = item.detailControl,
                                displayText = item.displayText,
                                formulaId = copydetailId,
                                operate = item.operate,
                                orderNum = item.orderNum,
                                templateId = copyTemId
                            };
                            db.tblDetailFormula.Add(copyderai);
                        }
                    }
                    //控件项目复制
                    var oldControItemList = db.tblControlItem.Where(p => p.templateId == oldTemId).ToList();
                    if (oldControItemList != null)
                    {
                        foreach (var item in oldControItemList)
                        {
                            var copyControItemId = db.prcGetPrimaryKey("tblControlItem", obj).FirstOrDefault().Value;
                            var copyControItem = new tblControlItem
                            {
                                itemId = copyControItemId,
                                controlId = item.controlId,
                                itemText = item.itemText,
                                orderNum = item.orderNum,
                                templateId = copyTemId,
                                checkOn = item.checkOn
                            };
                            db.tblControlItem.Add(copyControItem);
                        }
                        //流程节点复制
                        //var oldFlowNode = db.tblFlowNode.Where(p => p.templateId == oldTemId).ToList();
                        //if (oldFlowNode != null)
                        //{
                        //    foreach (var item in oldFlowNode)
                        //    {
                        //        var copyFolwNodeid = db.prcGetPrimaryKey("tblFlowNode", obj).FirstOrDefault().Value;
                        //        var copyFolwNode = new tblFlowNode
                        //        {
                        //            nodeId = copyFolwNodeid,
                        //            nodeName = item.nodeName,
                        //            nodeType = item.nodeType,
                        //            templateId = copyTemId
                        //        };
                        //        db.tblFlowNode.Add(copyFolwNode);

                        //        //节点字段复制
                        //        var oldNodeField = db.tblNodeField.Where(p => p.nodeId == item.nodeId).ToList();
                        //        foreach (var ii in oldNodeField)
                        //        {
                        //            var copyFolwField = db.prcGetPrimaryKey("tblNodeField", obj).FirstOrDefault().Value;
                        //            var copyNodeField = new tblNodeField
                        //            {
                        //                controlId = ii.controlId,
                        //                nodeId = copyFolwField,
                        //                status = ii.status
                        //            };
                        //        }

                        //    }
                        //}
                    }
                    db.SaveChanges();
                }
                flag = true;
            }
            return flag;
        }

        #endregion 表单复制

        /// <summary>
        /// 表单分类模糊查询
        /// </summary>
        /// <param name="text"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        public List<TemplateCategoryModel> GetSelectCategoryList(string text)
        {
            List<TemplateCategoryModel> CategoryList = new List<TemplateCategoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                CategoryList = (from us in db.tblTemplateCategory
                                where us.categoryName.IndexOf(text) != -1 && us.deleteFlag != true
                                select new TemplateCategoryModel
                            {
                                categoryId = us.categoryId,
                                categoryName = us.categoryName,
                                system = us.system,
                                id = us.categoryId,
                                name = us.categoryName
                            }).ToList();
            }
            return CategoryList;
        }

        /// <summary>
        /// 新建表单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddTemplate(TemplateManageModel model)
        {
            int TemplateId;
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                TemplateId = db.prcGetPrimaryKey("tblTemplate", obj).FirstOrDefault().Value;
                var Template = new tblTemplate
                {
                    categoryId = model.categoryId,
                    description = model.description,
                    status = model.status,
                    system = model.system,
                    templateId = TemplateId,
                    templateName = model.templateName,
                    createTime = DateTime.Now,
                    createUser = model.creatUser,
                    updateTime = DateTime.Now,
                    deleteFlag = false,
                    updateUser = model.creatUser,
                    testFlag = false
                };
                db.tblTemplate.Add(Template);
                db.SaveChanges();
            }
            return TemplateId;
        }

        //表单删除
        public int DeleteTem(int[] temId)
        {
            var flag = 1;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in temId)
                {
                    var isUse = db.tblUserForm.Where(p => p.templateId == item && p.deleteFlag == false).ToList();
                    if (isUse.Count > 0)
                    {
                        flag = 2;
                        break;
                    }
                    var firstData = db.tblTemplate.Where(p => p.templateId == item).FirstOrDefault();
                    if (firstData != null)
                    {
                        firstData.deleteFlag = true;
                        db.SaveChanges();
                    }
                    flag = 3;
                }
            }
            return flag;
        }

        //表单移动
        public bool MoveTem(int[] tempIds, int tocaregoryId)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                if (tempIds.Length > 0)
                {
                    foreach (var item in tempIds)
                    {
                        var tem = db.tblTemplate.Where(p => p.templateId == item).FirstOrDefault();
                        if (tem != null)
                        {
                            tem.categoryId = tocaregoryId;
                            db.SaveChanges();
                            //添加日志
                            // AddCompanyDocumentLog(db, docModel.documentId, 3, string.Empty, userId);
                        }
                    }
                }
                flag = true;
            }
            return flag;
        }

        public bool StopOrStartTem(int tempId, int flag)
        {
            var f = false;
            using (var db = new TargetNavigationDBEntities())
            {
                var tem = db.tblTemplate.Where(p => p.templateId == tempId).FirstOrDefault();
                if (flag == 1)
                {
                    tem.status = 1;
                }
                else
                {
                    tem.status = 0;
                }
                db.SaveChanges();
            }
            return f;
        }
    }
}