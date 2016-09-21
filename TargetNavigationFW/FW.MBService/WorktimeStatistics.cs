using System;
using System.Collections.Generic;
using System.Linq;

using MB.DAL;
using MB.Model;

namespace FW.MBService
{
    public class WorktimeStatistics
    {
        public void StatisticsWorkTime()
        {
            using (var db = new TargetNavigationDBEntities())
            {
                // 取得计划列表
                var list = this.GetPlanList(db);

                #region 统计人均日工时

                // 人均日实际工时统计
                var perDayWorkTime = (from work in list
                                      where work.haveStatistics == 0
                                      group work by new { userId = work.userId, submitTime = work.submitTime.Value.ToString("yyyyMMdd") } into temp
                                      select new tblPerDayWorkTime
                                      {
                                          userId = temp.Key.userId,
                                          statisticalTime = temp.Key.submitTime,
                                          workTime = temp.Sum(p => p.workTime.Value),
                                          effectiveTime = 0
                                      }).ToList();

                // 人均日有效工时统计
                var perDayEffectiveTime = (from work in list
                                           where work.confirmTime != null
                                           group work by new { userId = work.userId, confirmTime = work.confirmTime.Value.ToString("yyyyMMdd") } into temp
                                           select new tblPerDayWorkTime
                                           {
                                               userId = temp.Key.userId,
                                               statisticalTime = temp.Key.confirmTime,
                                               effectiveTime = temp.Sum(p => p.effectiveTime.Value)
                                           }).ToList();

                foreach (var item in perDayEffectiveTime)
                {
                    var workTime = perDayWorkTime.Where(p => p.userId == item.userId && p.statisticalTime == item.statisticalTime).FirstOrDefault();

                    if (workTime == null)
                    {
                        perDayWorkTime.Add(item);
                    }
                    else
                    {
                        workTime.effectiveTime += item.effectiveTime;
                    }
                }

                this.SavePerDayWorkTime(perDayWorkTime, db);

                #endregion 统计人均日工时

                #region 统计人均月工时

                // 人均月实际工时统计
                var perMonthWorkTime = (from work in list
                                        where work.haveStatistics == 0
                                        group work by new { userId = work.userId, submitTime = work.submitTime.Value.ToString("yyyyMM") } into temp
                                        select new tblPerMonthWorkTime
                                        {
                                            userId = temp.Key.userId,
                                            statisticalTime = temp.Key.submitTime,
                                            workTime = temp.Sum(p => p.workTime.Value),
                                            effectiveTime = 0
                                        }).ToList();

                // 人均月有效工时统计
                var perMonthEffectiveTime = (from work in list
                                             where work.confirmTime != null
                                             group work by new { userId = work.userId, confirmTime = work.confirmTime.Value.ToString("yyyyMM") } into temp
                                             select new tblPerMonthWorkTime
                                             {
                                                 userId = temp.Key.userId,
                                                 statisticalTime = temp.Key.confirmTime,
                                                 effectiveTime = temp.Sum(p => p.effectiveTime.Value)
                                             }).ToList();

                foreach (var item in perMonthEffectiveTime)
                {
                    var workTime = perMonthWorkTime.Where(p => p.userId == item.userId && p.statisticalTime == item.statisticalTime).FirstOrDefault();

                    if (workTime == null)
                    {
                        perMonthWorkTime.Add(item);
                    }
                    else
                    {
                        workTime.effectiveTime += item.effectiveTime;
                    }
                }

                this.SavePerMonthWorkTime(perMonthWorkTime, db);

                #endregion 统计人均月工时

                #region 统计部门日工时

                // 部门日实际工时统计
                var orgDayWorkTime = (from work in list
                                      where work.haveStatistics == 0
                                      group work by new { orgId = work.orgId, submitTime = work.submitTime.Value.ToString("yyyyMMdd") } into temp
                                      select new tblOrgDayWorkTime
                                      {
                                          organizationId = temp.Key.orgId,
                                          statisticalTime = temp.Key.submitTime,
                                          workTime = temp.Sum(p => p.workTime.Value),
                                          effectiveTime = 0
                                      }).ToList();

                // 部门日有效工时统计
                var orgDayEffectiveTime = (from work in list
                                           where work.confirmTime != null
                                           group work by new { orgId = work.orgId, confirmTime = work.confirmTime.Value.ToString("yyyyMMdd") } into temp
                                           select new tblOrgDayWorkTime
                                           {
                                               organizationId = temp.Key.orgId,
                                               statisticalTime = temp.Key.confirmTime,
                                               effectiveTime = temp.Sum(p => p.effectiveTime.Value)
                                           }).ToList();

                foreach (var item in orgDayEffectiveTime)
                {
                    var dateTime = item.statisticalTime.Substring(0, 4) + "-" + item.statisticalTime.Substring(4, 2) + "-" + item.statisticalTime.Substring(6, 2) + " 23:59:59";
                    // 获取部门在职人员数
                    var userNum = this.GetOrgUserNum(item.organizationId, DateTime.Parse(dateTime), db);

                    var workTime = orgDayWorkTime.Where(p => p.organizationId == item.organizationId && p.statisticalTime == item.statisticalTime).FirstOrDefault();

                    if (workTime == null)
                    {
                        item.personalTime = item.effectiveTime / userNum;
                        orgDayWorkTime.Add(item);
                    }
                    else
                    {
                        workTime.effectiveTime += item.effectiveTime;
                        workTime.personalTime += item.effectiveTime / userNum;
                    }
                }

                this.SaveOrgDayWorkTime(orgDayWorkTime, db);

                #endregion 统计部门日工时

                #region 统计部门月工时

                var orgMonthWorkTime = new List<tblOrgMonthWorkTime>();

                foreach (var item in orgDayWorkTime)
                {
                    var dateTime = item.statisticalTime.Substring(0, 4) + "-" + item.statisticalTime.Substring(4, 2) + "-" + item.statisticalTime.Substring(6, 2) + " 23:59:59";
                    // 获取部门在职人员数
                    var userNum = this.GetOrgUserNum(item.organizationId, DateTime.Parse(dateTime), db);

                    var model = orgMonthWorkTime.Where(p => p.organizationId == item.organizationId && p.statisticalTime == item.statisticalTime.Substring(0, 6)).FirstOrDefault();

                    if (model == null)
                    {
                        orgMonthWorkTime.Add(new tblOrgMonthWorkTime
                        {
                            organizationId = item.organizationId,
                            statisticalTime = item.statisticalTime.Substring(0, 6),
                            workTime = item.workTime,
                            effectiveTime = item.workTime,
                            personalTime = item.effectiveTime / userNum
                        });
                    }
                    else
                    {
                        model.workTime += item.workTime;
                        model.effectiveTime += item.workTime;
                        model.personalTime += item.effectiveTime / userNum;
                    }
                }

                this.SaveOrgMonthWorkTime(orgMonthWorkTime, db);

                #endregion 统计部门月工时

                #region 更新统计状态

                foreach (var item in list)
                {
                    var haveStatistics = item.confirmTime == null ? 1 : 2;

                    if (item.submitId != null)
                    {
                        this.UpdateLoopPlanStatisticsFlag(item.submitId.Value, haveStatistics, db);
                    }
                    else
                    {
                        this.UpdatePlanStatisticsFlag(item.planId.Value, haveStatistics, db);
                    }
                }

                #endregion 更新统计状态

                db.SaveChanges();
            }
        }

        #region 私有方法

        /// <summary>
        /// 获取组织信息==>递归查询
        /// </summary>
        /// <returns></returns>
        private List<OrganizationInfo> GetOrgListByOrgId(int? OrganizationId, ref List<OrganizationInfo> orgInfo, TargetNavigationDBEntities db)
        {
            var first = (from org in db.tblOrganization
                         where org.organizationId == OrganizationId
                         select new OrganizationInfo
                         {
                             organizationId = org.organizationId,
                             parentOrganization = org.parentOrganization,
                             organizationName = org.organizationName
                         }).FirstOrDefault<OrganizationInfo>();
            orgInfo.Add(first);
            if (first.parentOrganization == 0)
            {
                return orgInfo;
            }
            return GetOrgListByOrgId(first.parentOrganization, ref orgInfo, db);
        }

        /// <summary>
        /// 获取各个在职用户数量
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <returns></returns>
        private int GetOrgUserNum(int orgId, DateTime statisticalTime, TargetNavigationDBEntities db)
        {
            var userNum = (from user in db.tblUser
                           join us in db.tblUserStation
                           on user.userId equals us.userId
                           join station in db.tblStation
                           on us.stationId equals station.stationId
                           where station.organizationId == orgId && (user.workStatus == 1 || user.quitTime > statisticalTime)
                           select user.userId).ToList().Count();

            return userNum;
        }

        /// <summary>
        /// 统计部门工时
        /// </summary>
        /// <param name="orgId">部门ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="db">DB</param>
        /// <returns></returns>
        private List<WorkTimeModel> GetPlanList(TargetNavigationDBEntities db)
        {
            var list = new List<WorkTimeModel>();

            // 循环计划取得
            var loopList = (from loop in db.tblLoopPlan
                            join submit in db.tblLoopplanSubmit
                            on loop.loopId equals submit.loopId
                            where submit.haveStatistics < 2
                                && loop.deleteFlag == false
                            select new WorkTimeModel
                            {
                                submitId = submit.submitId,
                                userId = submit.createUser,
                                orgId = loop.organizationId.Value,
                                submitTime = submit.submitTime.Value,
                                confirmTime = submit.confirmTime,
                                effectiveTime = submit.confirmTime == null ? 0 : loop.unitTime.Value * submit.number.Value * submit.completeQuality.Value * submit.completeQuantity.Value * submit.completeTime.Value,
                                workTime = loop.unitTime.Value * submit.number.Value
                            }).ToList();

            // 普通计划取得
            var planList = (from plan in db.tblPlan
                            where plan.stop == 0 && (plan.status == 30 || plan.status == 90)
                                && plan.haveStatistics < 2
                                && plan.deleteFlag == false
                            select new WorkTimeModel
                            {
                                planId = plan.planId,
                                userId = plan.responsibleUser.Value,
                                orgId = plan.organizationId.Value,
                                submitTime = plan.submitTime.Value,
                                confirmTime = plan.confirmTime,
                                effectiveTime = plan.completeQuality == null ? 0 : plan.quantity.Value * plan.time.Value * plan.completeQuality.Value * plan.completeQuantity.Value * plan.completeTime.Value,
                                workTime = plan.quantity.Value * plan.time.Value
                            }).ToList();

            list.AddRange(loopList);
            list.AddRange(planList);

            return list;
        }

        /// <summary>
        /// 保存人均日工时
        /// </summary>
        /// <param name="model">用户工时信息</param>
        private void SavePerDayWorkTime(List<tblPerDayWorkTime> modelList, TargetNavigationDBEntities db)
        {
            foreach (var model in modelList)
            {
                var workTimeModel = db.tblPerDayWorkTime.Where(p => p.userId == model.userId && p.statisticalTime == model.statisticalTime).FirstOrDefault();

                if (workTimeModel == null)
                {
                    db.tblPerDayWorkTime.Add(model);
                }
                else
                {
                    workTimeModel.workTime += model.workTime;
                    workTimeModel.effectiveTime += model.effectiveTime;
                }
            }
        }

        /// <summary>
        /// 保存人均月工时
        /// </summary>
        /// <param name="model">用户工时信息</param>
        private void SavePerMonthWorkTime(List<tblPerMonthWorkTime> modelList, TargetNavigationDBEntities db)
        {
            foreach (var model in modelList)
            {
                var workTimeModel = db.tblPerMonthWorkTime.Where(p => p.userId == model.userId && p.statisticalTime == model.statisticalTime).FirstOrDefault();

                if (workTimeModel == null)
                {
                    db.tblPerMonthWorkTime.Add(model);
                }
                else
                {
                    workTimeModel.workTime += model.workTime;
                    workTimeModel.effectiveTime += model.effectiveTime;
                }
            }
        }

        /// <summary>
        /// 保存部门日工时
        /// </summary>
        /// <param name="model">用户工时信息</param>
        private void SaveOrgDayWorkTime(List<tblOrgDayWorkTime> modelList, TargetNavigationDBEntities db)
        {
            foreach (var model in modelList)
            {
                var workTimeModel = db.tblOrgDayWorkTime.Where(p => p.organizationId == model.organizationId && p.statisticalTime == model.statisticalTime).FirstOrDefault();

                if (workTimeModel == null)
                {
                    db.tblOrgDayWorkTime.Add(model);
                }
                else
                {
                    workTimeModel.workTime += model.workTime;
                    workTimeModel.effectiveTime += model.effectiveTime;
                }
            }
        }

        /// <summary>
        /// 保存部门月工时
        /// </summary>
        /// <param name="model">用户工时信息</param>
        private void SaveOrgMonthWorkTime(List<tblOrgMonthWorkTime> modelList, TargetNavigationDBEntities db)
        {
            foreach (var model in modelList)
            {
                var workTimeModel = db.tblOrgMonthWorkTime.Where(p => p.organizationId == model.organizationId && p.statisticalTime == model.statisticalTime).FirstOrDefault();

                if (workTimeModel == null)
                {
                    db.tblOrgMonthWorkTime.Add(model);
                }
                else
                {
                    workTimeModel.workTime += model.workTime;
                    workTimeModel.effectiveTime += model.effectiveTime;
                }
            }
        }

        /// <summary>
        /// 更新循环计划统计标识
        /// </summary>
        /// <param name="submitId">提交ID</param>
        /// <param name="statisticsFlag">统计标识</param>
        /// <param name="db">DB</param>
        private void UpdateLoopPlanStatisticsFlag(int submitId, int statisticsFlag, TargetNavigationDBEntities db)
        {
            var model = db.tblLoopplanSubmit.Where(p => p.submitId == submitId).FirstOrDefault();

            if (model != null)
            {
                model.haveStatistics = statisticsFlag;
            }
        }

        /// <summary>
        /// 更新计划统计标识
        /// </summary>
        /// <param name="planId">计划ID</param>
        /// <param name="statisticsFlag">统计标识</param>
        /// <param name="db">DB</param>
        private void UpdatePlanStatisticsFlag(int planId, int statisticsFlag, TargetNavigationDBEntities db)
        {
            var model = db.tblPlan.Where(p => p.planId == planId).FirstOrDefault();

            if (model != null)
            {
                model.haveStatistics = statisticsFlag;
            }
        }

        #endregion 私有方法
    }
}