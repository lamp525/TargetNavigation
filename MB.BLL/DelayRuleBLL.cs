using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class DelayRuleBLL : IDelayRuleBLL
    {
        /// <summary>
        /// 根据状态获取列表（计划延时，有效工时）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<DelayRuleModel> GetDelayList(int type)
        {
            var List = new List<DelayRuleModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                List = (from news in db.tblDelayRule
                        where news.ruleType == type
                        select new DelayRuleModel
                        {
                            deductionMode = news.deductionMode,
                            deductionNum = news.deductionNum,
                            delayEndTime = news.delayEndTime,
                            delayStartTime = news.delayStartTime,
                            ruleId = news.ruleId,
                            ruletype = news.ruleType
                        }).ToList();
            }
            return List;
        }

        /// <summary>
        /// 获取自定义激励规则（自定义）
        /// </summary>
        /// <returns></returns>
        public List<ValueIncentiveCustomModel> GetValueCustList()
        {
            var list = new List<ValueIncentiveCustomModel>();

            using (var db = new TargetNavigationDBEntities())
            {
                list = (from news in db.tblValueIncentiveCustom
                        select new ValueIncentiveCustomModel
                        {
                            customId = news.customId,
                            customEndTime = news.customEndTime,
                            customStartTime = news.customStartTime,
                            customType = news.customType,
                            deductionMode = news.deductionMode,
                            deductionNum = news.deductionNum
                        }).ToList();
            }
            return list;
        }

        /// <summary>
        /// 获取激励规则对象
        /// </summary>
        /// <returns></returns>
        public ValueIncentiveModel GetValueIncentive()
        {
            var model = new ValueIncentiveModel();
            using (var db = new TargetNavigationDBEntities())
            {
                model = (from news in db.tblValueIncentive
                         select new ValueIncentiveModel
                         {
                             incentiveType = news.incentiveType,
                             maxAverage = news.maxAverage,
                             maxValue = news.maxValue,
                             maxValueType = news.maxValueType,
                             minAverage = news.minAverage,
                             minValue = news.minValue,
                             minValueType = news.minValueType,
                             standardTime = news.standardTime
                         }).FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 更新或添加激励规则
        /// </summary>
        /// <param name="model"></param>
        public void AddOrUpdateValueIncentive(ValueIncentiveModel model)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var hasModel = db.tblValueIncentive.FirstOrDefault();
                if (hasModel == null)
                {
                    var addModel = new tblValueIncentive
                    {
                        incentiveType = Convert.ToInt32(model.incentiveType),
                        maxAverage = model.maxAverage,
                        maxValue = model.maxValue,
                        maxValueType = model.maxValueType,
                        minAverage = model.minAverage,
                        minValue = model.minValue,
                        minValueType = model.minValueType,
                        standardTime = Convert.ToInt32(model.standardTime)
                    };
                    db.tblValueIncentive.Add(addModel);
                }
                else
                {
                    hasModel.incentiveType = Convert.ToInt32(model.incentiveType);
                    hasModel.maxAverage = model.maxAverage;
                    hasModel.maxValue = model.maxValue;
                    hasModel.maxValueType = model.maxValueType;
                    hasModel.minAverage = model.minAverage;
                    hasModel.minValue = model.minValue;
                    hasModel.minValueType = model.minValueType;
                    hasModel.standardTime = Convert.ToInt32(model.standardTime);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 新增自定义规则
        /// </summary>
        /// <param name="id"></param>
        public void AddValueCust(List<ValueIncentiveCustomModel> List)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in List)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    item.customId = db.prcGetPrimaryKey("tblValueIncentiveCustom", obj).FirstOrDefault().Value;
                    var ValueIncentiveCustom = new tblValueIncentiveCustom
                    {
                        customId = Convert.ToInt32(item.customId),
                        customEndTime = item.customEndTime,
                        customStartTime = item.customStartTime,
                        customType = Convert.ToInt32(item.customType),
                        deductionMode = Convert.ToInt32(item.deductionMode),
                        deductionNum = Convert.ToDecimal(item.deductionNum)
                    };
                    db.tblValueIncentiveCustom.Add(ValueIncentiveCustom);
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 删除自定义
        /// </summary>
        /// <param name="valueIncentList"></param>
        public void DeleteValueCust(int[] id)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in id)
                {
                    var olddata = db.tblValueIncentiveCustom.Where(p => p.customId == item).FirstOrDefault();
                    if (olddata != null)
                    {
                        db.tblValueIncentiveCustom.Remove(olddata);
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 添加激励规则(计划延期，审批超时)
        /// </summary>
        /// <param name="delayRuleList"></param>
        /// <returns></returns>
        public bool AddDelayRule(List<DelayRuleModel> delayRuleList)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in delayRuleList)
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    item.ruleId = db.prcGetPrimaryKey("tblDelayRule", obj).FirstOrDefault().Value;
                    var ValueIncentiveCustom = new tblDelayRule
                    {
                        ruleId = Convert.ToInt32(item.ruleId),
                        deductionMode = item.deductionMode,
                        deductionNum = item.deductionNum,
                        delayEndTime = item.delayEndTime,
                        ruleType = item.ruletype,
                        delayStartTime = item.delayStartTime
                    };
                    db.tblDelayRule.Add(ValueIncentiveCustom);
                }
                db.SaveChanges();
            }
            return flag;
        }

        /// <summary>
        /// 删除激励规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDelayRule(int[] id)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in id)
                {
                    var olddata = db.tblDelayRule.Where(p => p.ruleId == item).FirstOrDefault();
                    if (olddata != null)
                    {
                        db.tblDelayRule.Remove(olddata);
                    }
                }
                db.SaveChanges();
            }
            return flag;
        }
    }
}