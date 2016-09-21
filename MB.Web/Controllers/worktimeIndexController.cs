using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.Common;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    [UserAuthorize]
    public class WorkTimeIndexController : BaseController
    {
        //
        // GET: /worktimeIndex/
        public ActionResult Index()
        {
            return View();
        }

        #region 获取首页工效价值报表的Json格式数据,【总计】

        /// <summary>
        /// 获取首页工效价值报表的Json格式数据
        /// </summary>
        /// <param name="year">日期：年</param>
        /// <param name="month">日期：月</param>
        /// <param name="mode">0/1</param>
        /// <returns></returns>

        public string getJsonTotal(int year, int month, int mode)
        {
            CommonWorkTime comworktime = new CommonWorkTime();
            DateTime time;
            string strtime = year + "/" + month;//拼接日期
            string size = mode == 0 ? "year" : "month";//mode  0是年，1是月，参数转换
            string josnindex;
            if (DateTime.TryParse(strtime, out time))
            {
                List<WorkTimeIndex> worktimeindex = comworktime.getDepartmentIndex(time, size);
                JsonModelIndex jsonmodel = new JsonModelIndex();
                jsonmodel.top10 = worktimeindex;
                int i = worktimeindex.Count > 3 ? 3 : worktimeindex.Count;
                switch (i)
                {
                    case 1:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size) };
                        break;

                    case 2:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[1].orgid, time, size) };
                        break;

                    case 3:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[1].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[2].orgid, time, size) };
                        break;

                    default:
                        break;
                }
                //JsonModelIndex jsonmodel = new JsonModelIndex { top10 = worktimeindex, top3 = new[] { listTopone, listToptwo, listTopthree } };
                var jsonResult = new JsonResultModel(JsonResultType.success, jsonmodel, "正常");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "请检查传递的参数【后期返回有好错误信息】");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            return josnindex;
        }

        #endregion 获取首页工效价值报表的Json格式数据,【总计】

        #region 获取首页工效价值报表的Json格式数据,【人均】

        /// <summary>
        /// 获取首页工效价值报表的Json格式数据
        /// </summary>
        /// <param name="year">日期：年</param>
        /// <param name="month">日期：月</param>
        /// <param name="mode">0/1</param>
        /// <returns></returns>

        public string getJson(int year, int month, int mode)
        {
            CommonWorkTime comworktime = new CommonWorkTime();
            DateTime time;
            string strtime = year + "/" + month;//拼接日期
            string size = mode == 0 ? "year" : "month";//mode  0是年，1是月，参数转换
            string josnindex;
            if (DateTime.TryParse(strtime, out time))
            {
                List<WorkTimeIndex> worktimeindex = comworktime.getDepartmentIndexAverage(time, size);
                JsonModelIndex jsonmodel = new JsonModelIndex();
                jsonmodel.top10 = worktimeindex;
                int i = worktimeindex.Count > 3 ? 3 : worktimeindex.Count;
                switch (i)
                {
                    case 1:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size) };
                        break;

                    case 2:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[1].orgid, time, size) };
                        break;

                    case 3:
                        jsonmodel.top3 = new[] { comworktime.getDepartmentlWorktime(worktimeindex[0].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[1].orgid, time, size), comworktime.getDepartmentlWorktime(worktimeindex[2].orgid, time, size) };
                        break;

                    default:
                        break;
                }
                //JsonModelIndex jsonmodel = new JsonModelIndex { top10 = worktimeindex, top3 = new[] { listTopone, listToptwo, listTopthree } };
                var jsonResult = new JsonResultModel(JsonResultType.success, jsonmodel, "请检查传递的参数【后期返回有好错误信息】");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            else
            {
                var jsonResult = new JsonResultModel(JsonResultType.success, null, "请检查传递的参数【后期返回有好错误信息】");
                josnindex = JsonConvert.SerializeObject(jsonResult);
            }
            return josnindex;
        }

        #endregion 获取首页工效价值报表的Json格式数据,【人均】
    }
}