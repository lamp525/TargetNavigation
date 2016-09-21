using System;
using System.Collections.Generic;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public interface IObjectiveIndexBLL
    {
        /// 获取目标首页列表
        ObjectiveInfo GetObjectiveList(string condition, DateTime start, DateTime end, int userId);

        /// 根据当前目标Id获取子目标列表
        ObjectiveInfo GetChildrenObjectiveList(int objectiveId, int userId);

        /// 目标展开数据
        ObjectiveHasChildModel ExpandObjective(int objectiveId);

        /// 获取甘特图列表
        List<GanttChartModel> GetGanttChartObjectiveList(DateTime thisStart, DateTime thisEnd, int userId);

        /// 获取甘特图子列表
        List<GanttChartModel> GetGanttChartChildObjectiveList(int objectiveId);

        /// 删除目标
        bool DeleteObjective(int objectiveId, int userId);

        /// 目标授权
        bool AuthorizeObjective(int objectiveId, int authorizedUser, int userId);

        /// 取消授权
        bool CancelAuthorizeObjective(int objectiveId, int userId);

        /// 撤销操作
        bool RevokeObjective(int objectiveId, int userId);

        /// 新建目标提交保存操作
        ReturnConfirm NewObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, out int objectiveId);

        /// 待提交目标提交保存操作
        ReturnConfirm UpdateObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, int operateFlag);

        /// 分解目标提交保存操作
        ReturnConfirm SplitObjective(AddNewObjectiveModel objectiveModel, int userId, int flag, out int objectiveId);

        /// 修改目标数据
        ReturnConfirm EditObjective(AddNewObjectiveModel objectiveModel, int userId, int flag);

        /// 删除目标文档
        bool DeleteDocument(int objectiveId, int attachmentId, int userId);

        /// 文件上传成功后数据库插数据
        void InsertObjectiveDoc(int objectiveId, UploadFileModel file, int userId);

        /// 甘特图拖动更新
        bool MoveObjectiveGanttChart(int objectiveId, DateTime fromTime, DateTime toTime, int userId, int flag);

        /// 甘特图子目标更新
        void MoveObjectiveGanttChart(TargetNavigationDBEntities db, int objectiveId, int day, int userId);

        /// 获取目标详情
        ObjectiveIndexModel GetObjectInfo(int objectiveId);

        /// 获取待提交状态的目标详情
        ObjectiveIndexModel GetSimpleObjectInfo(int objectiveId);

        /// 目标审核
        ReturnConfirm ApproveObjective(ObjectiveCheckModel checkModel, int userId);

        /// 目标提交确认
        int SubmitObjectiveExecuteResult(int objectiveId, string actualValue, int userId);

        /// 目标确认
        bool ConfirmObjective(int objectiveId, string message, int result, int userId);

        /// 获取饼图数量统计
        List<FlowProcessModel> GetObjectiveProcessList(int year, int month, int userId);

        /// 获取目标不同状态的数量统计信息
        List<FlowProcessModel> GetObjectiveStatusList(int userId, int flag);

        /// 更新目标进度
        bool UpdateObjectiveProcess(int objectiveId, int newProcess, int userId);

        /// 获取目标公式信息
        List<ObjectiveFormula> GetObjectFormula(int objectiveId);

        /// 获取目标文档信息
        List<ObjectiveDocumentInfo> GetObjectiveNewDocuments(int objectiveId);

        /// 获取目标操作日志
        List<ObjectiveOperateLog> GetObjectiveNewLogs(int objectiveId);

        /// 添加下载日志
        void AddDownLoadOperate(int objectiveId, int userId);

        /// 判断该目标能否移动
        bool CheckObjectiveMove(int objectiveId);

        /// 递归子目标
        bool CheckChildObjective(TargetNavigationDBEntities db, int objectiveId, bool flag);

        /// 目标公式检查
        int CheckFormulaValidity(int objectiveCheckType, List<ObjectiveFormula> formulaInfo);

        //计划日程化获取目标列表
        ObjectiveInfo GetCalendarObjectiveList(string condition, DateTime date, int userId);

        //获取目标超时列表
        ObjectiveInfo GetObjectiveOverTimeList(string condition, DateTime end, int userId);
    }
}