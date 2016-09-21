using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MB.New.BLL.Plan
{
    public class PlanCommentBLL : IPlanCommentBLL
    {
        /// <summary>
        /// 取得计划评论信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        public List<PlanCommentModel> GetPlanCommentInfo(TargetNavigationDBEntities db, int planId)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            var result = (from ps in db.tblPlanSuggestion
                          join u in db.tblUser
                          on ps.replyUser equals u.userId
                          where ps.planId == planId && ps.deleteFlag != true 
                          orderby ps.createTime descending
                          select new PlanCommentModel
                          {
                              planId = ps.planId,
                              suggestionId = ps.suggestionId,
                              suggestion = ps.suggestion,
                              replyUser = ps.replyUser,
                              replyUserName = u.userName,
                              replyUserImage = string.IsNullOrEmpty(u.bigImage) ? ConstVar.DefaultUserHead : "/" + ConstVar.HeadImageUpLoadPath + "/" + u.bigImage,
                              createTime = ps.createTime,
                              createUser = ps.createUser,
                              updateTime = ps.updateTime,
                              updateUser = ps.updateUser
                          }).ToList();

            return result;
        }

        /// <summary>
        /// 添加计划评论
        /// </summary>
        /// <param name="db"></param>
        /// <param name="commentInfo"></param>
        public void InsPlanComment(TargetNavigationDBEntities db, PlanCommentModel commentInfo)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            //取得主键
            var id = DBUtility.GetPrimaryKeyByTableName(db, "tblPlanSuggestion");
            var model = new tblPlanSuggestion
            {
                suggestionId = id,
                planId = commentInfo.planId.Value,
                suggestion = commentInfo.suggestion,
                replyUser = commentInfo.replyUser,
                createTime = commentInfo.createTime.Value,
                createUser = commentInfo.createUser.Value,
                updateTime = commentInfo.updateTime.Value,
                updateUser = commentInfo.updateUser.Value,
                deleteFlag = false
            };
            db.tblPlanSuggestion.Add(model);
        }
    }
}