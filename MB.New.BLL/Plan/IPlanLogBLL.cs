using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL.Plan
{
    public interface IPlanLogBLL
    {
        #region 一般计划

        /// <summary>
        /// 根据计划ID取得操作日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<PlanLogModel> GetPlanLogByPlanId(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 插入计划操作日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planLogModel"></param>
        void InsPlanLog(TargetNavigationDBEntities db, PlanLogModel planLogModel);

        #endregion 一般计划

        #region 循环计划

        /// <summary>
        /// 根据循环计划ID取得操作日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="loopId"></param>
        /// <returns></returns>
        List<PlanLogModel> GetLoopLogByLoopId(TargetNavigationDBEntities db, int loopId);

        /// <summary>
        /// 插入循环计划操作日志
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planLogModel"></param>
        void InsLoopPlanLog(TargetNavigationDBEntities db, PlanLogModel planLogModel);

        #endregion 循环计划

        /// <summary>
        /// 撤销计划操作添加日志
        /// </summary>
        /// <param name="stop">中止状态</param>
        /// <param name="status">计划状态</param>
        EnumDefine.PlanOperateStatus ReturnPlanOperateStatus(EnumDefine.PlanStopStatus stop, EnumDefine.PlanStatus status);

        /// <summary>
        /// 撤销循环计划操作添加日志
        /// </summary>
        /// <param name="stop">中止状态</param>
        /// <param name="status">计划状态</param>
        EnumDefine.PlanOperateStatus ReturnLoopPlanOperateStatus(EnumDefine.LoopPlanStopStatus stop, EnumDefine.LoopPlanStatus status);
    }
}