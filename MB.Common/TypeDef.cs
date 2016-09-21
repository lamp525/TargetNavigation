namespace MB.Common
{
    /// <summary>
    /// 错误等级
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>1：情报</summary>
        INFO = 1,

        /// <summary>2：调试</summary>
        DEBUG = 2,

        /// <summary>3：警告</summary>
        WARN = 3,

        /// <summary>4：错误</summary>
        ERROR = 4,

        /// <summary>5：致命</summary>
        FATAL = 5
    }

    /// <summary>
    /// 上传文件路径
    /// </summary>
    public enum UploadFilePath
    {
        /// <summary> News/新闻---NewsUpLoadPath</summary>
        News = 1,

        /// <summary> Document/文档---DocumentUpLoadPath</summary>
        Document = 2,

        /// <summary> Plan/计划---PlanUpLoadPath</summary>
        Plan = 3,

        /// <summary> Mine/私人---MineUpLoadPath</summary>
        Mine = 4,

        /// <summary>HeadImage/头像---HeadImageUpLoadPath</summary>
        HeadImage = 5
    }

    /// <summary>
    /// 计划操作日志行为
    /// </summary>
    public enum Operate
    {
        提交 = 1,
        审核通过 = 2,
        审核不通过 = 3,
        取消提交 = 4,
        取消审核 = 5,
        评论 = 6,
        下载 = 7,
        查看 = 8,
        转办 = 9,
        申请修改 = 10,
        申请中止 = 11,
        重新开始 = 12,
        删除 = 13,
        确认通过 = 14,
        确认未通过 = 15,
        更新进度 = 16,
        分解计划 = 17,
        新建计划 = 18,
        新建循环计划 = 19,
        修改保存 = 20
    }

    /// <summary>
    /// 计划状态
    /// </summary>
    public enum PlanStatus
    {
        /// <summary>
        /// 未提交
        /// </summary>
        Completing = 0,/*未提交*/

        /// <summary>
        /// 审核中
        /// </summary>
        Reviewing = 10,/*审核中*/

        /// <summary>
        /// 审核未通过
        /// </summary>
        ReviewFail = 15,/*审核未通过*/

        /// <summary>
        /// 审核通过
        /// </summary>
        Reviewed = 20,/*审核通过*/

        /// <summary>
        /// 申请修改
        /// </summary>
        RequestReWork = 25,/*申请修改*/

        /// <summary>
        /// 等待确认
        /// </summary>
        Confirming = 30,/*等待确认*/

        /// <summary>
        /// 确认未通过
        /// </summary>
        ConfirmFail = 40,/*确认未通过*/

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 90/*已完成*/
    }
}