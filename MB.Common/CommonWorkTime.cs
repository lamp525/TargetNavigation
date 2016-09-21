using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.Common
{
    public class CommonWorkTime
    {
        #region 根据获取的 用户ID/部门ID，时间，年月; 返回其 实际工时、有效工时、时间（月/日）

        /// <summary>
        /// 根据用户ID返回用户的"实际工时"and"有效工时"
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="time">时间（eg:2015-3-3）</param>
        /// <param name="size">year/month</param>
        /// <returns></returns>
        public List<Worktime> getPersonalWorktime(int id, DateTime time, string size)
        {
            List<Worktime> mList = new List<Worktime>();
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                List<worktime> list = db.prcPersonalWorktime(id, time, size).ToList();

                foreach (var li in list)
                {
                    var mworktime = new Worktime
                    {
                        totalwork = li.totalwork,
                        totaleffective = li.totaleffective,
                        workdate = li.workdate
                    };
                    mList.Add(mworktime);
                }
                switch (size)
                {
                    case "year":
                        FullMonth(mList, time);
                        break;

                    case "month":
                        FullDay(mList, time);
                        break;

                    default:
                        break;
                }
                return mList;
            }
        }

        ///根据表名获取id值
        public int GetPlanidByTblName(string tableName)
        {
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var PlanId = db.prcGetPrimaryKey(tableName, obj);

                return Convert.ToInt32(PlanId.FirstOrDefault());
            }
        }

        /// <summary>
        /// 根据部门ID返回部门总共的"实际工时"and"有效工时"
        /// </summary>
        /// <param name="id">部门id</param>
        /// <param name="time">时间（eg:2015-3-3）</param>
        /// <param name="size">year/month</param>
        /// <returns></returns>
        public List<Worktime> getDepartmentlWorktime(int? id, DateTime time, string size)
        {
            List<Worktime> mList = new List<Worktime>();
            using (TargetNavigationDBEntities db = new TargetNavigationDBEntities())
            {
                List<worktime> list = db.prcDepartmentlWorktime(id, time, size).ToList();

                foreach (var li in list)
                {
                    var mworktime = new Worktime
                    {
                        totalwork = li.totalwork,
                        totaleffective = li.totaleffective,
                        workdate = li.workdate
                    };
                    mList.Add(mworktime);
                }
                switch (size)
                {
                    case "year":
                        FullMonth(mList, time);
                        break;

                    case "month":
                        FullDay(mList, time);
                        break;

                    default:
                        break;
                }
                return mList;
            }
        }

        /// <summary>
        /// 获取首页报表TOP10数据
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="size">year/month</param>
        /// <returns></returns>
        public List<WorkTimeIndex> getDepartmentIndex(DateTime time, string size)
        {
            List<WorkTimeIndex> mList = new List<WorkTimeIndex>();
            using (var db = new TargetNavigationDBEntities())
            {
                List<worktimeindex> list = db.prcDepartmentIndex(time, size).ToList();
                foreach (var li in list)
                {
                    var mworktimeindex = new WorkTimeIndex
                    {
                        totalwork = li.totalwork,
                        totaleffective = li.totaleffective,
                        orgid = li.orgid,
                        orgname = li.orgname,
                        workdate = li.workdate
                    };
                    mList.Add(mworktimeindex);
                }
            }
            return mList;
        }

        /// <summary>
        /// 获取首页报表，人均TOP10数据
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="size">year/month</param>
        /// <returns></returns>
        public List<WorkTimeIndex> getDepartmentIndexAverage(DateTime time, string size)
        {
            List<WorkTimeIndex> mList = new List<WorkTimeIndex>();
            using (var db = new TargetNavigationDBEntities())
            {
                List<worktimeindex> list = db.prcDepartmentIndexAverage(time, size).ToList();
                foreach (var li in list)
                {
                    var mworktimeindex = new WorkTimeIndex
                    {
                        totalwork = li.totalwork,
                        totaleffective = li.totaleffective,
                        orgid = li.orgid,
                        orgname = li.orgname,
                        workdate = li.workdate
                    };
                    mList.Add(mworktimeindex);
                }
            }
            return mList;
        }

        #endregion 根据获取的 用户ID/部门ID，时间，年月; 返回其 实际工时、有效工时、时间（月/日）

        #region 补全 月/日 数据为NULL,日期为缺失日期

        /// <summary>
        /// 补足月份,并且按月份从小到大排列
        /// </summary>
        /// <param name="list"></param>
        private void FullMonth(List<Worktime> list, DateTime date)
        {
            DateTime dtime = DateTime.Now;
            for (int i = 1; i <= 12; i++)
            {
                if (!list.Exists(x => x.workdate == i))
                {
                    if (date.Year < dtime.Year)
                    {
                        var mworktime = new Worktime
                        {
                            workdate = i,
                            totaleffective = 0.0m,
                            totalwork = 0.0m
                        };
                        list.Add(mworktime);
                    }
                    else if (date.Year == dtime.Year)
                    {
                        if (i > date.Month)
                        {
                            var mworktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = null,
                                totalwork = null
                            };
                            list.Add(mworktime);
                        }
                        else
                        {
                            var mworktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = 0.0m,
                                totalwork = 0.0m
                            };
                            list.Add(mworktime);
                        }
                    }
                    else
                    {
                        var mworktime = new Worktime
                        {
                            workdate = i,
                            totaleffective = null,
                            totalwork = null
                        };
                        list.Add(mworktime);
                    }
                }
            }
            list.Sort((x, y) => x.workdate.Value - y.workdate.Value);
        }

        /// <summary>
        /// 补足月份中无数据的日，并且按照日从小到大排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="date"></param>
        private void FullDay(List<Worktime> list, DateTime date)
        {
            int MaxDays = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime dtime = DateTime.Now;
            for (int i = 1; i <= MaxDays; i++)
            {
                if (!list.Exists(x => x.workdate == i))
                {
                    if (dtime > date)
                    {
                        if (dtime.Year == date.Year && date.Month == dtime.Month && dtime.Day >= i)
                        {
                            var worktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = 0.0m,
                                totalwork = 0.0m
                            };
                            list.Add(worktime);
                        }
                        else if (dtime.Year == date.Year && date.Month < dtime.Month)
                        {
                            var worktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = 0.0m,
                                totalwork = 0.0m
                            };
                            list.Add(worktime);
                        }
                        else if (dtime.Year > date.Year)
                        {
                            var worktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = 0.0m,
                                totalwork = 0.0m
                            };
                            list.Add(worktime);
                        }
                        else
                        {
                            var worktime = new Worktime
                            {
                                workdate = i,
                                totaleffective = null,
                                totalwork = null
                            };
                            list.Add(worktime);
                        }
                    }
                    else
                    {
                        var worktime = new Worktime
                        {
                            workdate = i,
                            totaleffective = null,
                            totalwork = null
                        };
                        list.Add(worktime);
                    }
                }
            }
            list.Sort((x, y) => x.workdate.Value - y.workdate.Value);
        }

        #endregion 补全 月/日 数据为NULL,日期为缺失日期
    }
}