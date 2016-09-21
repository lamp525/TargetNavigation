using MB.DAL;
using MB.New.Model;
using System.Collections.Generic;

namespace MB.New.BLL.Plan
{
    public interface IPlanCommentBLL
    {
        /// <summary>
        /// 取得计划评论信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        List<PlanCommentModel> GetPlanCommentInfo(TargetNavigationDBEntities db, int planId);

        /// <summary>
        /// 添加计划评论
        /// </summary>
        /// <param name="db"></param>
        /// <param name="commentInfo"></param>
        void InsPlanComment(TargetNavigationDBEntities db, PlanCommentModel commentInfo);
    }
}