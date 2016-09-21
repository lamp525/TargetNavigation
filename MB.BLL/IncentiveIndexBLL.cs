using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MB.Common;
using MB.DAL;
using MB.Model;
using MB.Model.UserIndexModels;

namespace MB.BLL
{
    public class IncentiveIndexBLL : IIncentiveIndexBLL
    {
        #region 获取每月奖惩数

        /// <summary>
        /// 获取每月奖惩数
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="userId">人员Id</param>
        /// <returns></returns>
        public List<RewardPunishNum> GetRewardPunishNum(int? year, int userId)
        {
            var modelList = new List<RewardPunishNum>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (year == null)
                {
                    var yearList = new int[DateTime.Now.Year - 2000];
                    for (int i = 0; i <= yearList.Count(); i++)
                    {
                        var model = new RewardPunishNum
                        {
                            year = 2000 + i,
                            number = RewardMoney(userId, 2000 + i, null)
                        };
                        modelList.Add(model);
                    }
                }
                else
                {
                    var monthList = new int[12];
                    for (int i = 1; i <= monthList.Count(); i++)
                    {
                        var model = new RewardPunishNum
                        {
                            month = i,
                            number = RewardMoney(userId, year, i)
                        };
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }

        #endregion 获取每月奖惩数

        #region 获取激励详细情况

        /// <summary>
        /// 获取激励详细情况
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月</param>
        /// <param name="userId">人员Id</param>
        public RewardPunishDetail GetRewardPunishDetail(int year, int? month, int userId)
        {
            var model = new RewardPunishDetail();
            var list = new List<PerWorkTimeModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {
                    var rewardNum = this.GetTimeReward(userId, year, null);
                    if (rewardNum != 0)
                    {
                        //有效工时奖惩金额
                        model.timeReward = rewardNum;
                    }

                    list = this.GetPerMonthWorkTime(userId, year.ToString());
                    if (list.Count != 0)
                    {
                        //平均有效工时
                        model.avgTime = list.Sum(c => c.effectiveTimeSum) / GetHoliday(year, null);
                    }
                    //年的场合
                    model.detail = (from r in db.tblRewardPunish
                                    where r.userId == userId && r.delayTime.Value.Year == year
                                    orderby r.statisticalTime descending
                                    select new RewardPunishModel
                                    {
                                        timeOutDay = r.timeoutDay.Value,
                                        targetId = r.targetId.Value,
                                        type = r.type,
                                        deductionMode = r.deductionMode,
                                        deductionNum = r.deductionNum,
                                        delayTime = r.delayTime
                                    }).Take(20).ToList();
                    foreach (var item in model.detail)
                    {
                        if (item.type < 4)
                        {//计划名称
                            item.targetName = db.tblPlan.Where(c => c.planId == item.targetId).Select(c => c.eventOutput).FirstOrDefault();
                        }
                        else if (item.type == 4)
                        {//表单抄送节点状态
                            var num = db.tblFormDuplicate.Where(c => c.formId == item.targetId).Select(c => c.alreadyRead).FirstOrDefault();
                            switch (num)
                            {
                                case 2: item.targetName = "审批"; break;
                                case 3: item.targetName = "会签"; break;
                            }
                        }
                        else if (item.type > 4)
                        {//目标名称
                            item.targetName = db.tblObjective.Where(c => c.objectiveId == item.targetId).Select(c => c.objectiveName).FirstOrDefault();
                        }
                    }
                }
                else
                {
                    var monthNum = this.GetTimeReward(userId, year, month);
                    //月的场合
                    if (monthNum != 0)
                    {
                        //有效工时奖惩金额
                        model.timeReward = monthNum;
                    }
                    list = this.GetPerMonthWorkTime(userId, year.ToString(), month.ToString());
                    if (list.Count != 0)
                    {
                        //平均有效工时
                        model.avgTime = list.Sum(c => c.effectiveTimeSum) / GetHoliday(year, month);
                    }

                    model.detail = (from r in db.tblRewardPunish
                                    join user in db.tblUser on r.userId equals user.userId
                                    where r.userId == userId && r.delayTime.Value.Year == year && r.delayTime.Value.Month == month
                                    select new RewardPunishModel
                                    {
                                        timeOutDay = r.timeoutDay.Value,
                                        targetId = r.targetId.Value,
                                        type = r.type,
                                        deductionMode = r.deductionMode,
                                        deductionNum = r.deductionNum,
                                        delayTime = r.delayTime
                                    }).ToList();
                    foreach (var item in model.detail)
                    {
                        if (item.type < 4)
                        {//计划名称
                            item.targetName = db.tblPlan.Where(c => c.planId == item.targetId).Select(c => c.eventOutput).FirstOrDefault();
                        }
                        else if (item.type == 4)
                        {//流程名称
                            var num = db.tblFormDuplicate.Where(c => c.formId == item.targetId).Select(c => c.alreadyRead).FirstOrDefault();
                            switch (num)
                            {
                                case 2: item.targetName = "审批"; break;
                                case 3: item.targetName = "会签"; break;
                            }
                        }
                        else if (item.type > 4)
                        {//目标名称
                            item.targetName = db.tblObjective.Where(c => c.objectiveId == item.targetId).Select(c => c.objectiveName).FirstOrDefault();
                        }
                    }
                }
            }
            return model;
        }

        #endregion 获取激励详细情况

        #region 获取图表数据

        /// <summary>
        /// 获取图表数据
        /// </summary>
        /// <param name="type">1:计划执行力 2:流程执行力 3:功效价值 4:目标价值</param>
        /// <param name="year">年的场合</param>
        /// <param name="month">月的场合</param>
        /// <param name="userId">人员Id</param>
        public List<IncentiveDataModel> GetIncentiveData(int type, int year, int? month, int userId)
        {
            var list = new List<IncentiveDataModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {
                    list = GetList(type, year, null, userId, db);
                }
                else
                {
                    list = GetList(type, year, month, userId, db);
                }
            }
            return list;
        }

        #region 获取计划执行力、流程执行力、功效价值、目标价值信息

        private List<IncentiveDataModel> GetList(int type, int year, int? month, int userId, TargetNavigationDBEntities db)
        {
            var list = new List<IncentiveDataModel>();
            if (month == null)
            {
                switch (type)
                {
                    //计划执行力
                    case 1: var planList = this.GetPlanCompleteStatistics(userId, year.ToString(), null);
                        foreach (var item in planList)
                        {
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.strname),
                                count = item.planCount,
                                completeCount = item.completeCount,
                                //超时数
                                timeoutCount = item.timeOutCount,
                                completeRate = (decimal)item.completeCount / (decimal)item.planCount,
                                //超时率
                                timeoutRate = (decimal)item.timeOutCount / (decimal)item.planCount
                            };
                            list.Add(model);
                        }; break;
                    //流程执行力
                    case 2: var FormFlow = this.GetFormDuplicate(userId, year, null);
                        list = FormFlow.GroupBy(c => c.FFcreateTime.Month).Select(c => new IncentiveDataModel
                        {
                            name = c.Key,
                            count = c.Count(),
                            timeoutCount = c.Sum(x => (x.FFcreateTime - x.FDcreateTime).Days),
                            timeoutRate = (decimal)c.Sum(x => (x.FFcreateTime - x.FDcreateTime).Days) / (decimal)c.Count()
                        }).ToList();
                        //foreach (var item in FormFlow)
                        //{
                        //    var model = new IncentiveDataModel
                        //    {
                        //        name = item.FFcreateTime.Month,
                        //        count = FormFlow.Count(),
                        //        //审批超时天数
                        //        timeoutCount = (item.FFcreateTime - item.FDcreateTime).Days,
                        //        //审批超时率
                        //        timeoutRate = (item.FFcreateTime - item.FDcreateTime).Days / FormFlow.Count()
                        //    };
                        //    list.Add(model);
                        //}

                        ; break;
                    //功效价值
                    case 3: var modelList = GetPerMonthWorkTime(userId, year.ToString());
                        foreach (var item in modelList)
                        {
                            var e = Convert.ToInt32(item.effectiveTimeSum);
                            var w = Convert.ToInt32(item.workTimeSum);
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.name),
                                //有效工时
                                completeCount = e,
                                //实际工时
                                timeoutCount = w,
                                //效率系数
                                timeoutRate = item.workTimeSum / item.effectiveTimeSum
                            };
                            list.Add(model);
                        }

                        ; break;
                    //目标价值
                    case 4:
                        var objectiveList = this.GetTargetCompleteStatistics(userId, year.ToString(), null);
                        foreach (var item in objectiveList)
                        {
                            if (item.strname == "")
                            {
                                item.strname = "0";
                            }
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.strname),
                                count = item.planCount.Value,
                                completeCount = item.completeCount.Value,
                                timeoutCount = item.timeOutCount.Value,
                                completeRate = (decimal)item.completeCount.Value / (decimal)item.planCount.Value,
                                timeoutRate = (decimal)item.timeOutCount.Value / (decimal)item.planCount.Value
                            };
                            list.Add(model);
                        }
                        ; break;
                }
            }
            else
            {
                switch (type)
                {
                    //计划执行力
                    case 1: var planList = this.GetPlanCompleteStatistics(userId, year.ToString(), month.ToString());
                        foreach (var item in planList)
                        {
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.strname),
                                count = item.planCount,
                                completeCount = item.completeCount,
                                //超时数
                                timeoutCount = item.timeOutCount,
                                completeRate = (decimal)item.completeCount / (decimal)item.planCount,
                                //超时率
                                timeoutRate = (decimal)item.timeOutCount / (decimal)item.planCount
                            };
                            list.Add(model);
                        }; break;
                    //流程执行力
                    case 2: var FormFlow = this.GetFormDuplicate(userId, year, month);
                        list = FormFlow.GroupBy(c => c.FFcreateTime.Month).Select(c => new IncentiveDataModel
                        {
                            name = c.Key,
                            count = c.Count(),
                            timeoutCount = c.Sum(x => (x.FFcreateTime - x.FDcreateTime).Days),
                            timeoutRate = (decimal)c.Sum(x => (x.FFcreateTime - x.FDcreateTime).Days) / (decimal)c.Count()
                        }).ToList();
                        //foreach (var item in FormFlow)
                        //{
                        //    var model = new IncentiveDataModel
                        //    {
                        //        name = item.FFcreateTime.Day,
                        //        count = FormFlow.Count(),
                        //        //审批超时天数
                        //        timeoutCount = (item.FFcreateTime - item.FDcreateTime).Days,
                        //        //审批超时率
                        //        timeoutRate = (decimal)(item.FFcreateTime - item.FDcreateTime).Days / (decimal)FormFlow.Count()
                        //    };
                        //    list.Add(model);
                        //}

                        ; break;
                    //功效价值
                    case 3: var modelList = GetPerMonthWorkTime(userId, year.ToString(), month.ToString());
                        foreach (var item in modelList)
                        {
                            var e = Convert.ToInt32(item.effectiveTimeSum);
                            var w = Convert.ToInt32(item.workTimeSum);
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.name),
                                //有效工时
                                completeCount = e,
                                //实际工时
                                timeoutCount = w,
                                //效率系数
                                timeoutRate = item.workTimeSum / item.effectiveTimeSum
                            };
                            list.Add(model);
                        }
                        ; break;
                    //目标价值
                    case 4:
                        var objectiveList = this.GetTargetCompleteStatistics(userId, year.ToString(), month.ToString());
                        foreach (var item in objectiveList)
                        {
                            if (item.strname == "")
                            {
                                item.strname = "0";
                            }
                            var model = new IncentiveDataModel
                            {
                                name = int.Parse(item.strname),
                                count = item.planCount.Value,
                                completeCount = item.completeCount.Value,
                                timeoutCount = item.timeOutCount.Value,
                                completeRate = (decimal)item.completeCount.Value / (decimal)item.planCount.Value,
                                timeoutRate = (decimal)item.timeOutCount.Value / (decimal)item.planCount.Value
                            };
                            list.Add(model);
                        }
                        ; break;
                }
            }

            return list.OrderBy(c => c.name).ToList();
        }

        #endregion 获取计划执行力、流程执行力、功效价值、目标价值信息

        #endregion 获取图表数据

        #region 下属奖励列表

        /// <summary>
        /// 下属奖励列表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="userId"></param>
        public List<UnderReward> GetUnderReward(int? year, int? month, int? userId)
        {
            var list = new List<UnderReward>();
            using (var db = new TargetNavigationDBEntities())
            {
                list = GetList(userId.Value);
                foreach (var item in list)
                {
                    //判断该用户是否有下属
                    var isHave = GetList(item.userId);
                    if (isHave.Count > 0)
                    {
                        item.isParent = true;
                    }
                    //应得数
                    item.deservedNum = (int)RewardMoney(item.userId, year, month);
                }
            }
            return list;
        }

        #endregion 下属奖励列表

        #region 私有方法

        #region 用户有效工时

        /// <summary>
        /// 用户有效工时
        /// </summary>
        /// <param name="year">年的场合</param>
        /// <param name="month">月的场合</param>
        /// <returns></returns>
        private List<decimal> GetUserEffectiveTime(int userId, string year, string month = null)
        {
            var list = new List<decimal>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {
                    list = (from Time in db.tblPerDayWorkTime
                            where Time.userId == userId && Time.statisticalTime.IndexOf(year) != -1
                            select Time.effectiveTime).ToList();
                }
                else if (month != null && !month.Equals("0"))
                {
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    list = (from Time in db.tblPerDayWorkTime
                            where Time.userId == userId && Time.statisticalTime.IndexOf(year + month) != -1
                            select Time.effectiveTime).ToList();
                }
                else
                {
                    TimeSpan span = DateTime.Now - GetMondayDate(DateTime.Now);
                    for (int i = 0; i <= System.Math.Abs(span.Days); i++)
                    {
                        var model = db.tblPerDayWorkTime.Where(p => p.userId == userId && p.statisticalTime == GetMondayDate(DateTime.Now).AddDays(i).ToString("yyyyMMdd")).FirstOrDefault().effectiveTime;
                        if (model != null)
                        {
                            list.Add(model);
                        }
                    }
                }
            }
            return list;
        }

        #endregion 用户有效工时

        #region 工作日天数

        /// <summary>
        /// 工作日天数
        /// </summary>
        /// <returns></returns>
        private int GetHoliday(int year, int? month)
        {
            var day = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                var yDate = DateTime.Now.Year;
                var mDate = DateTime.Now.Month;
                var dDate = DateTime.Now.Day;
                var holiday = 0;
                if (month == null || month == 0)
                {
                    var time = 0;
                    if (year == yDate)
                    {
                        //小于当前月份的所有月份天数
                        for (int i = 1; i < mDate; i++)
                        {
                            time = GetHoliday(year, i);
                            day = day + time;
                        }

                        //当前月的放假天数
                        //holiday = (from d in db.tblHoliday
                        //           where d.holiday.Year == year && d.holiday.Month == month && d.holiday.Day < dDate
                        //           select d.holidayId).Count();
                        //小于等于当前月的天数和
                        day = day + dDate - holiday;
                    }
                    else
                    {
                        //年节假日
                        //day = (from d in db.tblHoliday
                        //       where d.holiday.Year == year
                        //       select d.holidayId).Count();
                        //年工作日
                        day = 365 - day;
                    }
                }
                else
                {
                    if (month == mDate)
                    {
                        //当前月的放假天数
                        //holiday = (from d in db.tblHoliday
                        //           where d.holiday.Year == year && d.holiday.Month == month && d.holiday.Day < dDate
                        //           select d.holidayId).Count();
                        //小于等于当前月的天数和
                        day = dDate - holiday;
                    }
                    else
                    {
                        //月节假日
                        //day = (from d in db.tblHoliday
                        //       where d.holiday.Year == year && d.holiday.Month == month
                        //       select d.holidayId).Count();
                        int monthDay = DateTime.DaysInMonth(year, month.Value);
                        //月工作日
                        day = monthDay - day;
                    }
                }
            }
            return day;
        }

        #endregion 工作日天数

        #region 价值激励自定义

        /// <summary>
        /// 价值激励自定义
        /// </summary>
        /// <param name="timeOut"></param>
        private ValueIncentiveCustomModel GetValueIncentiveCustom(decimal timeOut, decimal? salary)
        {
            var IncentiveCustomList = new ValueIncentiveCustomModel();
            using (var db = new TargetNavigationDBEntities())
            {
                if (timeOut > 0)
                {
                    IncentiveCustomList = (from c in db.tblValueIncentiveCustom
                                           where c.customType == 2 && c.customStartTime <= timeOut && c.customEndTime >= timeOut
                                           select new ValueIncentiveCustomModel
                                           {
                                               customId = c.customId,
                                               customType = c.customType,
                                               customStartTime = c.customStartTime.Value,
                                               customEndTime = c.customEndTime.Value,
                                               deductionMode = c.deductionMode,
                                               deductionNum = c.deductionNum
                                           }).FirstOrDefault();
                    if (IncentiveCustomList != null)
                    {
                        if (IncentiveCustomList.deductionMode == 2)
                        {
                            IncentiveCustomList.deductionNum = IncentiveCustomList.deductionNum / 100 * salary.Value;
                        }
                    }
                }
                else
                {
                    IncentiveCustomList = (from c in db.tblValueIncentiveCustom
                                           where c.customType == 1 && c.customStartTime <= Math.Abs(timeOut) && c.customEndTime >= Math.Abs(timeOut)
                                           select new ValueIncentiveCustomModel
                                           {
                                               customId = c.customId,
                                               customType = c.customType,
                                               customStartTime = c.customStartTime.Value,
                                               customEndTime = c.customEndTime.Value,
                                               deductionMode = c.deductionMode,
                                               deductionNum = c.deductionNum
                                           }).FirstOrDefault();
                    if (IncentiveCustomList != null)
                    {
                        if (IncentiveCustomList.deductionMode == 2)
                        {
                            IncentiveCustomList.deductionNum = IncentiveCustomList.deductionNum / 100 * salary.Value;
                        }
                    }
                }
            }
            return IncentiveCustomList;
        }

        #endregion 价值激励自定义

        #region 有效工时奖惩规则 获取奖惩金额

        /// <summary>
        ///有效工时奖惩规则 获取奖惩金额
        /// </summary>
        /// <returns></returns>
        private decimal GetTimeReward(int userId, int? year, int? month)
        {
            var timeReward = 0M;
            using (var db = new TargetNavigationDBEntities())
            {
                //获取价值激励列表
                var IncentiveModel = (from v in db.tblValueIncentive
                                      select new ValueIncentiveinfo
                                      {
                                          standardTime = v.standardTime,
                                          incentiveType = v.incentiveType,
                                          maxValueType = v.maxValueType,
                                          maxValue = v.maxValue,
                                          maxAverage = v.maxAverage,
                                          minValueType = v.minValueType,
                                          minValue = v.minValue,
                                          minAverage = v.minAverage
                                      }).FirstOrDefault();

                if (month == null)
                {
                    timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), null);
                }
                else if (month != null && month != 0)
                {
                    timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), month.ToString());
                }
                else
                {
                    timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), month.ToString());
                }
                if (timeReward != 0)
                {
                    return timeReward;
                }
            }

            return timeReward;
        }

        #endregion 有效工时奖惩规则 获取奖惩金额

        #region 获取超时天数，并算出奖励金额

        /// <summary>
        /// 获取超时天数，并算出奖励金额
        /// </summary>
        /// <param name="model"></param>
        /// <param name="timeOut"></param>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private decimal GetTimeout(ValueIncentiveinfo model, int userId, string year = null, string month = null)
        {
            var timeOut = 0M;
            var timeReward = 0M;
            using (var db = new TargetNavigationDBEntities())
            {
                //工资
                var salary = db.tblUser.Where(c => c.userId == userId).FirstOrDefault();
                if (model != null)
                {
                    switch (model.incentiveType)
                    {
                        //无的场合
                        case 0: timeReward = 0; break;
                        //按比例的场合
                        case 1: foreach (var y in this.GetUserEffectiveTime(userId, year, month))
                            {
                                timeOut = y / 60 - model.standardTime;

                                var reward = 0M;
                                if (timeOut > 0)
                                {//超出
                                    if (model.maxValueType == 1)
                                    {
                                        //金额类型
                                        //奖励金额
                                        reward = timeOut * model.maxAverage.Value;
                                    }
                                    else
                                    {
                                        if (salary.salary != null)
                                        {
                                            //百分比
                                            //奖励金额
                                            reward = timeOut * salary.salary.Value * model.maxAverage.Value;
                                        }
                                    }
                                    if (reward > model.maxValue)
                                    {
                                        reward = model.maxValue.Value;
                                    }
                                }
                                else
                                {
                                    //不足
                                    if (model.minValueType == 1)
                                    {
                                        //金额类型
                                        //奖励金额
                                        reward = timeOut * model.minAverage.Value;
                                    }
                                    else
                                    {
                                        if (salary.salary != null)
                                        {
                                            //百分比
                                            //奖励金额
                                            reward = timeOut * salary.salary.Value * model.minAverage.Value;
                                        }
                                    }
                                    if (reward > model.minValue)
                                    {
                                        reward = model.minValue.Value;
                                    }
                                }
                                timeReward = timeReward + reward;
                            }
                            ; break;
                        //自定义场合
                        case 2: foreach (var y in this.GetUserEffectiveTime(userId, year, month))
                            {
                                timeOut = y / 60 - model.standardTime;

                                var reward = 0M;
                                var ValueIncentiveCustomModel = this.GetValueIncentiveCustom(timeOut, salary.salary);
                                if (timeOut > 0)
                                {
                                    //超出
                                    if (ValueIncentiveCustomModel != null)
                                    {
                                        //金额
                                        reward = ValueIncentiveCustomModel.deductionNum.Value;
                                    }
                                }
                                else if (timeOut < 0)
                                {
                                    //不足

                                    if (ValueIncentiveCustomModel != null)
                                    {
                                        //金额
                                        reward = ValueIncentiveCustomModel.deductionNum.Value;
                                    }
                                }
                                timeReward = timeReward + reward;
                            }; break;
                    }
                }
            }
            return timeReward;
        }

        #endregion 获取超时天数，并算出奖励金额

        #region 执行力奖惩

        /// <summary>
        /// 执行力奖惩
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="year">年的场合</param>
        /// <param name="month">月的场合</param>
        /// <param name="day">日的场合</param>
        private List<RewardPunishModel> GetRewardPunish(int userId, int? year, int? month)
        {
            var list = new List<RewardPunishModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                //年的场合
                if (month == null)
                {
                    list = (from r in db.tblRewardPunish
                            where r.userId == userId && r.delayTime.Value.Year == year
                            select new RewardPunishModel
                            {
                                rewardId = r.rewardId,
                                userId = r.userId.Value,
                                type = r.type,
                                targetId = r.targetId.Value,
                                deductionMode = r.deductionMode,
                                deductionNum = r.deductionNum,
                                delayTime = r.delayTime
                            }).ToList();
                }
                else if (month != 0)
                {//月的场合
                    list = (from r in db.tblRewardPunish
                            where r.userId == userId && r.delayTime.Value.Year == year && r.delayTime.Value.Month == month
                            select new RewardPunishModel
                            {
                                rewardId = r.rewardId,
                                userId = r.userId.Value,
                                type = r.type,
                                targetId = r.targetId.Value,
                                deductionMode = r.deductionMode,
                                deductionNum = r.deductionNum,
                                delayTime = r.delayTime
                            }).ToList();
                }
                else
                {
                    //周场合
                    list = (from r in db.tblRewardPunish
                            where r.userId == userId && r.delayTime >= GetMondayDate(DateTime.Now) && r.delayTime <= GetMondayDate(DateTime.Now).AddDays(7)
                            select new RewardPunishModel
                            {
                                rewardId = r.rewardId,
                                userId = r.userId.Value,
                                type = r.type,
                                targetId = r.targetId.Value,
                                deductionMode = r.deductionMode,
                                deductionNum = r.deductionNum,
                                delayTime = r.delayTime
                            }).ToList();
                }
            }
            return list;
        }

        #endregion 执行力奖惩

        #region 目标价值奖惩

        /// <summary>
        /// 目标价值奖惩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        private decimal GetObjective(int userId, int? year, int? month)
        {
            var money = 0M;
            var timeReward = 0M;
            var list = new List<ObjectiveModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {
                    list = (from o in db.tblObjective
                            where o.responsibleUser == userId && o.actualEndTime.Value.Year == year
                            select new ObjectiveModel
                            {
                                objectiveId = o.objectiveId,
                                objectiveValue = o.objectiveValue,
                                expectedValue = o.expectedValue,
                                actualValue = o.actualValue,
                                formula = o.formula,
                                bonus = o.bonus,
                                checkType = o.checkType,
                                actualStartTime = o.actualStartTime,
                                actualEndTime = o.actualEndTime,
                                alarmTime = o.alarmTime,
                                weight = o.weight,
                                endTime = o.endTime
                            }).ToList();
                }
                else if (month != null && month != 0)
                {
                    list = (from o in db.tblObjective
                            where o.responsibleUser == userId && o.actualEndTime.Value.Year == year && o.actualEndTime.Value.Month == month
                            select new ObjectiveModel
                            {
                                objectiveId = o.objectiveId,
                                objectiveValue = o.objectiveValue,
                                expectedValue = o.expectedValue,
                                actualValue = o.actualValue,
                                formula = o.formula,
                                bonus = o.bonus,
                                checkType = o.checkType,
                                actualStartTime = o.actualStartTime,
                                actualEndTime = o.actualEndTime,
                                alarmTime = o.alarmTime,
                                weight = o.weight,
                                endTime = o.endTime
                            }).ToList();
                }
                else
                {
                    list = (from o in db.tblObjective
                            where o.responsibleUser == userId && o.actualEndTime >= GetMondayDate(DateTime.Now) && o.actualEndTime <= GetMondayDate(DateTime.Now).AddDays(7)
                            select new ObjectiveModel
                            {
                                objectiveId = o.objectiveId,
                                objectiveValue = o.objectiveValue,
                                expectedValue = o.expectedValue,
                                actualValue = o.actualValue,
                                formula = o.formula,
                                bonus = o.bonus,
                                checkType = o.checkType,
                                actualStartTime = o.actualStartTime,
                                actualEndTime = o.actualEndTime,
                                alarmTime = o.alarmTime,
                                weight = o.weight,
                                endTime = o.endTime
                            }).ToList();
                }

                foreach (var item in list)
                {
                    var timeOutDay = (item.actualEndTime.Value - item.endTime.Value).Days;
                    //目标公式编号
                    var formulaNumList = db.tblObjectiveFormula.Where(c => c.objectiveId == item.objectiveId).OrderBy(c => c.formulaNum).ThenBy(c => c.orderNum).Select(c => c.formulaNum).Distinct().ToList();
                    switch (item.formula)
                    {
                        //无的场合
                        case 0: timeReward = 0; break;
                        //默认公式的场合
                        case 1:
                            switch (item.checkType)
                            {
                                //金额
                                case 1:
                                    if (item.bonus != null && item.actualValue != null)
                                    {
                                        if (item.expectedValue != item.objectiveValue)
                                        {
                                            timeReward = item.bonus.Value * ((int.Parse(item.actualValue) - int.Parse(item.objectiveValue)) / (int.Parse(item.expectedValue) - int.Parse(item.objectiveValue)) + 1);
                                        }
                                    }
                                    ; break;
                                //时间
                                case 2:
                                    if (item.bonus != null && item.actualValue != null)
                                    {
                                        if (item.expectedValue != item.objectiveValue)
                                        {
                                            timeReward = item.bonus.Value * ((DateTime.Parse(item.actualValue) - DateTime.Parse(item.objectiveValue)).Days / (DateTime.Parse(item.expectedValue) - DateTime.Parse(item.objectiveValue)).Days + 1);
                                        }
                                    }
                                    ; break;
                                //数量
                                case 3:
                                    if (item.bonus != null && item.actualValue != null)
                                    {
                                        if (item.expectedValue != item.objectiveValue)
                                        {
                                            timeReward = item.bonus.Value * ((int.Parse(item.actualValue) - int.Parse(item.objectiveValue)) / (int.Parse(item.expectedValue) - int.Parse(item.objectiveValue)) + 1);
                                        }
                                    }
                                    ; break;
                            }
                            if (timeOutDay > 0)
                            {//惩罚
                                if (timeReward > item.minValue * item.bonus)
                                {
                                    timeReward = (decimal)(item.minValue * item.bonus);
                                }
                            }
                            else
                            { //奖励
                                if (timeReward > item.maxValue * item.bonus)
                                {
                                    timeReward = (decimal)(item.maxValue * item.bonus);
                                }
                            }
                            ; break;
                        //自定义的场合
                        case 2:
                            switch (item.checkType)
                            {
                                //金额
                                case 1:
                                    if (item.actualValue != null)
                                    {
                                        timeReward = this.GetFromualString(formulaNumList, item, db);
                                    }
                                    ; break;
                                //时间
                                case 2:
                                    if (item.actualValue != null)
                                    {
                                        var actualValue = item.actualValue.Replace("-", "").Replace("/", "");
                                        var objectiveValue = item.objectiveValue.Replace("-", "").Replace("/", "");
                                        var expectedValue = item.expectedValue.Replace("-", "").Replace("/", "");
                                        timeReward = this.GetFromualString(formulaNumList, item, db);
                                    }
                                    ; break;
                                //数字
                                case 3:
                                    if (item.actualValue != null)
                                    {
                                        timeReward = this.GetFromualString(formulaNumList, item, db);
                                    }
                                    ; break;
                            }
                            ; break;
                    }
                    money = money + timeReward;
                }
            }
            return money;
        }

        #endregion 目标价值奖惩

        #region 根据目标Id，取得目标公式列表，计算出结果

        /// <summary>
        /// 根据目标Id，取得目标公式列表，计算出结果
        /// </summary>
        /// <param name="formulaNumList"></param>
        /// <param name="item"></param>
        /// <param name="db"></param>
        /// <param name="values"></param>
        /// <param name="timeReward"></param>
        private decimal GetFromualString(List<int> formulaNumList, ObjectiveModel item, TargetNavigationDBEntities db)
        {
            var money = 0M;
            //循环目标公式编号
            foreach (var formulaNum in formulaNumList)
            {
                //根据目标Id查询目标公式列表
                var objectiveFormula = (from objective in db.tblObjectiveFormula
                                        where objective.objectiveId == item.objectiveId && objective.formulaNum == formulaNum
                                        orderby objective.formulaNum, objective.orderNum ascending
                                        select objective).ToList();
                //公式字符串
                var formula = new StringBuilder();
                var field = 0;
                var numValue = 0M;
                var count = 0;
                //循环目标公式列表
                foreach (var objectItem in objectiveFormula)
                {
                    count++;
                    if (objectItem.field == 1)
                    {//实际值
                        formula.Append(item.actualValue);
                    }
                    else if (objectItem.field == 2)
                    {
                        //指标值
                        formula.Append(item.objectiveValue);
                    }
                    else if (objectItem.field == 3)
                    {//理想值
                        formula.Append(item.expectedValue);
                    }
                    else if (objectItem.field == 4)
                    {//开始时间
                        formula.Append(item.actualStartTime);
                    }
                    else if (objectItem.field == 5)
                    {//结束时间
                        formula.Append(item.actualEndTime);
                    }
                    else if (objectItem.field == 6)
                    {//警戒时间
                        formula.Append(item.alarmTime);
                    }
                    else if (objectItem.field == 7)
                    {//权重
                        formula.Append(item.weight);
                    }
                    else if (objectItem.field == 8)
                    {
                        field = 8;
                    }
                    else if (objectItem.field == 9)
                    {
                        field = 9;
                    }

                    if (objectItem.operate != null)
                    {
                        //判断操作符是否含有&、|字符
                        if (objectItem.operate.Contains("&"))
                        {
                            formula.Append("&&");
                        }
                        else if (objectItem.operate.Contains("|"))
                        {
                            formula.Append("||");
                        }
                        else if (objectItem.operate != null)
                        {
                            formula.Append(objectItem.operate);
                        }
                    }

                    if (objectItem.numValue != null)
                    {
                        if (count == objectiveFormula.Count)
                        {
                            numValue = decimal.Parse(objectItem.numValue);
                        }
                        else
                        {
                            if (objectItem.numValue != null)
                            {
                                formula.Append(objectItem.numValue);
                            }
                        }
                    }
                }
                if (bool.Parse(StringUtils.StringCalculate(formula.ToString()).ToString()) == true)
                {
                    if (field == 8)
                    {//奖励基数
                        money = item.bonus.Value * numValue;
                    }
                    else if (field == 9)
                    {//数字
                        money = numValue;
                    }
                }
            }
            return money;
        }

        #endregion 根据目标Id，取得目标公式列表，计算出结果

        #region 获取计划执行力

        /// <summary>
        ///  获取计划执行力
        /// </summary>
        /// <param name="year">年</param>
        private List<PlanModel> GetPlanCompleteStatistics(int userId, string year, string month = null)
        {
            var list = new List<PlanModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {//年的场合
                    list = (from plan in db.tblYearPlanCompleteStatistics
                            where plan.userId == userId && plan.statisticalTime.IndexOf(year) != -1
                            select new PlanModel
                            {
                                strname = plan.statisticalTime.Substring(4, 2),
                                userId = plan.userId,
                                organizationId = plan.organizationId,
                                planCount = plan.planCount,
                                completeCount = plan.completeCount,
                                //超时数
                                timeOutCount = plan.timeOut
                            }).ToList();
                }
                else
                {//月的场合
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    list = (from plan in db.tblMonthPlanCompleteStatistics
                            where plan.userId == userId && plan.statisticalTime.IndexOf(year + month) != -1
                            select new PlanModel
                            {
                                strname = plan.statisticalTime.Substring(6, 2),
                                userId = plan.userId,
                                organizationId = plan.organizationId,
                                planCount = plan.planCount,
                                completeCount = plan.completeCount,
                                //超时数
                                timeOutCount = plan.timeOut
                            }).ToList();
                }
            }
            return list;
        }

        #endregion 获取计划执行力

        #region 获取流程执行力

        /// <summary>
        /// 获取流程执行力
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year">年的场合</param>
        /// <returns></returns>
        private List<FormModel> GetFormDuplicate(int userId, int year, int? month)
        {
            var list = new List<FormModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {//年的场合
                    list = (from fd in db.tblFormDuplicate
                            join ff in db.tblFormFlow on fd.formId equals ff.formId
                            where fd.userId == userId && fd.createTime.Year == year
                            select new FormModel
                            {
                                userId = userId,
                                FDcreateTime = fd.createTime,
                                FFcreateTime = ff.createTime
                            }).ToList();
                    list = list.GroupBy(c => c.FDcreateTime).Select(c => new FormModel
                     {
                         FFcreateTime = c.Max(x => x.FFcreateTime),
                         FDcreateTime = c.Key,
                         userId = userId
                     }).OrderBy(x => x.FDcreateTime).ToList();
                }
                else
                {//月的场合
                    list = (from fd in db.tblFormDuplicate
                            join ff in db.tblFormFlow on fd.formId equals ff.formId
                            where fd.userId == userId && fd.createTime.Year == year && fd.createTime.Month == month
                            orderby fd.createTime
                            select new FormModel
                            {
                                userId = userId,
                                FDcreateTime = fd.createTime,
                                FFcreateTime = ff.createTime
                            }).ToList();
                    list = list.GroupBy(c => c.FDcreateTime).Select(c => new FormModel
                    {
                        FFcreateTime = c.Max(x => x.FFcreateTime),
                        FDcreateTime = c.Key,
                        userId = userId
                    }).OrderBy(x => x.FDcreateTime).ToList();
                }
            }
            return list;
        }

        #endregion 获取流程执行力

        #region 获取功效价值

        /// <summary>
        /// 获取功效价值
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<PerWorkTimeModel> GetPerMonthWorkTime(int userId, string year, string month = null)
        {
            var model = new List<PerWorkTimeModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {//年的场合
                    model = (from monthTime in db.tblPerMonthWorkTime
                             where monthTime.userId == userId && monthTime.statisticalTime.IndexOf(year) != -1
                             select new PerWorkTimeModel
                             {
                                 name = monthTime.statisticalTime.Substring(4, 2),
                                 effectiveTimeSum = monthTime.effectiveTime,
                                 workTimeSum = monthTime.workTime
                             }).ToList();
                }
                else
                {//月的场合
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    model = (from dayTime in db.tblPerDayWorkTime
                             where dayTime.userId == userId && dayTime.statisticalTime.IndexOf(year + month) != -1
                             select new PerWorkTimeModel
                             {
                                 name = dayTime.statisticalTime.Substring(6, 2),
                                 effectiveTimeSum = dayTime.effectiveTime,
                                 workTimeSum = dayTime.workTime
                             }).ToList();
                }
            }
            return model;
        }

        #endregion 获取功效价值

        #region 获取功效价值 总数

        /// <summary>
        /// 获取功效价值 总数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private int GetPerWorkTimeCount(int userId, string year, string month = null)
        {
            var count = 0;
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {//年的场合
                    count = (from monthTime in db.tblPerMonthWorkTime
                             where monthTime.userId == userId && monthTime.statisticalTime.IndexOf(year) != -1
                             select monthTime
                           ).Count();
                }
                else
                {//月的场合
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    count = (from dayTime in db.tblPerDayWorkTime
                             where dayTime.userId == userId && dayTime.statisticalTime.IndexOf(year + month) != -1
                             select dayTime).Count();
                }
            }
            return count;
        }

        #endregion 获取功效价值 总数

        #region 获取目标价值

        /// <summary>
        /// 获取目标价值
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private List<PlanModel> GetTargetCompleteStatistics(int userId, string year, string month = null)
        {
            var list = new List<PlanModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                if (month == null)
                {
                    list = (from mTarget in db.tblYearTargetCompleteStatistics
                            where mTarget.userId == userId && mTarget.statisticalTime.IndexOf(year) != -1
                            select new PlanModel
                            {
                                strname = mTarget.statisticalTime.Substring(4, 2),
                                userId = mTarget.userId,
                                organizationId = mTarget.organizationId,
                                planCount = mTarget.objectiveCount,
                                completeCount = mTarget.completeCount,
                                timeOutCount = mTarget.timeOut
                            }).ToList();
                }
                else
                {
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    list = (from dTarget in db.tblMonthTargetCompleteStatistics
                            where dTarget.userId == userId && dTarget.statisticalTime.IndexOf(year + month) != -1
                            select new PlanModel
                            {
                                strname = dTarget.statisticalTime.Substring(6, 2),
                                userId = dTarget.userId,
                                organizationId = dTarget.organizationId,
                                planCount = dTarget.objectiveCount,
                                completeCount = dTarget.completeCount,
                                timeOutCount = dTarget.timeOut
                            }).ToList();
                }
            }
            return list;
        }

        #endregion 获取目标价值

        #region 年度奖惩、月度奖惩金额

        /// <summary>
        /// 年度奖惩、月度奖惩金额
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private decimal RewardMoney(int userId, int? year, int? month)
        {
            //每月有效工作时奖惩
            var value1 = GetTimeReward(userId, year, month);
            //var dNum = 0M;
            //计划完成情况奖惩
            var list = GetRewardPunish(userId, year, month).Where(c => c.type == 1).Sum(c => -c.deductionNum);

            var value2 = list;
            //计划审核确认奖惩
            var value3 = 0M;
            foreach (var item in GetRewardPunish(userId, year, month))
            {
                if (item.type == 2 || item.type == 3)
                {
                    value3 = value3 - item.deductionNum.Value;
                }
            }

            //目标价值奖惩
            var value4 = GetObjective(userId, year, month);

            return value1 + value2.Value + value3 + value4;
        }
        /// <summary>
        /// 获取周 月激励
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <param name="month">0为周</param>
        /// <returns></returns>
        public UserIndexIncentiveInfoModel RewardMoneyByMonthAndWeek(int userId, int? year, int? month)
        {
            UserIndexIncentiveInfoModel UserIncentiveInfo = new UserIndexIncentiveInfoModel();
            //每月有效工作时奖惩
            var value1 = GetTimeReward(userId, year, month);
            if (value1 >= 0)
            {
                UserIncentiveInfo.rewardAmount += value1;
            }
            else
            {
                UserIncentiveInfo.punishmentAmount += value1;
            }
            //var dNum = 0M;
            //计划完成情况奖惩
            var list = GetRewardPunish(userId, year, month).Where(c => c.type == 1).Sum(c => -c.deductionNum);

            var value2 = list;
            if (value2 >= 0)
            {
                UserIncentiveInfo.rewardAmount += Convert.ToDecimal(value2);
            }
            else
            {
                UserIncentiveInfo.punishmentAmount += Convert.ToDecimal(value2);
            }
            //计划审核确认奖惩
            var value3 = 0M;
            foreach (var item in GetRewardPunish(userId, year, month))
            {
                if (item.type == 2 || item.type == 3)
                {
                    value3 = value3 - item.deductionNum.Value;
                }
            }
            if (value3 >= 0)
            {
                UserIncentiveInfo.rewardAmount += value3;
            }
            else
            {
                UserIncentiveInfo.punishmentAmount += value3;
            }
            //目标价值奖惩
            var value4 = GetObjective(userId, year, month);
            if (value4 >= 0)
            {
                UserIncentiveInfo.rewardAmount += value4;
            }
            else
            {
                UserIncentiveInfo.punishmentAmount += value4;
            }

            return UserIncentiveInfo;
        }

        #endregion 年度奖惩、月度奖惩金额

        #region 根据用户Id查找下属

        /// <summary>
        /// 根据用户Id查找下属
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        private List<UnderReward> GetList(int userId)
        {
            var list = new List<UnderReward>();
            using (var db = new TargetNavigationDBEntities())
            {
                int orgId = (from user in db.tblUser
                             join us in db.tblUserStation on user.userId equals us.userId
                             join station in db.tblStation on us.stationId equals station.stationId
                             where user.userId == userId
                             select station.stationId).FirstOrDefault();
                list = (from user in db.tblUser
                        join us in db.tblUserStation on user.userId equals us.userId
                        join station in db.tblStation on us.stationId equals station.stationId
                        where station.parentStation == orgId
                        select new UnderReward
                        {
                            userId = user.userId,
                            userName = user.userName,
                            isParent = false
                        }).ToList();
            }
            return list;
        }

        #endregion 根据用户Id查找下属
        /// <summary>
        /// 计算某日起始日期（礼拜一的日期）
        /// </summary>
        /// <param name="someDate">该周中任意一天</param>
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns>
        private DateTime GetMondayDate(DateTime someDate)
        {
            var dayDiff = (someDate.DayOfWeek == DayOfWeek.Sunday) ? (someDate.DayOfWeek + 7 - 1) : (someDate.DayOfWeek - 1);

            return someDate.Subtract(new TimeSpan((int)dayDiff, 0, 0, 0));
        }
        #endregion 私有方法
    }
}