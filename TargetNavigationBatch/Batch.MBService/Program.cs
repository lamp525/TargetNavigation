using System;
using FW.MBService;
using MB.Common;

namespace Batch.MBService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("激励统计测试用Batch");
                Console.WriteLine();

                ProcessStatistics();
            }
            catch (Exception)
            {
                Console.WriteLine("系统错误：统计未完成 ！");
            }
        }

        private static void ProcessStatistics()
        {
            Console.WriteLine("开始执行数据统计处理……");

            var statisticsTime = DateTime.Now;

            //每天开始统计
            Console.WriteLine("有效工时统计开始");
            WorktimeStatistics wts = new WorktimeStatistics();
            wts.StatisticsWorkTime();
            Console.WriteLine("有效工时统计完成");

            Console.WriteLine("奖惩情况统计开始");
            RewardPunish rp = new RewardPunish();
            rp.GetRewardpunishStatistics(statisticsTime.AddDays(-1));
            Console.WriteLine("奖惩情况统计完成");

            //每周一开始统计
            if (statisticsTime.DayOfWeek == DayOfWeek.Monday)
            {
                //上周一的日期
                DateTime startTime = statisticsTime.AddDays(-7);
                //上周日的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                Console.WriteLine("周计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Week);
                Console.WriteLine("周计划完成情况统计完成");

                Console.WriteLine("周目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Week);
                Console.WriteLine("周目标完成情况统计完成");
            }

            //每月第一天开始统计
            if (statisticsTime.Day == 1)
            {
                //上月第一天的日期
                DateTime startTime = statisticsTime.AddMonths(-1);
                //上月最后一天的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                Console.WriteLine("月计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Month);
                Console.WriteLine("月计划完成情况统计完成");

                Console.WriteLine("月目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Month);
                Console.WriteLine("月目标完成情况统计完成");
            }

            //每年的第一天开始统计
            if (statisticsTime.DayOfYear == 1)
            {
                //上一年度第一天的日期
                DateTime startTime = statisticsTime.AddYears(-1);
                //上一年度最后一天的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                Console.WriteLine("年计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Year);
                Console.WriteLine("年计划完成情况统计完成");

                Console.WriteLine("年目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Year);
                Console.WriteLine("年目标完成情况统计完成");
            }

            //保存本次统计时间
            // _lastSatisticsTime = statisticsTime;

            Console.WriteLine("数据统计处理完成！");
        }
    }
}