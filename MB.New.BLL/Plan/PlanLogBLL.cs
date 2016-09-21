using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Plan
{
    public class PlanLogBLL : IPlanLogBLL
    {
        /// <summary>
        /// 根据循环计划ID取得计划日志信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<PlanLogModel> GetLoopLogByLoopId(TargetNavigationDBEntities db, int loopId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 设置循环计划ID
            var condition = "loopId == @0";
            var values = new object[] { loopId };

            var planLogs = db.tblLoopplanOperate.Where(condition, values).OrderByDescending(p => p.operateId).Select(p => new PlanLogModel
            {
                planId = p.loopId,
                message = p.message,
                type = (EnumDefine.PlanOperateStatus)p.result.Value,
                operateUser = p.reviewUser.Value,
                operateTime = p.reviewTime.Value
            }).ToList();

            return planLogs;
        }

        /// <summary>
        /// 根据计划ID取得计划日志信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<PlanLogModel> GetPlanLogByPlanId(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 设置计划ID
            var condition = "planId == @0";
            var values = new object[] { planId };

            var planLogs = db.tblPlanOperate.Where(condition, values)
                .OrderByDescending(p => p.operateId)
                .Select(p => new PlanLogModel
                {
                    planId = p.planId,
                    message = p.message,
                    type = (EnumDefine.PlanOperateStatus)p.result.Value,
                    operateUser = p.reviewUser.Value,
                    operateTime = p.reviewTime.Value
                }).ToList();

            return planLogs;
        }

        /// <summary>
        /// 保存循环计划日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planLogModel"></param>
        public void InsLoopPlanLog(TargetNavigationDBEntities db, PlanLogModel planLogModel)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得主键
            var id = DBUtility.GetPrimaryKeyByTableName(db, "tblLoopPlanOperate");

            var model = new tblLoopplanOperate
            {
                operateId = id,
                loopId = planLogModel.planId,
                message = planLogModel.message,
                result = (int)planLogModel.type,
                reviewUser = planLogModel.operateUser,
                reviewTime = planLogModel.operateTime
            };

            db.tblLoopplanOperate.Add(model);
        }

        /// <summary>
        /// 保存计划日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planLogModel"></param>
        public void InsPlanLog(TargetNavigationDBEntities db, PlanLogModel planLogModel)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            // 取得主键
            var id = DBUtility.GetPrimaryKeyByTableName(db, "tblPlanOperate");
            var model = new tblPlanOperate
            {
                operateId = id,
                planId = planLogModel.planId,
                message = planLogModel.message,
                result = (int)planLogModel.type,
                reviewUser = planLogModel.operateUser,
                reviewTime = planLogModel.operateTime
            };

            db.tblPlanOperate.Add(model);
        }

        /// <summary>
        /// 撤销操作添加日志
        /// </summary>
        /// <param name="stop">中止状态</param>
        /// <param name="status">计划状态</param>
        public EnumDefine.PlanOperateStatus ReturnPlanOperateStatus(EnumDefine.PlanStopStatus stop, EnumDefine.PlanStatus status)
        {
            var result = EnumDefine.PlanOperateStatus.CancelCheck;
            //判断计划中止状态
            switch (stop)
            {
                //申请中止的场合
                case EnumDefine.PlanStopStatus.Checking:
                    result = EnumDefine.PlanOperateStatus.CancelCheck;
                    break;
                //其他场合
                case (int)EnumDefine.PlanStopStatus.Running:
                    switch (status)
                    {
                        //待审核
                        case EnumDefine.PlanStatus.Checking:
                            result = EnumDefine.PlanOperateStatus.CancelCheck;
                            break;
                        //审核通过
                        case EnumDefine.PlanStatus.CheckPassed:
                            result = EnumDefine.PlanOperateStatus.CancelSubmit;
                            break;
                        //申请修改
                        case EnumDefine.PlanStatus.RequestEdit:
                            result = EnumDefine.PlanOperateStatus.CancelCheck;
                            break;
                        //待确认
                        case EnumDefine.PlanStatus.Confirming:
                            result = EnumDefine.PlanOperateStatus.CancelCheck;
                            break;

                        default:
                            break;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// 撤销循环计划操作添加日志
        /// </summary>
        /// <param name="stop">中止状态</param>
        /// <param name="status">计划状态</param>
        public EnumDefine.PlanOperateStatus ReturnLoopPlanOperateStatus(EnumDefine.LoopPlanStopStatus stop, EnumDefine.LoopPlanStatus status)
        {
            var result = EnumDefine.PlanOperateStatus.CancelCheck;
            //判断计划中止状态
            switch (stop)
            {
                //申请中止的场合
                case EnumDefine.LoopPlanStopStatus.Checking:
                    result = EnumDefine.PlanOperateStatus.CancelCheck;
                    break;
                //其他场合
                case EnumDefine.LoopPlanStopStatus.Running:
                    switch (status)
                    {
                        //待审核
                        case EnumDefine.LoopPlanStatus.Checking:
                            result = EnumDefine.PlanOperateStatus.CancelCheck;
                            break;
                        //审核通过
                        case EnumDefine.LoopPlanStatus.CheckPassed:
                            result = EnumDefine.PlanOperateStatus.CancelSubmit;
                            break;
                        //申请修改
                        case EnumDefine.LoopPlanStatus.RequestEdit:
                            result = EnumDefine.PlanOperateStatus.CancelCheck;
                            break;

                        default:
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}