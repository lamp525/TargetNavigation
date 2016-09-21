using System;
using System.ServiceProcess;
using System.Timers;
using MB.Common;

namespace FW.MBService
{
    public partial class MBService : ServiceBase
    {
        public MBService()
        {
            InitializeComponent();
        }

        private log4net.ILog _log = log4net.LogManager.GetLogger("mbService");
        private DateTime _lastSatisticsTime = DateTime.MinValue;

        #region 服务启动关闭处理

        protected override void OnStart(string[] args)
        {
            try
            {
                _log.Info("目标导航服务开始运行");

                //this.ProcessStatistics();

                Timer exeTimer = new Timer();
                exeTimer.Interval = 3600000;        //一小时  60*60*1000
                exeTimer.Elapsed += new ElapsedEventHandler(exeTimer_Elapsed);
                exeTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            _log.Info("目标导航服务停止运行");
        }

        #endregion 服务启动关闭处理

        private void exeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_lastSatisticsTime.Date.Equals(DateTime.Now.Date))
            {
                this.ProcessStatistics();
            }
        }

        private void ProcessStatistics()
        {
            _log.Info("开始执行数据统计处理……");

            var statisticsTime = DateTime.Now;

            //每天开始统计
            _log.Info("有效工时统计开始");
            WorktimeStatistics wts = new WorktimeStatistics();
            wts.StatisticsWorkTime();
            _log.Info("有效工时统计完成");

            _log.Info("奖惩情况统计开始");
            RewardPunish rp = new RewardPunish();
            rp.GetRewardpunishStatistics(statisticsTime.AddDays(-1));
            _log.Info("奖惩情况统计完成");

            //每周一开始统计
            if (statisticsTime.DayOfWeek == DayOfWeek.Monday)
            {
                //上周一的日期
                DateTime startTime = statisticsTime.AddDays(-7);
                //上周日的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                _log.Info("周计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Week);
                _log.Info("周计划完成情况统计完成");

                _log.Info("周目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Week);
                _log.Info("周目标完成情况统计完成");
            }

            //每月第一天开始统计
            if (statisticsTime.Day == 1)
            {
                //上月第一天的日期
                DateTime startTime = statisticsTime.AddMonths(-1);
                //上月最后一天的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                _log.Info("月计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Month);
                _log.Info("月计划完成情况统计完成");

                _log.Info("月目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Month);
                _log.Info("月目标完成情况统计完成");
            }

            //每年的第一天开始统计
            if (statisticsTime.DayOfYear == 1)
            {
                //上一年度第一天的日期
                DateTime startTime = statisticsTime.AddYears(-1);
                //上一年度最后一天的日期
                DateTime endTime = statisticsTime.AddDays(-1);

                _log.Info("年计划完成情况统计开始");
                PlanStatistics ps = new PlanStatistics();
                ps.Statistics(startTime, endTime, ConstVar.StatisticsType.Year);
                _log.Info("年计划完成情况统计完成");

                _log.Info("年目标完成情况统计开始");
                ObjectiveStatistics os = new ObjectiveStatistics();
                os.Statistics(startTime, endTime, ConstVar.StatisticsType.Year);
                _log.Info("年目标完成情况统计完成");
            }

            //保存本次统计时间
            _lastSatisticsTime = statisticsTime;

            _log.Info("数据统计处理完成！");
        }
    }
}