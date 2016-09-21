using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class PlanStatisticsBLL : IPlanStatisticsBLL
    {
        #region 添加统计信息

        /// <summary>
        /// 添加统计信息
        /// </summary>
        /// <param name="stainfo">统计信息对象</param>
        public StatisticsInfo AddStatisticsInfo(StatisticsInfo stainfo)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                stainfo.statisticsId = db.prcGetPrimaryKey("tblPlanStatistics", obj).FirstOrDefault().Value;
                var planStatis = new tblPlanStatistics
                {
                    statisticsId = stainfo.statisticsId,
                    statisticsName = stainfo.statisticsName,
                    createUser = stainfo.createUser,
                    createTime = stainfo.createTime,
                    updateUser = stainfo.updateUser,
                    updateTime = stainfo.updateTime
                };
                db.tblPlanStatistics.Add(planStatis);
                for (int i = 0; i < stainfo.organizationId.Length; i++)
                {
                    var staorg = new tblPlanStatisticsOrg
                    {
                        statisticsId = stainfo.statisticsId,
                        organizationId = stainfo.organizationId[i]
                    };
                    db.tblPlanStatisticsOrg.Add(staorg);
                }
                db.SaveChanges();
            }
            return stainfo;
        }

        #endregion 添加统计信息

        #region 按部门号获取该部门成员的计划完成情况

        /// <summary>
        /// 按部门号获取该部门成员的计划完成情况
        /// </summary>
        /// <param name="organizationId">组织Id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="sortby">排序 0：计划量 1：完成量 2：完成率</param>
        /// <param name="sort">排序方式   0：升序 1：降序</param>
        /// <returns>计划统计的集合</returns>
        public List<PlanStatisticsModel> GetPlanStatistics(int organizationId, DateTime startTime, DateTime endTime, int sortby, int sort)
        {
            var planStaList = new List<PlanStatisticsModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var orgIdsNew = GetAllorganizationIds(organizationId, db);
                foreach (var orgId in orgIdsNew)
                {
                    var tempList = new List<PlanStatisticsModel>();
                    //获取部门下的人员信息
                    tempList = (from user in db.tblUser
                                join userStation in db.tblUserStation on user.userId equals userStation.userId into group1
                                from userStation in group1.DefaultIfEmpty()
                                join station in db.tblStation on userStation.stationId equals station.stationId into group2
                                from station in group2.DefaultIfEmpty()
                                where station.organizationId == orgId && ((!user.deleteFlag && user.workStatus == 1) || ((user.deleteFlag || user.workStatus != 1) && user.quitTime >= startTime && user.quitTime < endTime))
                                select new PlanStatisticsModel
                                {
                                    userId = user.userId,
                                    name = user.userName,
                                    image = user.midImage
                                }).ToList();

                    if (tempList.Count() > 0)
                    {
                        //遍历人员进行统计
                        foreach (var planSta in tempList)
                        {
                            planSta.eventTotalCount = 0;
                            var statusCountList = (from plan in db.tblPlan
                                                   where plan.responsibleUser == planSta.userId && plan.organizationId == orgId && plan.stop == 0 && plan.endTime.Value >= startTime && plan.endTime < endTime && !plan.deleteFlag
                                                   group plan by plan.status into statusCount
                                                   select new PlanStatusCount
                                                   {
                                                       status = statusCount.Key,
                                                       stop = 0,
                                                       statusCount = statusCount.Count()
                                                   }).ToList();
                            var stopList = (from plan in db.tblPlan
                                            where plan.responsibleUser == planSta.userId && plan.organizationId == orgId && plan.stop != 0 && plan.endTime.Value >= startTime && plan.endTime < endTime && !plan.deleteFlag
                                            group plan by plan.stop into statusCount
                                            select new PlanStatusCount
                                            {
                                                status = 0,
                                                stop = statusCount.Key,
                                                statusCount = statusCount.Count()
                                            });
                            if (stopList.Count() > 0)
                            {
                                foreach (var stopModel in stopList)
                                {
                                    statusCountList.Add(stopModel);
                                }
                            }
                            if (statusCountList.Count() > 0)
                            {
                                //遍历统计事件总数
                                foreach (var item in statusCountList)
                                {
                                    planSta.eventTotalCount += item.statusCount;
                                }
                                planSta.completeCount = statusCountList.Where(p => p.status == 90 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 90 && p.stop == 0).FirstOrDefault().statusCount;
                                planSta.completeRate = (Convert.ToDouble(planSta.completeCount) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                                planSta.unConfirm = statusCountList.Where(p => p.status == 30 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 30 && p.stop == 0).FirstOrDefault().statusCount;
                                planSta.confirmRate = (Convert.ToDouble(planSta.unConfirm) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                                planSta.unCompleteCount = (statusCountList.Where(p => p.status == 20 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 20 && p.stop == 0).FirstOrDefault().statusCount)
                                                        + (statusCountList.Where(p => p.status == 40 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 40 && p.stop == 0).FirstOrDefault().statusCount);
                                planSta.unCompleteRate = (Convert.ToDouble(planSta.unCompleteCount) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                                planSta.submitCount = (statusCountList.Where(p => p.status == 25 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 25 && p.stop == 0).FirstOrDefault().statusCount)
                                                        + (statusCountList.Where(p => p.stop == 10).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.stop == 10).FirstOrDefault().statusCount)
                                                        + (statusCountList.Where(p => p.status == 10 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 10 && p.stop == 0).FirstOrDefault().statusCount);
                                planSta.submitRate = (Convert.ToDouble(planSta.submitCount) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                                planSta.unCommittedCount = (statusCountList.Where(p => p.status == 0 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 0 && p.stop == 0).FirstOrDefault().statusCount)
                                                        + (statusCountList.Where(p => p.status == 15 && p.stop == 0).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.status == 15 && p.stop == 0).FirstOrDefault().statusCount);

                                planSta.committedRate = (Convert.ToDouble(planSta.unCommittedCount) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                                planSta.stopCount = statusCountList.Where(p => p.stop == 90).FirstOrDefault() == null ? 0 : statusCountList.Where(p => p.stop == 90).FirstOrDefault().statusCount;
                                planSta.stopRate = (Convert.ToDouble(planSta.stopCount) / Convert.ToDouble(planSta.eventTotalCount) * 100).ToString("0.0");
                            }
                            else
                            {
                                planSta.eventTotalCount = 0;
                                planSta.completeCount = 0;
                                planSta.completeRate = "0.0";
                                planSta.unConfirm = 0;
                                planSta.confirmRate = "0.0";
                                planSta.unCompleteCount = 0;
                                planSta.unCompleteRate = "0.0";
                                planSta.submitCount = 0;
                                planSta.submitRate = "0.0";
                                planSta.unCommittedCount = 0;
                                planSta.committedRate = "0.0";
                                planSta.stopCount = 0;
                                planSta.stopRate = "0.0";
                            }
                        }
                    }
                    planStaList.AddRange(tempList);
                }
            }
            switch (sortby)
            {
                case 0:
                    planStaList = sort == 0 ? planStaList.OrderBy(p => p.eventTotalCount).ToList() : planStaList.OrderByDescending(p => p.eventTotalCount).ToList();
                    break;

                case 1:
                    planStaList = sort == 0 ? planStaList.OrderBy(p => p.completeCount).ToList() : planStaList.OrderByDescending(p => p.completeCount).ToList();
                    break;

                case 2:
                    planStaList = sort == 0 ? planStaList.OrderBy(p => Convert.ToDouble(p.completeRate)).ToList() : planStaList.OrderByDescending(p => Convert.ToDouble(p.completeRate)).ToList();
                    break;
            }
            return planStaList;
        }

        #endregion 按部门号获取该部门成员的计划完成情况

        #region 按统计号获取各部门计划完成情况

        /// <summary>
        /// 按统计号获取各部门计划完成情况
        /// </summary>
        /// <param name="statisticsId">统计Id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="sortby">排序字段</param>
        /// <param name="sortDirect">排序方向</param>
        /// <returns>部门完成情况集合</returns>
        public List<PlanStatusByOrgModel> GetStatusByOrg(int statisticsId, DateTime startTime, DateTime endTime, int sortby, int sortDirect)
        {
            var planStatusCounts = new List<PlanStatusByOrgModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                var orgIds = db.tblPlanStatisticsOrg.Where(p => p.statisticsId == statisticsId);
                if (orgIds.Count() > 0)
                {
                    foreach (var orgId in orgIds)
                    {
                        var planStatusOrg = new PlanStatusByOrgModel();
                        planStatusOrg.planCount = 0;
                        planStatusOrg.id = orgId.organizationId;
                        planStatusOrg.name = db.tblOrganization.Where(p => p.organizationId == orgId.organizationId).FirstOrDefault() == null ? "" : db.tblOrganization.Where(p => p.organizationId == orgId.organizationId).FirstOrDefault().organizationName;
                        var orgIdsNew = GetAllorganizationIds(orgId.organizationId, db);
                        var tempListNew = new List<Plan>();
                        //初始化tempListNew
                        tempListNew = AddPlanModel(tempListNew);
                        foreach (var item in orgIdsNew)
                        {
                            var tempList = new List<Plan>();
                            //初始化tempList
                            tempList = AddPlanModel(tempList);

                            var plans = from c in db.tblPlan
                                        where c.stop == 0 && c.endTime.Value >= startTime && c.endTime.Value < endTime && c.organizationId == item
                                        group c by c.status into g
                                        select new Plan
                                        {
                                            status = g.Key.Value,
                                            count = g.Count(),
                                            color = (g.Key.Value == 0 || g.Key.Value == 15) ? "#57acdb" : ((g.Key.Value == 10 || g.Key.Value == 25) ? "#e00e16" : ((g.Key.Value == 20 || g.Key.Value == 40) ? "#be1d9a" : (g.Key.Value == 30 ? "#fbab11" : "#58b557")))
                                        };
                            if (plans.Count() > 0)
                            {
                                //然后根据相同的颜色分组，分组后是集合套集合的结构
                                var lists = plans.GroupBy(p => p.color);
                                foreach (var items in lists)
                                {
                                    var plan = new Plan();
                                    plan.count = 0;
                                    foreach (var model in items)
                                    {
                                        plan.status = model.status;
                                        plan.count += model.count;
                                        planStatusOrg.planCount += model.count;
                                        plan.color = model.color;
                                    }
                                    if (plan != null)
                                    {
                                        tempList.Where(p => p.color == plan.color).FirstOrDefault().count = plan.count;
                                    }
                                }
                            }
                            var stops = from c in db.tblPlan
                                        where c.stop != 0 && c.endTime.Value >= startTime && c.endTime.Value < endTime && c.organizationId == item && !c.deleteFlag
                                        group c by c.stop into g
                                        select new Plan
                                        {
                                            status = g.Key.Value,
                                            count = g.Count(),
                                            color = (g.Key.Value == 10) ? "#e00e16" : "#49dca7"
                                        };
                            if (stops.Count() > 0)
                            {
                                if (tempList.Where(p => p.color == "#e00e16").FirstOrDefault() != null)
                                {
                                    planStatusOrg.planCount += stops.Where(p => p.color == "#e00e16").FirstOrDefault() == null ? 0 : stops.Where(p => p.color == "#e00e16").FirstOrDefault().count;
                                    tempList.Where(p => p.color == "#e00e16").FirstOrDefault().count += stops.Where(p => p.color == "#e00e16").FirstOrDefault() == null ? 0 : stops.Where(p => p.color == "#e00e16").FirstOrDefault().count;
                                }
                                else
                                {
                                    if (stops.Where(p => p.color == "#e00e16").FirstOrDefault() != null)
                                    {
                                        tempList.Add(stops.Where(p => p.color == "#e00e16").FirstOrDefault());
                                    }
                                }
                                if (stops.Where(p => p.status == 90).FirstOrDefault() != null)
                                {
                                    planStatusOrg.planCount += stops.Where(p => p.status == 90).FirstOrDefault().count;
                                    tempList.Add(stops.Where(p => p.status == 90).FirstOrDefault());
                                }
                            }
                            foreach (var lastModel in tempList)
                            {
                                if (tempListNew.Where(p => p.color == lastModel.color).FirstOrDefault() != null)
                                {
                                    tempListNew.Where(p => p.color == lastModel.color).FirstOrDefault().count += lastModel.count;
                                }
                            }
                        }
                        foreach (var nameModel in tempListNew)
                        {
                            nameModel.statusName = nameModel.color == "#57acdb" ? "待提交" : (nameModel.color == "#e00e16" ? "待审核" : (nameModel.color == "#be1d9a" ? "已审核" : (nameModel.color == "#fbab11" ? "待确认" : (nameModel.color == "#58b557" ? "已完成" : "已中止"))));
                        }
                        planStatusOrg.plans = tempListNew;
                        if (planStatusOrg.planCount == 0)
                        {
                            planStatusOrg.completeRate = 0;
                        }
                        else
                        {
                            planStatusOrg.completeRate = planStatusOrg.plans.Where(p => p.color == "#58b557").FirstOrDefault() == null ? 0 : (Convert.ToDouble(planStatusOrg.plans.Where(p => p.color == "#58b557").FirstOrDefault().count) / Convert.ToDouble(planStatusOrg.planCount));
                        }
                        planStatusCounts.Add(planStatusOrg);
                    }
                }
            }

            switch (sortby)
            {
                case 0:
                    planStatusCounts = (sortDirect == 0 ? planStatusCounts.OrderBy(p => p.planCount) : planStatusCounts.OrderByDescending(p => p.planCount)).ToList();
                    break;

                case 1:
                    planStatusCounts = (sortDirect == 0 ? planStatusCounts.OrderBy(p => p.completeRate) : planStatusCounts.OrderByDescending(p => p.completeRate)).ToList();
                    break;
            }
            //if (planStatusCounts.Count()<=1)
            //{
            //    foreach (var planStatusCount in planStatusCounts)
            //    {
            //        var s = planStatusCount.plans.Where(p => p.count == 0).Count();
            //        if (planStatusCount.plans.Where(p => p.count == 0).Count() == 6)
            //        {
            //            return null;
            //        }
            //    }

            //}
            return planStatusCounts;
        }

        #endregion 按统计号获取各部门计划完成情况

        #region 初始化一个状态统计list

        /// <summary>
        /// 初始化一个状态统计list
        /// </summary>
        /// <param name="tempList"></param>
        /// <returns></returns>
        public List<Plan> AddPlanModel(List<Plan> tempList)
        {
            tempList.Add(new Plan
            {
                status = 15,
                color = "#57acdb",
                count = 0
            });
            tempList.Add(new Plan
            {
                status = 25,
                color = "#e00e16",
                count = 0
            });
            tempList.Add(new Plan
            {
                status = 40,
                color = "#be1d9a",
                count = 0
            });
            tempList.Add(new Plan
            {
                status = 30,
                color = "#fbab11",
                count = 0
            });
            tempList.Add(new Plan
            {
                status = 90,
                color = "#58b557",
                count = 0
            });
            tempList.Add(new Plan
            {
                status = 90,
                color = "#49dca7",
                count = 0
            });
            return tempList;
        }

        #endregion 初始化一个状态统计list

        #region 获取统计名信息

        /// <summary>
        /// 获取统计名信息
        /// </summary>
        /// <returns>统计名信息列表</returns>
        public List<StatisticsInfo> GetStatisticsList()
        {
            var statisticsList = new List<StatisticsInfo>();
            using (var db = new TargetNavigationDBEntities())
            {
                statisticsList = (from c in db.tblPlanStatistics
                                  select new StatisticsInfo
                                  {
                                      statisticsId = c.statisticsId,
                                      statisticsName = c.statisticsName
                                  }).ToList();
            }
            return statisticsList;
        }

        #endregion 获取统计名信息

        #region 递归获取所有的相关部门Id

        /// <summary>
        /// 递归获取所有的相关部门Id
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<int> GetAllorganizationIds(int organizationId, TargetNavigationDBEntities db)
        {
            var organizationIds = new List<int>();
            organizationIds.Add(organizationId);
            var organizationList = (from c in db.tblOrganization where c.parentOrganization == organizationId select c.organizationId).ToList();
            if (organizationList.Count() > 0)
            {
                foreach (var item in organizationList)
                {
                    organizationIds.Add(item);
                    var organizationIdsTemp = GetAllorganizationIds(item, db);
                    if (organizationIdsTemp.Count <= 0) continue;
                }
                return organizationIds;
            }
            else
            {
                return organizationIds;
            }
        }

        #endregion 递归获取所有的相关部门Id
    }
}