using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class ExecutionModeBLL : IExecutionModeBLL
    {
        /// <summary>
        /// 执行方式获取
        /// </summary>
        /// <returns></returns>
        public List<ExecutionMode> GetExecutionList()
        {
            using (var db = new TargetNavigationDBEntities())
            {
                var Execution = (from ex in db.tblExecutionMode
                                 where ex.deleteFlag == false
                                 select new ExecutionMode
                                 {
                                     executionId = ex.executionId,
                                     executionMode = ex.executionMode
                                 }).ToList();
                return Execution;
            }
        }

        /// <summary>
        /// 新增执行方式
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddNewExecution(ExecutionMode list)
        {
            var flag = 1;

            list.createTime = DateTime.Now;
            list.updateTime = DateTime.Now;
            using (var db = new TargetNavigationDBEntities())
            {
                var hasExecution = db.tblExecutionMode.Where(p => p.executionMode == list.executionMode).FirstOrDefault();
                if (hasExecution != null)
                {
                    flag = 2;
                }
                else
                {
                    System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                    list.executionId = db.prcGetPrimaryKey("tblExecutionMode", obj).FirstOrDefault().Value;
                    var ValueIncentiveCustom = new tblExecutionMode
                    {
                        executionId = list.executionId,
                        executionMode = list.executionMode,
                        createTime = list.createTime,
                        createUser = list.createUser,
                        deleteFlag = list.deleteFlag,
                        updateTime = list.updateTime,
                        updateUser = list.updateUser
                    };
                    db.tblExecutionMode.Add(ValueIncentiveCustom);

                    db.SaveChanges();
                    flag = 3;
                }
            }

            return flag;
        }

        /// <summary>
        /// 删除执行方式
        /// </summary>
        /// <param name="id">id列表</param>
        /// <returns></returns>
        public bool DeleteExection(int id)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                var olddata = db.tblExecutionMode.Where(p => p.executionId == id).FirstOrDefault();
                if (olddata != null)
                {
                    db.tblExecutionMode.Remove(olddata);
                }

                db.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 更新执行
        /// </summary>
        /// <param name="list">更新列表</param>
        /// <returns></returns>
        public int Update(ExecutionMode list)
        {
            var flag = 1;
            using (var db = new TargetNavigationDBEntities())
            {
                var hasExecution = db.tblExecutionMode.Where(p => p.executionMode == list.executionMode).FirstOrDefault();
                if (hasExecution != null)
                {
                    flag = 2;
                }
                else
                {
                    var model = db.tblExecutionMode.Where(p => p.executionId == list.executionId).FirstOrDefault();

                    if (model != null)
                    {
                        model.executionMode = list.executionMode;
                        model.updateUser = list.updateUser;
                        model.updateTime = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
            return flag;
        }
    }
}