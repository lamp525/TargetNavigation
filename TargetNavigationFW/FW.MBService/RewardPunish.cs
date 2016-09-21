using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace FW.MBService
{
    public class RewardPunish
    {
        /// <summary>
        /// 获取超时天数，并添加到奖惩规则表（tblRewardPunish）中
        /// </summary>
        /// <param name="time"></param>
        public void GetRewardpunishStatistics(DateTime time)
        {
            var delayRuleList = new List<DelayRuleModel>();
            var rewardPModel = new RewardPunishModel();
            var timeOutDay = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                #region 计划统计处理

                //获取需要统计的所有计划
                var allPlanList = (from plan in db.tblPlan
                                   where !plan.deleteFlag && plan.rpFlag != true && plan.status > 0
                                   select new PlanOperateTimeModel
                                   {
                                       planId = plan.planId,
                                       responsibleUser = plan.responsibleUser,
                                       confirmUser = plan.confirmUser,
                                       planGenerateTime = plan.planGenerateTime,
                                       auditTime = plan.auditTime,
                                       submitTime = plan.submitTime,
                                       confirmTime = plan.confirmTime,
                                       status = plan.status,
                                       endTime = plan.endTime
                                   }).ToList();
                foreach (var item in allPlanList)
                {
                    //审核中或申请审核
                    if (item.status == 10 || item.status == 25)
                    {
                        //计划审核超时天数
                        timeOutDay = item.auditTime == null ? (time - item.planGenerateTime.Value).Days : (item.auditTime.Value - item.planGenerateTime.Value).Days;

                        //超时天数不为0 则计划审核延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.confirmUser.Value;
                            rewardPModel.targetId = item.planId;
                            rewardPModel.type = 2;
                            rewardPModel.delayTime = item.auditTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 2);
                           
                        }
                    }//待确认
                    else if (item.status == 30)
                    {
                        //计划确认超时天数

                        timeOutDay = item.confirmTime == null ? (time - item.submitTime.Value).Days : (item.confirmTime.Value - item.submitTime.Value).Days;
                        //超时天数不为0 则确认计划延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.confirmUser.Value;
                            rewardPModel.targetId = item.planId;
                            rewardPModel.type = 3;
                            rewardPModel.delayTime = item.confirmTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 2);
                            
                        }
                    }//审核通过
                    else if (item.status == 20)
                    {
                        //计划提交超时天数
                        timeOutDay = item.submitTime == null ? (time - item.endTime.Value).Days : (item.submitTime.Value - item.endTime.Value).Days;

                        //超时天数不为0 则计划完成延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.responsibleUser.Value;
                            rewardPModel.targetId = item.planId;
                            rewardPModel.type = 1;
                            rewardPModel.delayTime = item.submitTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 1);
                        }
                    }
                }

                //更新所有状态为已完成或已中止的计划统计标志为true
                db.tblPlan.Where(c => c.status == 90 || c.stop == 90).ToList().ForEach(c => c.rpFlag = true);

                #endregion 计划统计处理

                #region 目标统计处理

                //获取需要统计的所有目标
                var lastMonthObjectiveList = (from objective in db.tblObjective
                                              where !objective.deleteFlag && objective.rpFlag != true && objective.status > (int)ConstVar.ObjectIndexStatus.unSubmit
                                              //&& objective.createSubmitTime.HasValue &&objective.createSubmitTime.Value.Year.Equals(time.Year)&&objective.createSubmitTime.Value.Month.Equals(time.Month)&&objective.createSubmitTime.Value.Day.Equals(time.Day)
                                              select new
                                              {
                                                  objectiveId = objective.objectiveId,
                                                  responsibleUser = objective.responsibleUser,
                                                  confirmUser = objective.confirmUser,
                                                  createSubmitTime = objective.createSubmitTime,
                                                  auditTime = objective.auditTime,
                                                  resultSubmitTime = objective.resultSubmitTime,
                                                  confirmTime = objective.confirmTime,
                                                  status = objective.status,
                                                  endTime = objective.endTime
                                              }).ToList();
                foreach (var item in lastMonthObjectiveList)
                {
                    //待审核
                    if (item.status == (int)ConstVar.ObjectIndexStatus.unChecked)
                    {
                        //目标审核超时天数
                        timeOutDay = item.auditTime == null ? (time - item.createSubmitTime.Value).Days : (item.auditTime.Value - item.createSubmitTime.Value).Days;

                        //超时天数不为0 则目标审核延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.confirmUser.Value;
                            rewardPModel.targetId = item.objectiveId;
                            rewardPModel.type = 6;
                            rewardPModel.delayTime = item.auditTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 2);
                        }
                    }//待确认
                    else if (item.status == (int)ConstVar.ObjectIndexStatus.unConfirm)
                    {
                        //目标确认超时天数

                        timeOutDay = item.confirmTime == null ? (time - item.resultSubmitTime.Value).Days : (item.confirmTime.Value - item.resultSubmitTime.Value).Days;
                        //超时天数不为0 则确认目标延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.confirmUser.Value;
                            rewardPModel.targetId = item.objectiveId;
                            rewardPModel.type = 7;
                            rewardPModel.delayTime = item.confirmTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 2);
                        }
                    }//审核通过
                    else if (item.status == (int)ConstVar.ObjectIndexStatus.hasChecked)
                    {
                        //目标提交超时天数
                        timeOutDay = item.resultSubmitTime == null ? (time - item.endTime.Value).Days : (item.resultSubmitTime.Value - item.endTime.Value).Days;

                        //超时天数不为0 则目标完成延时
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = item.responsibleUser.Value;
                            rewardPModel.targetId = item.objectiveId;
                            rewardPModel.type = 5;
                            rewardPModel.delayTime = item.resultSubmitTime.Value;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 1);
                        }
                    }
                }

                //更新所有状态为已完成的目标统计标志为true
                db.tblObjective.Where(c => c.status == (int)ConstVar.ObjectIndexStatus.hasCompleted).ToList().ForEach(c => c.rpFlag = true);

                #endregion 目标统计处理

                #region 流程统计处理

                //流程审批
                var formList = (from uf in db.tblUserForm
                                join fn in db.tblFlowNode on uf.currentNode equals fn.nodeId
                                where !uf.deleteFlag && uf.rpFlag != true && fn.nodeType == (int)ConstVar.NodeType.Approval
                                select uf).ToList();
                foreach (var item in formList)
                {
                    //根据列表中每条记录的formId和currentNode从表单抄送表中找出所有alreadyRead为审批和会签的关联记录。
                    var list = db.tblFormDuplicate.Where(c => c.formId == item.formId && c.nodeId == item.currentNode &&
                        (c.alreadyRead == (int)ConstVar.FormDuplicateStatus.Approval || c.alreadyRead == (int)ConstVar.FormDuplicateStatus.countersign)).ToList();
                    foreach (var fdItem in list)
                    {
                        timeOutDay = (time - fdItem.createTime).Days;
                        if (timeOutDay != 0)
                        {
                            rewardPModel.timeOutDay = timeOutDay;
                            rewardPModel.userId = fdItem.userId;
                            rewardPModel.targetId = fdItem.formId;
                            rewardPModel.type = 4;
                            rewardPModel.delayTime = fdItem.createTime;
                            rewardPModel.statisticeTime = time;
                            this.RewardPunishByTimeoutDay(rewardPModel, 2);
                        }
                    }

                    //取出formId对应的表单流程表中所有操作类型为1：提交 2：通过 3：退回 4：撤销 的记录
                    var formFlowList = db.tblFormFlow.Where(c => !c.deleteFlag && c.formId == item.formId &&
                        (c.result == (int)ConstVar.FormOperateType.Submit || c.result == (int)ConstVar.FormOperateType.Pass || c.result == (int)ConstVar.FormOperateType.Return || c.result == (int)ConstVar.FormOperateType.Revoke)).OrderByDescending(c => c.createTime).ToList();

                    foreach (var ffItem in formFlowList)
                    {
                        //找出最新的nodeId不为当前表单currentNode的记录
                        if (ffItem.nodeId != item.currentNode)
                        {
                            //流程审批超时天数
                            timeOutDay = (time - ffItem.createTime).Days;

                            //超时天数不为0 则目标完成延时
                            if (timeOutDay != 0)
                            {
                                rewardPModel.timeOutDay = timeOutDay;
                                rewardPModel.userId = ffItem.createUser;
                                rewardPModel.targetId = ffItem.formId.Value;
                                rewardPModel.type = 4;
                                rewardPModel.delayTime = ffItem.createTime;
                                rewardPModel.statisticeTime = time;
                                this.RewardPunishByTimeoutDay(rewardPModel, 2);
                            }
                        }
                    }
                }
                //更新所有状态为已归档的表单奖惩统计标志rpFlag为true
                db.tblUserForm.Where(c => c.archive == true).ToList().ForEach(c => c.rpFlag = true);

                #endregion 流程统计处理

                db.SaveChanges();
            }
        }

        #region Deleted

        //        //循环计划完成列表
        //        var loopPlanList = (from loopPlan in db.tblLoopPlan
        //                            join submit in db.tblLoopplanSubmit
        //                            on loopPlan.loopId equals submit.loopId
        //                            where submit.confirmTime.Value.Month == DateTime.Now.Month - 1
        //                            select new LoopPlanSubmitModel
        //                            {
        //                                submitId = submit.submitId,
        //                                loopId = submit.loopId,
        //                                createUser = submit.createUser,
        //                                createTime = submit.createTime,
        //                                updateUser = submit.updateUser,
        //                                updateTime = submit.updateTime,
        //                                confirmTime = submit.confirmTime
        //                            }).ToList();
        //        foreach (var loopPlanItem in loopPlanList)
        //        {
        //            //提交时间不为NULL
        //            if (loopPlanItem.confirmTime != null)
        //            {
        //                timeOutDay = (loopPlanItem.confirmTime.Value - loopPlanItem.createTime).Days;
        //                //超时天数不为0 则循环计划确认延时
        //                if (timeOutDay != 0)
        //                {
        //                    //this.RewardPunishByTimeoutDay(timeOutDay, loopPlanItem.createUser, loopPlanItem.loopId, 3, loopPlanItem.confirmTime.Value,time);
        //                }
        //            }
        //        }
        //    }

        /// <summary>
        ///循环上个月末所有计划
        /// </summary>
        /// <param name="planModel"></param>
        /// <param name="timeOutDay"></param>
        //public void lastMonthPlan()
        //{
        //    var timeOutDay = 0;
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        //获取上个月末或这个月初所有的计划
        //        var planModel = (from plan in db.tblPlan
        //                         where plan.planGenerateTime.Value.Month == DateTime.Now.Month - 1 &&
        //                         (plan.auditTime.Value.Month == DateTime.Now.Month || plan.submitTime.Value.Month == DateTime.Now.Month || plan.confirmTime.Value.Month == DateTime.Now.Month)
        //                         select new PlanOperateTimeModel
        //                         {
        //                             responsibleUser = plan.responsibleUser,
        //                             confirmUser = plan.confirmUser,
        //                             planGenerateTime = plan.planGenerateTime,
        //                             auditTime = plan.auditTime,
        //                             submitTime = plan.submitTime,
        //                             confirmTime = plan.confirmTime,
        //                             status = plan.status
        //                         }).ToList();
        //        foreach (var item in planModel)
        //        {
        //            //计划提交时间不为NULL
        //            if (item.submitTime != null)
        //            {
        //                timeOutDay = (item.submitTime.Value - item.planGenerateTime.Value).Days;
        //                //超时天数不为0 则计划完成延时
        //                if (timeOutDay != 0)
        //                {
        //                    this.RewardPunishByTimeoutDay(timeOutDay, item.responsibleUser.Value, item.planId, 1, item.submitTime.Value);
        //                }
        //            }
        //            //计划审核时间不为NULL
        //            if (item.auditTime != null)
        //            {
        //                timeOutDay = (item.auditTime.Value - item.planGenerateTime.Value).Days;
        //                //超时天数不为0 则计划审核延时
        //                if (timeOutDay != 0)
        //                {
        //                    this.RewardPunishByTimeoutDay(timeOutDay, item.confirmUser.Value, item.planId, 2, item.auditTime.Value);
        //                }
        //            }
        //            //计划确认时间不为NULL
        //            if (item.confirmTime != null)
        //            {
        //                timeOutDay = (item.confirmTime.Value - item.submitTime.Value).Days;
        //                //超时天数不为0 则确认计划延时
        //                if (timeOutDay != 0)
        //                {
        //                    this.RewardPunishByTimeoutDay(timeOutDay, item.confirmUser.Value, item.planId, 3, item.confirmTime.Value);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion Deleted

        #region 私有方法

        /// <summary>
        /// 根据超时天数 新建奖惩规则
        /// </summary>
        /// <param name="timeOutDay">超时天数</param>
        /// <param name="rewardPModel">奖惩模型</param>
        /// <param name="planModel">计划信息模型</param>
        /// <param name="type">1.计划完成  2.计划审核  3.计划确认  4.流程审核</param>
        private void RewardPunishByTimeoutDay(RewardPunishModel rpModel, int ruleType)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                //根据超时天数 获取延时规则
                var delayRule = this.GetDelayRule(ruleType, rpModel.timeOutDay);

                if (delayRule != null)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    var rewardId = db.prcGetPrimaryKey("tblRewardPunish", obj).FirstOrDefault().Value;
                    var model = new tblRewardPunish
                    {
                        rewardId = rewardId,
                        userId = rpModel.userId,
                        type = rpModel.type,
                        targetId = rpModel.targetId,
                        timeoutDay = rpModel.timeOutDay,
                        deductionMode = delayRule.deductionMode,
                        deductionNum = delayRule.deductionNum,
                        delayTime = rpModel.delayTime,
                        statisticalTime = rpModel.statisticeTime
                    };
                    db.tblRewardPunish.Add(model);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 获取延迟规则
        /// </summary>
        /// <param name="timeOutday">超出天数</param>
        /// <returns></returns>
        private DelayRuleModel GetDelayRule(int ruleType, int timeOutday)
        {
            var delayRule = new DelayRuleModel();
            using (var db = new TargetNavigationDBEntities())
            {
                delayRule = (from rule in db.tblDelayRule
                             where rule.ruleType == ruleType && rule.delayStartTime <= timeOutday && rule.delayEndTime >= timeOutday
                             select new DelayRuleModel
                             {
                                 ruleId = rule.ruleId,
                                 ruletype = rule.ruleType,
                                 deductionMode = rule.deductionMode,
                                 deductionNum = rule.deductionNum
                             }).FirstOrDefault();
            }
            return delayRule;
        }

        #endregion 私有方法
    }
}