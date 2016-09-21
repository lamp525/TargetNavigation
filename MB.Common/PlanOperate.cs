using System;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.Common
{
    public static class PlanOperate
    {
        #region

        /// <summary>
        /// 新增循环计划操作日志
        /// </summary>
        /// <param name="LoopPlanInfo">LoopPlanInfo</param>
        public static void AddLoopPlanOperate(LoopPlanInfo loopPlanInfo, object result/*状态,从Operate枚举中选取*/, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblLoopplanOperate", obj).FirstOrDefault().Value;
            var loopplan = new tblLoopplanOperate
            {
                loopId = Convert.ToInt32(loopPlanInfo.loopId),
                operateId = operId,
                message = loopPlanInfo.eventOutput,
                result = Convert.ToInt32(result),
                reviewTime = DateTime.Now,
                reviewUser = loopPlanInfo.createUser
            };
            db.tblLoopplanOperate.Add(loopplan);
        }

        /// <summary>
        /// 新增计划操作日志
        /// </summary>
        /// <param name="PlanInfo">PlanInfo</param>
        public static void AddPlanOperate(PlanInfo PlanInfo, object result/*状态,从Operate枚举中选取*/, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblPlanOperate", obj).FirstOrDefault().Value;
            var loopplan = new tblPlanOperate
            {
                planId = PlanInfo.planId,
                operateId = operId,
                message = PlanInfo.eventOutput,
                result = Convert.ToInt32(result),
                reviewTime = DateTime.Now,
                reviewUser = PlanInfo.createUser
            };
            db.tblPlanOperate.Add(loopplan);
        }

        #endregion
    }
}