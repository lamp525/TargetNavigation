using MB.DAL;
using MB.New.BLL.Calendar;
using MB.New.BLL.Plan;
using MB.New.BLL.User;
using MB.New.BLL.WorkTime;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.Facade.Index
{
    public class UserIndexFacade : IUserIndexFacade
    {
        /// <summary>
        /// 取得用户工作信息
        /// </summary>
        /// <param name="userId">登录用户ID</param>
        /// <returns></returns>
        public PageWorkTimeInfoModel GetUserWorkTimeInfo(int userId)
        {
            IUserBLL userBLL = new UserBLL();
            IWorkTimeBLL workTimeBLL = new WorkTimeBLL();
            IHolidayBLL holidayBLL = new HolidayBLL();
            PageWorkTimeInfoModel workTimeModel = new PageWorkTimeInfoModel();

            var now = DateTime.Now;
            var thisMonth = now.ToString("yyyyMM");

            // 取得本周第一天
            var thisweekMonday = DateUtility.GetWeekFirstDay(now);
            // 取得本周最后一天
            var thisweekSunday = DateUtility.GetWeekLastDay(now).AddHours(23);
            // 取得第几周
            var thisWeekNum = DateUtility.GetWeekOfYear(thisweekMonday);

            using (var db = new TargetNavigationDBEntities())
            {
                // 获取个人月有效工时
                var date = workTimeBLL.GetPerWorkTimeByMonth(db, userId, thisMonth);
                workTimeModel.monthTotalWorkTime = Math.Round(date / 60, 1);

                // 获取个人昨天有效工时
                var yesTodayModel = workTimeBLL.GetPerWorkTimeByDay(db, userId, now.AddDays(-1));
                workTimeModel.yesterdayWorkTime = Math.Round((decimal)yesTodayModel / 60, 1);

                // 获取个人周有效工时
                var thisweekWorkTime = workTimeBLL.GetPerWorkTimeByWeek(db, userId, now.Year, thisWeekNum);
                workTimeModel.weekTotalWorkTime = Math.Round((decimal)thisweekWorkTime / 60, 1);

                // 取得目前为止的工作日天数
                var workDays = holidayBLL.GetWorkdaysCount(db, DateUtility.GetWeekFirstDay(now), now);
                workDays = workDays == 0 ? 1 : workDays;
                // 获取周平均
                workTimeModel.weekAvgWorkTIme = Math.Round(workTimeModel.weekTotalWorkTime.Value / workDays, 1);

                // 获取头像地址
                workTimeModel.imgUrl = userBLL.GetUserInfoById(db, userId).headImage;
            }
            return workTimeModel;
        }

        /// <summary>
        /// 用户计划完成情况
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        public PagePlanCompleteCountModel GetUserPlanCompleteInfo(int userId, EnumDefine.StatisticsType statisticsType)
        {
            IPlanBLL planBLL = new PlanBLL();
            // 开始时间
            var fromTime = DateTime.Now;
            // 结束时间
            var toTime = DateTime.Now;

            var pageModel = new PagePlanCompleteCountModel();
            PlanCompleteCountModel planCompleteCountModel = null;

            switch (statisticsType)
            {
                // 周
                case EnumDefine.StatisticsType.Week:
                    fromTime = DateUtility.GetWeekFirstDay(fromTime);
                    toTime = DateUtility.GetWeekLastDay(toTime).AddHours(23);
                    break;
                // 月
                case EnumDefine.StatisticsType.Month:
                    fromTime = DateUtility.GetMonthFirstDay(fromTime);
                    toTime = DateUtility.GetMonthLastDay(toTime).AddHours(23);
                    break;

                default:
                    return pageModel;
            }

            // 取得时间范围内完成未完成数量
            using (var db = new TargetNavigationDBEntities())
            {
                planCompleteCountModel = planBLL.GetUserPlanCompleteInfo(db, userId, fromTime, toTime);
            }

            // 结果映射
            ModelMapping.SingleMapping(planCompleteCountModel, pageModel);

            return pageModel;
        }

        /// <summary>
        /// 个人首页-计划状态数据获取
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="statisticsType">是否是周状态统计</param>
        /// <returns></returns>
        public PagePlanStatusCountModel GetUserPlanStatusInfo(int userId, EnumDefine.StatisticsType statisticsType)
        {
            IPlanBLL planBLL = new PlanBLL();
            // 开始时间
            var fromTime = DateTime.Now;
            // 结束时间
            var toTime = DateTime.Now;

            var pageModel = new PagePlanStatusCountModel();
            PlanStatusCountModel planStatusCountModel = null;

            switch (statisticsType)
            {
                // 周
                case EnumDefine.StatisticsType.Week:
                    fromTime = DateUtility.GetWeekFirstDay(fromTime);
                    toTime = DateUtility.GetWeekLastDay(toTime);
                    break;
                // 月
                case EnumDefine.StatisticsType.Month:
                    fromTime = DateUtility.GetMonthFirstDay(fromTime);
                    toTime = DateUtility.GetMonthLastDay(toTime);
                    break;

                default:
                    return pageModel;
            }

            // 取得状态分组的计划数据
            using (var db = new TargetNavigationDBEntities())
            {
                planStatusCountModel = planBLL.GetMyPlanStatusInfo(db, userId, fromTime, toTime);
            }

            if (planStatusCountModel == null)
            {
                // 待审核计划数量
                pageModel.checkingCount = 0;
                // 待提交计划数量
                pageModel.submitingCount = 0;
                // 进行中计划数量
                pageModel.checkedCount = 0;
                // 待确认计划数量
                pageModel.confirmingCount = 0;
            }
            else {
                // 待审核计划数量
                pageModel.checkingCount = planStatusCountModel.planCheckingCount + planStatusCountModel.loopCheckingCount;
                // 待提交计划数量
                pageModel.submitingCount = planStatusCountModel.planSubmitingCount + planStatusCountModel.loopSubmitingCount;
                // 进行中计划数量
                pageModel.checkedCount = planStatusCountModel.planCheckedCount + planStatusCountModel.loopCheckedCount;
                // 待确认计划数量
                pageModel.confirmingCount = planStatusCountModel.planConfirmingCount + planStatusCountModel.loopConfirmingCount;
            }

            // 对数据状态进行重新分组计算
            return pageModel;
        }

        /// <summary>
        /// 首页计划列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planListType"></param>
        /// <returns></returns>
        public List<PagePlanInfoModel> GetNeedToDoPlanList(int userId, EnumDefine.PlanListType planListType)
        {
            IPlanBLL Plan = new PlanBLL();
            List<PlanInfoModel> planInfo = null;
            List<LoopPlanInfoModel> loopPlanInfo = null;

            List<PagePlanInfoModel> result = new List<PagePlanInfoModel>();

            // 声明检索用对象
            PlanSearchModel planSearchInfo = null;
            PlanSortInfoModel sortInfo = null;

            switch (planListType)
            {
                // 我的计划
                case EnumDefine.PlanListType.Mine:

                    planSearchInfo = new PlanSearchModel();

                    planSearchInfo.userId = userId;
                    planSearchInfo.page = -1;

                    // 排序信息
                    planSearchInfo.sortInfo = new List<PlanSortInfoModel>();
                    // 排序信息-完成时间
                    sortInfo = new PlanSortInfoModel();
                    sortInfo.orderType = EnumDefine.OrderType.Desc;
                    sortInfo.sortType = EnumDefine.PlanSortType.Time;
                    planSearchInfo.sortInfo.Add(sortInfo);

                    // 排序信息-确认人
                    sortInfo = new PlanSortInfoModel();
                    sortInfo.orderType = EnumDefine.OrderType.Asc;
                    sortInfo.sortType = EnumDefine.PlanSortType.ConfirmUser;
                    planSearchInfo.sortInfo.Add(sortInfo);

                    // 筛选信息
                    planSearchInfo.filterInfo = new PlanFilterInfoModel();
                    // 筛选信息-状态
                    planSearchInfo.filterInfo.status = new List<EnumDefine.PlanPageStatus>() { EnumDefine.PlanPageStatus.UnSubmit, EnumDefine.PlanPageStatus.Running };

                    // 数据检索
                    using (var db = new TargetNavigationDBEntities())
                    {
                        //取得我的一般计划信息
                        planInfo = Plan.GetMyPlanList(db, planSearchInfo);

                        //取得我的循环计划信息
                        loopPlanInfo = Plan.GetMyLoopPlanList(db, planSearchInfo);
                    }
                    break;

                // 下属计划
                case EnumDefine.PlanListType.Subordinate:

                    planSearchInfo = new PlanSearchModel();

                    planSearchInfo.userId = userId;
                    planSearchInfo.page = -1;

                    // 排序信息
                    planSearchInfo.sortInfo = new List<PlanSortInfoModel>();
                    // 排序信息-完成时间
                    sortInfo = new PlanSortInfoModel();
                    sortInfo.orderType = EnumDefine.OrderType.Desc;
                    sortInfo.sortType = EnumDefine.PlanSortType.Time;
                    planSearchInfo.sortInfo.Add(sortInfo);

                    // 排序信息-责任人
                    sortInfo = new PlanSortInfoModel();
                    sortInfo.orderType = EnumDefine.OrderType.Asc;
                    sortInfo.sortType = EnumDefine.PlanSortType.ResponsibleUser;
                    planSearchInfo.sortInfo.Add(sortInfo);

                    // 筛选信息
                    planSearchInfo.filterInfo = new PlanFilterInfoModel();
                    // 筛选信息-状态
                    planSearchInfo.filterInfo.status = new List<EnumDefine.PlanPageStatus>() { EnumDefine.PlanPageStatus.Checking, EnumDefine.PlanPageStatus.Confirming };

                    // 数据检索
                    using (var db = new TargetNavigationDBEntities())
                    {
                        //取得下属一般计划信息
                        planInfo = Plan.GetSubordinatePlanList(db, planSearchInfo);

                        //取得下属循环计划信息
                        loopPlanInfo = Plan.GetSubordinateLoopPlanList(db, planSearchInfo);
                    }
                    break;
     
            }

            result.AddRange( ModelMapping.JsonMapping<List<PlanInfoModel>, List<PagePlanInfoModel>>(planInfo));
            result.AddRange(ModelMapping.JsonMapping<List<LoopPlanInfoModel>, List<PagePlanInfoModel>>(loopPlanInfo));

            return result;
        }

        /// <summary>
        /// 激励
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        public PageIncentiveInfoModel GetUserIncentiveInfo(int userId, EnumDefine.StatisticsType statisticsType)
        {
            PageIncentiveInfoModel pageModel = new PageIncentiveInfoModel();
            pageModel.rewardAmount = 0;
            pageModel.punishmentAmount = 0;

            return pageModel;
        }

        /// <summary>
        /// 折线图
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        public List<PageWorkTimeStatisticsModel> GetUserWorkTimeStatistics(int userId, EnumDefine.StatisticsType statisticsType)
        {
            DateTime now = DateTime.Now;
            IWorkTimeBLL workTimeBLL = new WorkTimeBLL();
            IHolidayBLL holidayBLL = new HolidayBLL();
            List<PageWorkTimeStatisticsModel> pageModel = new List<PageWorkTimeStatisticsModel>();

            PageWorkTimeStatisticsModel pageWorkTimeStatisticsModel = null;
            WorkTimeStatisticsModel workTimeStatisticsModel = null;

            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                switch (statisticsType)
                {
                    case EnumDefine.StatisticsType.Week:
                        // 取得本周第一天
                        var date = DateUtility.GetWeekFirstDay(now);

                        // 循环取得本周每一天
                        for (int i = 0; i <= 6; i++)
                        {
                            pageWorkTimeStatisticsModel = new PageWorkTimeStatisticsModel();

                            // 取得对应日期
                           var thisdate = date.AddDays(i);

                            // 取得指定时间的有效工时、实际工时
                            workTimeStatisticsModel = workTimeBLL.GetPerTimeByDay(db, userId, thisdate);

                            // 对应日期
                            pageWorkTimeStatisticsModel.workdate = i.ToString();

                            // 对应数据不存在时
                            if (workTimeStatisticsModel == null)
                            {
                                pageWorkTimeStatisticsModel.effectiveWorkTime = 0;
                                pageWorkTimeStatisticsModel.actualWorkTime = 0;
                            }
                            else
                            {
                                // 有效工时
                                pageWorkTimeStatisticsModel.effectiveWorkTime = workTimeStatisticsModel.effectiveWorkTime == null ? 0 : Math.Round(workTimeStatisticsModel.effectiveWorkTime.Value / 60,2);
                                // 实际工时
                                pageWorkTimeStatisticsModel.actualWorkTime = workTimeStatisticsModel.actualWorkTime == null ? 0 : Math.Round(workTimeStatisticsModel.actualWorkTime.Value / 60,2);
                            }

                            pageModel.Add(pageWorkTimeStatisticsModel);
                        }

                        break;

                    case EnumDefine.StatisticsType.Month:

                        // 取得本月第一周
                        int monthFirstWeek = DateUtility.GetWeekOfYear(DateUtility.GetMonthFirstDay(now));

                        // 取得本月最后一周
                        int monthLastWeek = DateUtility.GetWeekOfYear(DateUtility.GetMonthLastDay(now));

                        for (int i = monthFirstWeek; i <= monthLastWeek; i++)
                        {
                            pageWorkTimeStatisticsModel = new PageWorkTimeStatisticsModel();

                            // 取得指定时间的有效工时、实际工时
                            workTimeStatisticsModel = workTimeBLL.GetPerTimeByWeek(db, userId, now.Year, i);

                            // 对应周
                            pageWorkTimeStatisticsModel.workdate = i.ToString();

                            // 对应数据不存在时
                            if (workTimeStatisticsModel == null)
                            {
                                pageWorkTimeStatisticsModel.effectiveWorkTime = 0;
                                pageWorkTimeStatisticsModel.actualWorkTime = 0;
                            }
                            else
                            {
                                // 取得周对应的日期范围
                                DateTime[] dateRange = DateUtility.GetWeekRange(now.Year, i);
                                var workDays = holidayBLL.GetWorkdaysCount(db, dateRange[0], dateRange[1]);
                                workDays = workDays == 0 ? 5 : workDays;

                                // 有效工时
                                pageWorkTimeStatisticsModel.effectiveWorkTime = workTimeStatisticsModel.effectiveWorkTime == null ? 0 : Math.Round(workTimeStatisticsModel.effectiveWorkTime.Value / (60 * workDays),2);
                                // 实际工时
                                pageWorkTimeStatisticsModel.actualWorkTime = workTimeStatisticsModel.actualWorkTime == null ? 0 : Math.Round(workTimeStatisticsModel.actualWorkTime.Value / (60 * workDays),2);
                            }

                            pageModel.Add(pageWorkTimeStatisticsModel);
                        }

                        break;

                    default:
                        return pageModel;
                }
            }
            return pageModel;
        }

        /// <summary>
        /// 获取绩效排行
        /// </summary>
        /// <param name="statisticsType"></param>
        /// <returns></returns>
        public List<PagePerformanceModel> GetPerformanceRankInfo(EnumDefine.StatisticsType statisticsType)
        {
            IWorkTimeBLL workTimeBLL = new WorkTimeBLL();
            IHolidayBLL holidayBLL = new HolidayBLL();
            var orgWorkTimeList = new List<PagePerformanceModel>();
            List<UserOrgWorkTimeModel> userOrgWorkTimeModel = null;

            DateTime now = DateTime.Now;
            var workDays = 0;

            using (var db = new TargetNavigationDBEntities())
            {
                switch (statisticsType)
                {
                    case EnumDefine.StatisticsType.Week:
                        // 取得周对应的工作日天数
                        workDays = holidayBLL.GetWorkdaysCount(db, DateUtility.GetWeekFirstDay(now), now);
                        workDays = workDays == 0 ? 1 : workDays;

                        // 取得周有效工时排行
                        userOrgWorkTimeModel = workTimeBLL.GetOrgWorkTimeByWeek(db, now.Year, DateUtility.GetWeekOfYear(now));

                        break;

                    case EnumDefine.StatisticsType.Month:
                        // 取得月对应的工作日天数
                        workDays = holidayBLL.GetWorkdaysCount(db, DateUtility.GetMonthFirstDay(now), now);
                        workDays = workDays == 0 ? 1 : workDays;

                        // 取得月有效工时排行
                        userOrgWorkTimeModel = workTimeBLL.GetOrgWorkTimeByMonth(db, now.ToString("yyyyMM"));

                        break;

                    default:
                        return orgWorkTimeList;
                }
            }

            // 计算有效工时
            orgWorkTimeList = userOrgWorkTimeModel.Select(x => new PagePerformanceModel
            {
                headImage = x.headImage,
                userName = x.userName,
                workTime = Math.Round(x.workTime.Value / workDays, 1)
            }).OrderByDescending(x => x.workTime).ToList();

            return orgWorkTimeList.OrderByDescending(s => s.workTime).ToList();
        }
    }
}