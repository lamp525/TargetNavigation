using MB.DAL;
using MB.New.Model;
using System;

namespace MB.New.BLL.Incentive
{
    public class IncentiveBLL : IIncentiveBLL
    {
        public UserIndexIncentiveInfoModel GetUserIncentiveByMonth(TargetNavigationDBEntities db, int userId, string yearMonth)
        {
            throw new NotImplementedException();
        }

        public UserIndexIncentiveInfoModel GetUserIncentiveByWeek(TargetNavigationDBEntities db, int userId, int weekOfYear)
        {
            throw new NotImplementedException();
        }

        //    /// <summary>
        //    /// 获取周 月激励
        //    /// </summary>
        //    /// <param name="userId"></param>
        //    /// <param name="year"></param>
        //    /// <param name="month">0为周</param>
        //    /// <returns></returns>
        //    public UserIndexIncentiveInfoModel RewardMoneyByMonthAndWeek(int userId, int? year, int? month)
        //    {
        //        UserIndexIncentiveInfoModel UserIncentiveInfo = new UserIndexIncentiveInfoModel();
        //        //每月有效工作时奖惩
        //        var value1 = GetTimeReward(userId, year, month);
        //        if (value1 >= 0)
        //        {
        //            UserIncentiveInfo.rewardAmount += value1;
        //        }
        //        else
        //        {
        //            UserIncentiveInfo.punishmentAmount += value1;
        //        }
        //        //var dNum = 0M;
        //        //计划完成情况奖惩
        //        var list = GetRewardPunish(userId, year, month).Where(c => c.type == 1).Sum(c => -c.deductionNum);

        //        var value2 = list;
        //        if (value2 >= 0)
        //        {
        //            UserIncentiveInfo.rewardAmount += Convert.ToDecimal(value2);
        //        }
        //        else
        //        {
        //            UserIncentiveInfo.punishmentAmount += Convert.ToDecimal(value2);
        //        }
        //        //计划审核确认奖惩
        //        var value3 = 0M;
        //        foreach (var item in GetRewardPunish(userId, year, month))
        //        {
        //            if (item.type == 2 || item.type == 3)
        //            {
        //                value3 = value3 - item.deductionNum.Value;
        //            }
        //        }
        //        if (value3 >= 0)
        //        {
        //            UserIncentiveInfo.rewardAmount += value3;
        //        }
        //        else
        //        {
        //            UserIncentiveInfo.punishmentAmount += value3;
        //        }
        //        //目标价值奖惩
        //        var value4 = GetObjective(userId, year, month);
        //        if (value4 >= 0)
        //        {
        //            UserIncentiveInfo.rewardAmount += value4;
        //        }
        //        else
        //        {
        //            UserIncentiveInfo.punishmentAmount += value4;
        //        }

        //        return UserIncentiveInfo;
        //    }

        //    /// <summary>
        //    ///有效工时奖惩规则 获取奖惩金额
        //    /// </summary>
        //    /// <returns></returns>
        //    private decimal GetTimeReward(int userId, int? year, int? month)
        //    {
        //        var timeReward = 0M;
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            //获取价值激励列表
        //            var IncentiveModel = (from v in db.tblValueIncentive
        //                                  select new ValueIncentiveInfoModel
        //                                  {
        //                                      standardTime = v.standardTime,
        //                                      incentiveType = v.incentiveType,
        //                                      maxValueType = v.maxValueType,
        //                                      maxValue = v.maxValue,
        //                                      maxAverage = v.maxAverage,
        //                                      minValueType = v.minValueType,
        //                                      minValue = v.minValue,
        //                                      minAverage = v.minAverage
        //                                  }).FirstOrDefault();

        //            if (month == null)
        //            {
        //                timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), null);
        //            }
        //            else if (month != null && month != 0)
        //            {
        //                timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), month.ToString());
        //            }
        //            else
        //            {
        //                timeReward = this.GetTimeout(IncentiveModel, userId, year.ToString(), month.ToString());
        //            }
        //            if (timeReward != 0)
        //            {
        //                return timeReward;
        //            }
        //        }

        //        return timeReward;
        //    }

        //    /// <summary>
        //    /// 获取超时天数，并算出奖励金额
        //    /// </summary>
        //    /// <param name="model"></param>
        //    /// <param name="timeOut"></param>
        //    /// <param name="userId"></param>
        //    /// <param name="year"></param>
        //    /// <param name="month"></param>
        //    /// <param name="day"></param>
        //    /// <returns></returns>
        //    private decimal GetTimeout(ValueIncentiveInfoModel model, int userId, string year = null, string month = null)
        //    {
        //        var timeOut = 0M;
        //        var timeReward = 0M;
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            //工资
        //            var salary = db.tblUser.Where(c => c.userId == userId).FirstOrDefault().salary;
        //            if (model != null)
        //            {
        //                switch (model.incentiveType)
        //                {
        //                    //无的场合
        //                    case 0: timeReward = 0; break;
        //                    //按比例的场合
        //                    case 1: foreach (var y in this.GetUserEffectiveTime(userId, year, month))
        //                        {
        //                            timeOut = y / 60 - model.standardTime;

        //                            var reward = 0M;
        //                            if (timeOut > 0)
        //                            {//超出
        //                                if (model.maxValueType == 1)
        //                                {
        //                                    //金额类型
        //                                    //奖励金额
        //                                    reward = timeOut * model.maxAverage.Value;
        //                                }
        //                                else
        //                                {
        //                                    if (salary != null)
        //                                    {
        //                                        //百分比
        //                                        //奖励金额
        //                                        reward = timeOut * salary.Value * model.maxAverage.Value;
        //                                    }
        //                                }
        //                                if (reward > model.maxValue)
        //                                {
        //                                    reward = model.maxValue.Value;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                //不足
        //                                if (model.minValueType == 1)
        //                                {
        //                                    //金额类型
        //                                    //奖励金额
        //                                    reward = timeOut * model.minAverage.Value;
        //                                }
        //                                else
        //                                {
        //                                    if (salary != null)
        //                                    {
        //                                        //百分比
        //                                        //奖励金额
        //                                        reward = timeOut * salary.Value * model.minAverage.Value;
        //                                    }
        //                                }
        //                                if (reward > model.minValue)
        //                                {
        //                                    reward = model.minValue.Value;
        //                                }
        //                            }
        //                            timeReward = timeReward + reward;
        //                        }
        //                        ; break;
        //                    //自定义场合
        //                    case 2: foreach (var y in this.GetUserEffectiveTime(userId, year, month))
        //                        {
        //                            timeOut = y / 60 - model.standardTime;

        //                            var reward = 0M;
        //                            var ValueIncentiveCustomModel = this.GetValueIncentiveCustom(timeOut, salary);
        //                            if (timeOut > 0)
        //                            {
        //                                //超出
        //                                if (ValueIncentiveCustomModel != null)
        //                                {
        //                                    //金额
        //                                    reward = ValueIncentiveCustomModel.deductionNum.Value;
        //                                }
        //                            }
        //                            else if (timeOut < 0)
        //                            {
        //                                //不足

        //                                if (ValueIncentiveCustomModel != null)
        //                                {
        //                                    //金额
        //                                    reward = ValueIncentiveCustomModel.deductionNum.Value;
        //                                }
        //                            }
        //                            timeReward = timeReward + reward;
        //                        }; break;
        //                }
        //            }
        //        }
        //        return timeReward;
        //    }

        //    /// <summary>
        //    /// 用户有效工时
        //    /// </summary>
        //    /// <param name="year">年的场合</param>
        //    /// <param name="month">月的场合</param>
        //    /// <returns></returns>
        //    private List<decimal> GetUserEffectiveTime(int userId, string year, string month = null)
        //    {
        //        var list = new List<decimal>();
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            if (month == null)
        //            {
        //                list = (from Time in db.tblPerDayWorkTime
        //                        where Time.userId == userId && Time.statisticalTime.IndexOf(year) != -1
        //                        select Time.effectiveTime).ToList();
        //            }
        //            else if (month != null && !month.Equals("0"))
        //            {
        //                if (month.Length == 1)
        //                {
        //                    month = "0" + month;
        //                }
        //                list = (from Time in db.tblPerDayWorkTime
        //                        where Time.userId == userId && Time.statisticalTime.IndexOf(year + month) != -1
        //                        select Time.effectiveTime).ToList();
        //            }
        //            else
        //            {
        //                TimeSpan span = DateTime.Now - DateUtility.GetWeekFirstDay(DateTime.Now);
        //                for (int i = 0; i <= System.Math.Abs(span.Days); i++)
        //                {
        //                    var thisTime = DateUtility.GetWeekFirstDay(DateTime.Now).AddDays(i).ToString("yyyyMMdd");
        //                    var model = db.tblPerDayWorkTime.Where(p => p.userId == userId && p.statisticalTime == thisTime).FirstOrDefault();
        //                    if (model != null)
        //                    {
        //                        list.Add(model.effectiveTime);
        //                    }
        //                }
        //            }
        //        }
        //        return list;
        //    }

        //    /// <summary>
        //    /// 执行力奖惩
        //    /// </summary>
        //    /// <param name="userId">用户Id</param>
        //    /// <param name="year">年的场合</param>
        //    /// <param name="month">月的场合</param>
        //    /// <param name="day">日的场合</param>
        //    private List<RewardPunishModel> GetRewardPunish(int userId, int? year, int? month)
        //    {
        //        var list = new List<RewardPunishModel>();
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            //年的场合
        //            if (month == null)
        //            {
        //                list = (from r in db.tblRewardPunish
        //                        where r.userId == userId && r.delayTime.Value.Year == year
        //                        select new RewardPunishModel
        //                        {
        //                            rewardId = r.rewardId,
        //                            userId = r.userId.Value,
        //                            type = r.type,
        //                            targetId = r.targetId.Value,
        //                            deductionMode = r.deductionMode,
        //                            deductionNum = r.deductionNum,
        //                            delayTime = r.delayTime
        //                        }).ToList();
        //            }
        //            else if (month != 0)
        //            {//月的场合
        //                list = (from r in db.tblRewardPunish
        //                        where r.userId == userId && r.delayTime.Value.Year == year && r.delayTime.Value.Month == month
        //                        select new RewardPunishModel
        //                        {
        //                            rewardId = r.rewardId,
        //                            userId = r.userId.Value,
        //                            type = r.type,
        //                            targetId = r.targetId.Value,
        //                            deductionMode = r.deductionMode,
        //                            deductionNum = r.deductionNum,
        //                            delayTime = r.delayTime
        //                        }).ToList();
        //            }
        //            else
        //            {
        //                var thisTime = DateUtility.GetWeekFirstDay(DateTime.Now);
        //                var weekTime = DateUtility.GetWeekFirstDay(DateTime.Now).AddDays(7);

        //                //周场合
        //                list = (from r in db.tblRewardPunish
        //                        where r.userId == userId && r.delayTime >= thisTime && r.delayTime <= weekTime
        //                        select new RewardPunishModel
        //                        {
        //                            rewardId = r.rewardId,
        //                            userId = r.userId.Value,
        //                            type = r.type,
        //                            targetId = r.targetId.Value,
        //                            deductionMode = r.deductionMode,
        //                            deductionNum = r.deductionNum,
        //                            delayTime = r.delayTime
        //                        }).ToList();
        //            }
        //        }
        //        return list;
        //    }

        //    /// <summary>
        //    /// 目标价值奖惩
        //    /// </summary>
        //    /// <param name="userId"></param>
        //    /// <param name="year"></param>
        //    /// <param name="month"></param>
        //    /// <param name="day"></param>
        //    private decimal GetObjective(int userId, int? year, int? month)
        //    {
        //        var money = 0M;
        //        var timeReward = 0M;
        //        var list = new List<ObjectiveModel>();
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            if (month == null)
        //            {
        //                list = (from o in db.tblObjective
        //                        where o.responsibleUser == userId && o.actualEndTime.Value.Year == year
        //                        select new ObjectiveModel
        //                        {
        //                            objectiveId = o.objectiveId,
        //                            objectiveValue = o.objectiveValue,
        //                            expectedValue = o.expectedValue,
        //                            actualValue = o.actualValue,
        //                            formula = o.formula,
        //                            bonus = o.bonus,
        //                            checkType = o.checkType,
        //                            actualStartTime = o.actualStartTime,
        //                            actualEndTime = o.actualEndTime,
        //                            alarmTime = o.alarmTime,
        //                            weight = o.weight,
        //                            endTime = o.endTime
        //                        }).ToList();
        //            }
        //            else if (month != null && month != 0)
        //            {
        //                list = (from o in db.tblObjective
        //                        where o.responsibleUser == userId && o.actualEndTime.Value.Year == year && o.actualEndTime.Value.Month == month
        //                        select new ObjectiveModel
        //                        {
        //                            objectiveId = o.objectiveId,
        //                            objectiveValue = o.objectiveValue,
        //                            expectedValue = o.expectedValue,
        //                            actualValue = o.actualValue,
        //                            formula = o.formula,
        //                            bonus = o.bonus,
        //                            checkType = o.checkType,
        //                            actualStartTime = o.actualStartTime,
        //                            actualEndTime = o.actualEndTime,
        //                            alarmTime = o.alarmTime,
        //                            weight = o.weight,
        //                            endTime = o.endTime
        //                        }).ToList();
        //            }
        //            else
        //            {
        //                var Monday = DateUtility.GetWeekFirstDay(DateTime.Now);
        //                var weekday = DateUtility.GetWeekFirstDay(DateTime.Now).AddDays(7);
        //                list = (from o in db.tblObjective
        //                        where o.responsibleUser == userId && o.actualEndTime >= Monday && o.actualEndTime <= weekday
        //                        select new ObjectiveModel
        //                        {
        //                            objectiveId = o.objectiveId,
        //                            objectiveValue = o.objectiveValue,
        //                            expectedValue = o.expectedValue,
        //                            actualValue = o.actualValue,
        //                            formula = o.formula,
        //                            bonus = o.bonus,
        //                            checkType = o.checkType,
        //                            actualStartTime = o.actualStartTime,
        //                            actualEndTime = o.actualEndTime,
        //                            alarmTime = o.alarmTime,
        //                            weight = o.weight,
        //                            endTime = o.endTime
        //                        }).ToList();
        //            }

        //            foreach (var item in list)
        //            {
        //                var timeOutDay = (item.actualEndTime.Value - item.endTime.Value).Days;
        //                //目标公式编号
        //                var formulaNumList = db.tblObjectiveFormula.Where(c => c.objectiveId == item.objectiveId).OrderBy(c => c.formulaNum).ThenBy(c => c.orderNum).Select(c => c.formulaNum).Distinct().ToList();
        //                switch (item.formula)
        //                {
        //                    //无的场合
        //                    case 0: timeReward = 0; break;
        //                    //默认公式的场合
        //                    case 1:
        //                        switch (item.checkType)
        //                        {
        //                            //金额
        //                            case 1:
        //                                if (item.bonus != null && item.actualValue != null)
        //                                {
        //                                    if (item.expectedValue != item.objectiveValue)
        //                                    {
        //                                        timeReward = item.bonus.Value * ((int.Parse(item.actualValue) - int.Parse(item.objectiveValue)) / (int.Parse(item.expectedValue) - int.Parse(item.objectiveValue)) + 1);
        //                                    }
        //                                }
        //                                ; break;
        //                            //时间
        //                            case 2:
        //                                if (item.bonus != null && item.actualValue != null)
        //                                {
        //                                    if (item.expectedValue != item.objectiveValue)
        //                                    {
        //                                        timeReward = item.bonus.Value * ((DateTime.Parse(item.actualValue) - DateTime.Parse(item.objectiveValue)).Days / (DateTime.Parse(item.expectedValue) - DateTime.Parse(item.objectiveValue)).Days + 1);
        //                                    }
        //                                }
        //                                ; break;
        //                            //数量
        //                            case 3:
        //                                if (item.bonus != null && item.actualValue != null)
        //                                {
        //                                    if (item.expectedValue != item.objectiveValue)
        //                                    {
        //                                        timeReward = item.bonus.Value * ((int.Parse(item.actualValue) - int.Parse(item.objectiveValue)) / (int.Parse(item.expectedValue) - int.Parse(item.objectiveValue)) + 1);
        //                                    }
        //                                }
        //                                ; break;
        //                        }
        //                        if (timeOutDay > 0)
        //                        {//惩罚
        //                            if (timeReward > item.minValue * item.bonus)
        //                            {
        //                                timeReward = (decimal)(item.minValue * item.bonus);
        //                            }
        //                        }
        //                        else
        //                        { //奖励
        //                            if (timeReward > item.maxValue * item.bonus)
        //                            {
        //                                timeReward = (decimal)(item.maxValue * item.bonus);
        //                            }
        //                        }
        //                        ; break;
        //                    //自定义的场合
        //                    case 2:
        //                        switch (item.checkType)
        //                        {
        //                            //金额
        //                            case 1:
        //                                if (item.actualValue != null)
        //                                {
        //                                    timeReward = this.GetFromualString(formulaNumList, item, db);
        //                                }
        //                                ; break;
        //                            //时间
        //                            case 2:
        //                                if (item.actualValue != null)
        //                                {
        //                                    var actualValue = item.actualValue.Replace("-", "").Replace("/", "");
        //                                    var objectiveValue = item.objectiveValue.Replace("-", "").Replace("/", "");
        //                                    var expectedValue = item.expectedValue.Replace("-", "").Replace("/", "");
        //                                    timeReward = this.GetFromualString(formulaNumList, item, db);
        //                                }
        //                                ; break;
        //                            //数字
        //                            case 3:
        //                                if (item.actualValue != null)
        //                                {
        //                                    timeReward = this.GetFromualString(formulaNumList, item, db);
        //                                }
        //                                ; break;
        //                        }
        //                        ; break;
        //                }
        //                money = money + timeReward;
        //            }
        //        }
        //        return money;
        //    }

        //    /// <summary>
        //    /// 根据目标Id，取得目标公式列表，计算出结果
        //    /// </summary>
        //    /// <param name="formulaNumList"></param>
        //    /// <param name="item"></param>
        //    /// <param name="db"></param>
        //    /// <param name="values"></param>
        //    /// <param name="timeReward"></param>
        //    private decimal GetFromualString(List<int> formulaNumList, ObjectiveModel item, TargetNavigationDBEntities db)
        //    {
        //        var money = 0M;
        //        //循环目标公式编号
        //        foreach (var formulaNum in formulaNumList)
        //        {
        //            //根据目标Id查询目标公式列表
        //            var objectiveFormula = (from objective in db.tblObjectiveFormula
        //                                    where objective.objectiveId == item.objectiveId && objective.formulaNum == formulaNum
        //                                    orderby objective.formulaNum, objective.orderNum ascending
        //                                    select objective).ToList();
        //            //公式字符串
        //            var formula = new StringBuilder();
        //            var field = 0;
        //            var numValue = 0M;
        //            var count = 0;
        //            //循环目标公式列表
        //            foreach (var objectItem in objectiveFormula)
        //            {
        //                count++;
        //                if (objectItem.field == 1)
        //                {//实际值
        //                    formula.Append(item.actualValue);
        //                }
        //                else if (objectItem.field == 2)
        //                {
        //                    //指标值
        //                    formula.Append(item.objectiveValue);
        //                }
        //                else if (objectItem.field == 3)
        //                {//理想值
        //                    formula.Append(item.expectedValue);
        //                }
        //                else if (objectItem.field == 4)
        //                {//开始时间
        //                    formula.Append(item.actualStartTime);
        //                }
        //                else if (objectItem.field == 5)
        //                {//结束时间
        //                    formula.Append(item.actualEndTime);
        //                }
        //                else if (objectItem.field == 6)
        //                {//警戒时间
        //                    formula.Append(item.alarmTime);
        //                }
        //                else if (objectItem.field == 7)
        //                {//权重
        //                    formula.Append(item.weight);
        //                }
        //                else if (objectItem.field == 8)
        //                {
        //                    field = 8;
        //                }
        //                else if (objectItem.field == 9)
        //                {
        //                    field = 9;
        //                }

        //                if (objectItem.operate != null)
        //                {
        //                    //判断操作符是否含有&、|字符
        //                    if (objectItem.operate.Contains("&"))
        //                    {
        //                        formula.Append("&&");
        //                    }
        //                    else if (objectItem.operate.Contains("|"))
        //                    {
        //                        formula.Append("||");
        //                    }
        //                    else if (objectItem.operate != null)
        //                    {
        //                        formula.Append(objectItem.operate);
        //                    }
        //                }

        //                if (objectItem.numValue != null)
        //                {
        //                    if (count == objectiveFormula.Count)
        //                    {
        //                        numValue = decimal.Parse(objectItem.numValue);
        //                    }
        //                    else
        //                    {
        //                        if (objectItem.numValue != null)
        //                        {
        //                            formula.Append(objectItem.numValue);
        //                        }
        //                    }
        //                }
        //            }
        //            if (bool.Parse(StringUtility.StringCalculate(formula.ToString()).ToString()) == true)
        //            {
        //                if (field == 8)
        //                {//奖励基数
        //                    money = item.bonus.Value * numValue;
        //                }
        //                else if (field == 9)
        //                {//数字
        //                    money = numValue;
        //                }
        //            }
        //        }
        //        return money;
        //    }

        //    /// <summary>
        //    /// 价值激励自定义
        //    /// </summary>
        //    /// <param name="timeOut"></param>
        //    private ValueIncentiveCustomModel GetValueIncentiveCustom(decimal timeOut, decimal? salary)
        //    {
        //        var IncentiveCustomList = new ValueIncentiveCustomModel();
        //        using (var db = new TargetNavigationDBEntities())
        //        {
        //            if (timeOut > 0)
        //            {
        //                IncentiveCustomList = (from c in db.tblValueIncentiveCustom
        //                                       where c.customType == 2 && c.customStartTime <= timeOut && c.customEndTime >= timeOut
        //                                       select new ValueIncentiveCustomModel
        //                                       {
        //                                           customId = c.customId,
        //                                           customType = c.customType,
        //                                           customStartTime = c.customStartTime.Value,
        //                                           customEndTime = c.customEndTime.Value,
        //                                           deductionMode = c.deductionMode,
        //                                           deductionNum = c.deductionNum
        //                                       }).FirstOrDefault();
        //                if (IncentiveCustomList != null)
        //                {
        //                    if (IncentiveCustomList.deductionMode == 2)
        //                    {
        //                        IncentiveCustomList.deductionNum = IncentiveCustomList.deductionNum / 100 * salary.Value;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                IncentiveCustomList = (from c in db.tblValueIncentiveCustom
        //                                       where c.customType == 1 && c.customStartTime <= Math.Abs(timeOut) && c.customEndTime >= Math.Abs(timeOut)
        //                                       select new ValueIncentiveCustomModel
        //                                       {
        //                                           customId = c.customId,
        //                                           customType = c.customType,
        //                                           customStartTime = c.customStartTime.Value,
        //                                           customEndTime = c.customEndTime.Value,
        //                                           deductionMode = c.deductionMode,
        //                                           deductionNum = c.deductionNum
        //                                       }).FirstOrDefault();
        //                if (IncentiveCustomList != null)
        //                {
        //                    if (IncentiveCustomList.deductionMode == 2)
        //                    {
        //                        IncentiveCustomList.deductionNum = IncentiveCustomList.deductionNum / 100 * salary.Value;
        //                    }
        //                }
        //            }
        //        }
        //        return IncentiveCustomList;
        //    }
    }
}