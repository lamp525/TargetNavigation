using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class AuthManagementBLL : IAuthManagementBLL
    {
        private string[] planStr = { "计划待提交", "计划待审批", "计划已审批", "计划待确认", "计划已完成" };
        private string[] objectiveStr = { "目标待提交", "目标待审核", "目标进行中", "目标待确认" };
        private string[] flowStr = { "流程待提交", "流程待查阅", "流程待审批" };
        private string[] userDocumentStr = { "个人文档" };

        /// <summary>
        /// 权限设置查询
        /// </summary>
        /// <param name="authName">权限名</param>
        /// <returns></returns>
        public List<AuthShift> GetAuthList(string authName = null)
        {
            var list = new List<AuthShift>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (authName == null)
                {
                    list = (from auth in db.tblAuth
                            select new AuthShift
                            {
                                authId = auth.authId,
                                authName = auth.authName,
                                auth = auth.auth
                            }).ToList();
                }
                else
                {
                    list = (from auth in db.tblAuth
                            where auth.authName.IndexOf(authName) != -1
                            select new AuthShift
                            {
                                authId = auth.authId,
                                authName = auth.authName,
                                auth = auth.auth
                            }).ToList();
                }
                foreach (var item in list)
                {
                    var n = new ArrayList();
                    var o = new ArrayList();
                    var f = new ArrayList();
                    var d = new ArrayList();
                    var nameStr = item.auth.Split(',');
                    var planList = nameStr.Where(c => c.Contains("计划")).ToList();
                    for (int i = 0; i < planStr.Count(); i++)
                    {
                        for (int p = 0; p < planList.Count(); p++)
                        {
                            if (planStr[i] == planList[p])
                            {
                                n.Add(i + 1);
                            }
                        }
                    }
                    item.planData = (int[])n.ToArray(typeof(int));
                    var objList = nameStr.Where(c => c.Contains("目标")).ToList();
                    for (int i = 0; i < objectiveStr.Count(); i++)
                    {
                        for (int p = 0; p < objList.Count(); p++)
                        {
                            if (objectiveStr[i] == objList[p])
                            {
                                o.Add(i + 1);
                            }
                        }
                    }
                    item.objectiveData = (int[])o.ToArray(typeof(int));
                    var flowList = nameStr.Where(c => c.Contains("流程")).ToList();
                    for (int i = 0; i < flowStr.Count(); i++)
                    {
                        for (int p = 0; p < flowList.Count(); p++)
                        {
                            if (flowStr[i] == flowList[p])
                            {
                                f.Add(i + 1);
                            }
                        }
                    }
                    item.FlowData = (int[])f.ToArray(typeof(int));
                    var docuList = nameStr.Where(c => c.Contains("个人文档")).ToList();
                    for (int i = 0; i < userDocumentStr.Count(); i++)
                    {
                        for (int p = 0; p < docuList.Count(); p++)
                        {
                            if (userDocumentStr[i] == docuList[p])
                            {
                                d.Add(i + 1);
                            }
                        }
                    }
                    item.userDocumentData = (int[])d.ToArray(typeof(int));
                }
            }
            return list;
        }

        /// <summary>
        ///  权限新建和修改
        /// </summary>
        /// <param name="authModel"></param>
        public void SaveAuth(AuthShift authModel)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var authId = db.prcGetPrimaryKey("tblAuth", obj).FirstOrDefault().Value;

                var auth = new ArrayList();

                var authM = db.tblAuth.Where(c => c.authId == authModel.authId).FirstOrDefault();

                //循环计划状态
                foreach (var pItem in authModel.planData)
                {
                    auth.Add(planStr[pItem - 1]);
                }
                //循环目标状态
                foreach (var oItem in authModel.objectiveData)
                {
                    auth.Add(objectiveStr[oItem - 1]);
                }
                //循环流程状态
                foreach (var fItem in authModel.FlowData)
                {
                    auth.Add(flowStr[fItem - 1]);
                }
                //循环用户文档
                foreach (var uItem in authModel.userDocumentData)
                {
                    auth.Add(userDocumentStr[uItem - 1]);
                }
                if (authM == null)
                {
                    var model = new tblAuth
                    {
                        authId = authId,
                        authName = authModel.authName,
                        auth = string.Join(",", (string[])auth.ToArray(typeof(string)))
                    };
                    db.tblAuth.Add(model);
                }
                else
                {
                    authM.authName = authModel.authName;
                    authM.auth = string.Join(",", (string[])auth.ToArray(typeof(string)));
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        public void DeleteAuth(int id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var model = db.tblAuth.Where(c => c.authId == id).FirstOrDefault();
                if (model != null)
                {
                    db.tblAuth.Remove(model);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        ///  权限转移
        /// </summary>
        /// <param name="authShift"></param>
        public void AuthShift(AuthShift authShift)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //计划状态
                if (authShift.planData.Count() != 0)
                {
                    var planList = db.tblPlan.Where(c => !c.deleteFlag && (c.responsibleUser == authShift.turnUserId || c.confirmUser == authShift.turnUserId)).ToList();
                    //1.待提交
                    if (authShift.planData.Contains(1))
                    {
                        foreach (var item in planList.Where(c => c.status == 0).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    //2.待审批
                    if (authShift.planData.Contains(2))
                    {
                        foreach (var item in planList.Where(c => c.status == 10).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    // 3.已审批
                    if (authShift.planData.Contains(3))
                    {
                        foreach (var item in planList.Where(c => c.status == 20))
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    // 4.待确认
                    if (authShift.planData.Contains(4))
                    {
                        foreach (var item in planList.Where(c => c.status == 30))
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }

                    //5.已完成  添加协作人
                    if (authShift.planData.Contains(5))
                    {
                        foreach (var item in planList.Where(c => c.status == 90).ToList())
                        {
                            var pCooperation = db.tblPlanCooperation.Where(c => c.planId == item.planId && c.userId == authShift.acceptUserId).FirstOrDefault();
                            if (pCooperation == null)
                            {
                                var plan = new tblPlanCooperation
                                {
                                    planId = item.planId,
                                    userId = authShift.acceptUserId
                                };
                                db.tblPlanCooperation.Add(plan);
                            }
                        }
                    }
                }
                db.SaveChanges();

                if (authShift.objectiveData.Count() != 0)
                {
                    var objectiveList = db.tblObjective.Where(c => !c.deleteFlag && (c.responsibleUser == authShift.turnUserId || c.confirmUser == authShift.turnUserId)).ToList();
                    //目标状态  1.待提交       5.已完成  6.已超时
                    if (authShift.objectiveData.Contains(1))
                    {
                        foreach (var item in objectiveList.Where(c => c.status == 1).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    // 2.待审核
                    if (authShift.objectiveData.Contains(2))
                    {
                        foreach (var item in objectiveList.Where(c => c.status == 2).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    // 3.进行中
                    if (authShift.objectiveData.Contains(3))
                    {
                        foreach (var item in objectiveList.Where(c => c.status == 3).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                    // 4.待确认
                    if (authShift.objectiveData.Contains(4))
                    {
                        foreach (var item in objectiveList.Where(c => c.status == 4).ToList())
                        {
                            this.UpdateResponsibleUser(item, authShift.turnUserId, authShift.acceptUserId);
                        }
                    }
                }
                db.SaveChanges();
                //个人文档
                if (authShift.userDocumentData.Count() != 0)
                {
                    if (authShift.userDocumentData.Contains(1))
                    {
                        var userDocumentList = (from u in db.tblUserDocument
                                                join us in db.tblUser on u.createUser equals us.userId
                                                where !u.deleteFlag && u.createUser == authShift.turnUserId
                                                select u).ToList();
                        userDocumentList.ForEach(c => c.createUser = authShift.acceptUserId);
                    }

                    #region 共享

                    //1.我的共享
                    //if (authShift.userDocumentData.Contains(1))
                    //{
                    //    var userDocumentList = from u in db.tblUserDocument
                    //                           where !u.deleteFlag.Value && u.createUser == authShift.turnUserId
                    //                           select u;
                    //    foreach (var item in userDocumentList)
                    //    {
                    //        var document = db.tblDocumentShared.Where(c => c.documentId == item.documentId && c.userId == authShift.acceptUserId).FirstOrDefault();
                    //        if (document == null)
                    //        {
                    //            var d = new tblDocumentShared
                    //            {
                    //                documentId = item.documentId,
                    //                userId = authShift.acceptUserId
                    //            };
                    //            db.tblDocumentShared.Add(d);
                    //        }
                    //    }
                    //}
                    //// 2.他人共享
                    //if (authShift.userDocumentData.Contains(2))
                    //{
                    //    var documentShared = db.tblDocumentShared.Where(c => c.userId == authShift.turnUserId).ToList();
                    //    if (documentShared.Count() != 0)
                    //    {
                    //        foreach (var item in documentShared)
                    //        {
                    //            db.tblDocumentShared.Remove(item);
                    //            var model = new tblDocumentShared
                    //            {
                    //                documentId = item.documentId,
                    //                userId = authShift.acceptUserId
                    //            };
                    //            db.tblDocumentShared.Add(model);
                    //        }
                    //    }
                    //}
                    ////3.公司文档权限
                    //if (authShift.userDocumentData.Contains(3))
                    //{
                    //    var companyDocument = (from c in db.tblCompanyDocument
                    //                           join f in db.tblFolderAuth on c.documentId equals f.documentId into group1
                    //                           from f in group1.DefaultIfEmpty()
                    //                           join a in db.tblAuthResult on f.authId equals a.authId
                    //                           where !c.deleteFlag && c.createUser == authShift.turnUserId && f.type == 3
                    //                           select a).ToList();
                    //    foreach (var item in companyDocument)
                    //    {
                    //        item.targetId = authShift.acceptUserId;
                    //    }
                    //}

                    #endregion 共享
                }
                db.SaveChanges();

                //流程
                if (authShift.FlowData.Count() != 0)
                {
                    //修改节点操作人
                    var nodeOperateList = (from n in db.tblNodeOperate
                                           join o in db.tblOperateResult on n.operateId equals o.operateId
                                           where n.type == 3 && o.targetId == authShift.turnUserId
                                           select o).ToList();
                    foreach (var item in nodeOperateList)
                    {
                        item.targetId = authShift.acceptUserId;
                    }

                    //修改用户表单
                    var flowList = (from flowU in db.tblUserForm
                                    join fd in db.tblFormDuplicate on new { id = flowU.formId, node = flowU.currentNode.Value } equals new { id = fd.formId, node = fd.nodeId }
                                    where !flowU.deleteFlag && fd.userId == authShift.turnUserId
                                    orderby fd.createTime descending
                                    select new { flowU, fd }).ToList();
                    // 1.待提交
                    if (authShift.FlowData.Contains(1))
                    {
                        foreach (var item in flowList.Where(c => c.flowU.status == (int)(ConstVar.userFormStatus.unSubmit)).ToList())
                        {
                            var form = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.turnUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                            if (form != null)
                            {
                                db.tblFormDuplicate.Remove(form);
                                var form2 = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.acceptUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                                if (form2 == null)
                                {
                                    var model = this.HasFormDuplicate(item.fd, authShift.acceptUserId);
                                    db.tblFormDuplicate.Add(model);
                                }
                            }
                        }
                        db.SaveChanges();
                    }

                    #region 已提交

                    // 2.已提交
                    //if (authShift.FlowData.Contains(2))
                    //{
                    //    foreach (var item in flowList.Where(c => c.flowU.status == (int)(ConstVar.userFormStatus.flowing)).ToList())
                    //    {
                    //        var form = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.turnUserId).FirstOrDefault();
                    //        if (form != null)
                    //        {
                    //            db.tblFormDuplicate.Remove(item.fd);
                    //            var model = this.HasFormDuplicate(item.fd, authShift.acceptUserId);
                    //            db.tblFormDuplicate.Add(model);
                    //        }
                    //    }
                    //    db.SaveChanges();
                    //}

                    #endregion 已提交

                    //3.待查阅
                    if (authShift.FlowData.Contains(2))
                    {
                        foreach (var item in flowList.Where(c => c.fd.alreadyRead == (int)ConstVar.FormDuplicateStatus.Ready).ToList())
                        {
                            var form = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.turnUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                            if (form != null)
                            {
                                db.tblFormDuplicate.Remove(form);
                                var form2 = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.acceptUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                                if (form2 == null)
                                {
                                    var model = this.HasFormDuplicate(item.fd, authShift.acceptUserId);
                                    db.tblFormDuplicate.Add(model);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    // 4.待审批
                    if (authShift.FlowData.Contains(3))
                    {
                        var fd = flowList.Where(c => c.flowU.status == (int)ConstVar.userFormStatus.flowing && (c.fd.alreadyRead == (int)ConstVar.FormDuplicateStatus.Approval || c.fd.alreadyRead == (int)ConstVar.FormDuplicateStatus.Submit || c.fd.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign)).ToList();
                        foreach (var item in fd)
                        {
                            var form = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.turnUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                            if (form != null)
                            {
                                db.tblFormDuplicate.Remove(form);
                                var form2 = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.acceptUserId && c.nodeId == item.fd.nodeId).FirstOrDefault();
                                if (form2 == null)
                                {
                                    var model = this.HasFormDuplicate(item.fd, authShift.acceptUserId);
                                    db.tblFormDuplicate.Add(model);
                                }
                            }
                        }
                        db.SaveChanges();
                    }
                    //5.已处理
                    //if (authShift.FlowData.Contains(5))
                    //{
                    //    var f = (from u in flowList
                    //             join l in db.tblFormFlow on u.fd.formId equals l.formId
                    //             where u.flowU.status == (int)ConstVar.userFormStatus.flowing && (l.result == (int)(ConstVar.FormOperateType.Pass) || l.result == (int)(ConstVar.FormOperateType.Return) || l.result == (int)(ConstVar.FormOperateType.Revoke) || l.result == (int)(ConstVar.FormOperateType.Read))
                    //             select u).ToList();

                    //    foreach (var item in f)
                    //    {
                    //        var form = db.tblFormDuplicate.Where(c => c.formId == item.fd.formId && c.userId == authShift.turnUserId).FirstOrDefault();
                    //        if (form != null)
                    //        {
                    //            db.tblFormDuplicate.Remove(item.fd);
                    //            var model = this.HasFormDuplicate(item.fd, authShift.acceptUserId);
                    //            db.tblFormDuplicate.Add(model);
                    //        }
                    //    }
                    //    db.SaveChanges();
                    //}
                }
            }
        }

        #region 私有方法

        /// <summary>
        /// 修改流程操作人
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="acceptUserId"></param>
        private tblFormDuplicate HasFormDuplicate(tblFormDuplicate fd, int acceptUserId)
        {
            var model = new tblFormDuplicate
            {
                formId = fd.formId,
                userId = acceptUserId,
                nodeId = fd.nodeId,
                alreadyRead = fd.alreadyRead,
                createTime = fd.createTime
            };
            return model;
        }

        /// <summary>
        /// 修改目标责任人和确认人
        /// </summary>
        /// <param name="objective">目标实体</param>
        /// <param name="turnUserId">转出者</param>
        /// <param name="acceptUserId">接受者</param>
        private void UpdateResponsibleUser(tblObjective objective, int turnUserId, int acceptUserId)
        {
            if (objective.responsibleUser == turnUserId && objective.confirmUser != acceptUserId)
            {
                objective.responsibleUser = acceptUserId;
            }
            else if (objective.confirmUser == turnUserId)
            {
                objective.confirmUser = acceptUserId;
            }
        }

        /// <summary>
        /// 修改目标责任人和确认人
        /// </summary>
        /// <param name="plan">计划实体</param>
        /// <param name="turnUserId">转出者</param>
        /// <param name="acceptUserId">接受者</param>
        private void UpdateResponsibleUser(tblPlan plan, int turnUserId, int acceptUserId)
        {
            if (plan.responsibleUser == turnUserId && plan.confirmUser != acceptUserId)
            {
                plan.responsibleUser = acceptUserId;
            }
            else if (plan.confirmUser == turnUserId)
            {
                plan.confirmUser = acceptUserId;
            }
        }

        #endregion 私有方法
    }
}