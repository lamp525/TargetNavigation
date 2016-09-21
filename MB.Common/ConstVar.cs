namespace MB.Common
{
    public static class ConstVar
    {
        public const int adminId = 1;

        /// <summary>
        /// 统计时间周期类型
        /// </summary>
        public enum StatisticsType
        {
            Day = 0,

            Week = 1,

            Month = 2,

            Year = 3
        }

        /// <summary>
        /// 标签类别
        /// </summary>
        public enum TagType
        {
            /// <summary>计划标签</summary>
            Plan = 1,

            /// <summary>循环计划标签</summary>
            LoopPlan = 2,

            /// <summary>目标标签</summary>
            Objective = 3,

            /// <summary>日程标签</summary>
            Calendar = 4,

            /// <summary>新闻标签</summary>
            News = 5,

            /// <summary>个人文档标签</summary>
            UserDocument = 6,

            /// <summary>公司文档标签</summary>
            CompanyDocument = 7
        }

        /// <summary>
        /// 标签检索类别
        /// </summary>
        public enum TagSearchType
        {
            /// <summary>1：计划</summary>
            Plan = 1,

            /// <summary>2：目标</summary>
            Objective = 2,

            /// <summary>3：日程</summary>
            Calendar = 3,

            /// <summary>4：新闻</summary>
            News = 4,

            /// <summary>5：文档</summary>
            Document = 5
        }

        /// <summary>
        /// 公司文档权限对象类型
        /// </summary>
        public enum FolderAuthType
        {
            ///<summary>1：组织架构</summary>
            organization = 1,

            ///<summary>2：岗位</summary>
            station = 2,

            ///<summary>3：用户</summary>
            user = 3
        }

        public enum FolderAuthPower
        {
            ///<summary>禁止访问</summary>
            NoAccese = 1,

            ///<summary>仅下载</summary>
            DownloadOnly = 2,

            ///<summary>下载和上传</summary>
            DownloadAndUplodload = 3,

            ///<summary>完全控制</summary>
            FullControl = 4
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        public enum ControlType
        {
            /// <summary>0:标签</summary>
            Tag = 0,

            /// <summary>1：文本输入框</summary>
            TextBox = 1,

            /// <summary>2：数字输入框</summary>
            NumberInput = 2,

            /// <summary>3：金额小写</summary>
            MoneyLower = 3,

            /// <summary>4：单选框 </summary>
            RadioButton = 4,

            /// <summary>5：复选框</summary>
            CheckBox = 5,

            /// <summary>6：下拉列表</summary>
            ComboBox = 6,

            /// <summary>7：浏览人力资源</summary>
            BrowseHR = 7,

            /// <summary>8：浏览组织架构</summary>
            BrowseOrg = 8,

            /// <summary>9：浏览文件</summary>
            BrowseDoc = 9,

            /// <summary>10：分割线</summary>
            Line = 10,

            /// <summary>11：多行文本框</summary>
            MultiTextBox = 11,

            /// <summary>12：日期</summary>
            Date = 12,

            /// <summary>13：日期区间</summary>
            DateRange = 13,

            /// <summary>14：日期时间</summary>
            DateTime = 14,

            /// <summary>15：日期时间区间</summary>
            DateTimeRange = 15,

            /// <summary>16：明细列表</summary>
            DetailList = 16,

            /// <summary>17：金额大写</summary>
            MoneyUpper = 17,

            /// <summary>   18:一行两列布局 </summary>
            TwoColumnRow = 18,

            /// <summary> 19：一行三列布局</summary>
            ThreeColumnRow = 19
        }

        /// <summary>
        /// 模板状态
        /// </summary>
        public enum TemplateStatus
        {
            /// <summary>0:保存</summary>
            save = 0,

            /// <summary>1:使用</summary>
            used = 1
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public enum NodeType
        {
            /// <summary>1：创建</summary>
            Create = 1,

            /// <summary>2：提交</summary>
            Submit = 2,

            /// <summary>3：审批</summary>
            Approval = 3,

            /// <summary>4：归档</summary>
            End = 4
        }

        /// <summary>
        /// 节点状态（操作类型）
        /// </summary>
        public enum NodeStatus
        {
            /// <summary>0：无（审批）</summary>
            Approval = 0,

            /// <summary>1：会签</summary>
            Countersign = 1,

            /// <summary>2：抄送</summary>
            Duplicate = 2,

            /// <summary>3：提交</summary>
            Submit = 3,

            ///<summary>4：归档</summary>
            Archive = 4
        }

        /// <summary>
        /// 首页模块类型
        /// </summary>
        public enum ModuleType
        {
            /// <summary>1：新闻</summary>
            New = 1,

            /// <summary>2：通知</summary>
            Notice = 2,

            /// <summary>3：文档</summary>
            Document = 3,

            /// <summary>4：图片轮播</summary>
            Image = 4,

            /// <summary>5：绩效统计</summary>
            Performance = 5
        }

        /// <summary>
        /// 节点操作人类型
        /// </summary>
        public enum NodeOperatorType
        {
            /// <summary>1：操作者部门</summary>
            Organization = 1,

            /// <summary>2：操作者岗位</summary>
            Station = 2,

            /// <summary>3：操作者</summary>
            User = 3,

            /// <summary>4：上级岗位</summary>
            Superior = 4,

            /// <summary>5：所有人</summary>
            All = 5
        }

        /// <summary>
        /// 批次条件类型
        /// </summary>
        public enum BatchType
        {
            /// <summary>0：无批次条件</summary>
            None = 0,

            /// <summary>1：申请人部门</summary>
            Organization = 1,

            /// <summary>2：申请人岗位</summary>
            Station = 2,

            /// <summary>3：申请人</summary>
            User = 3
        }

        /// <summary>
        /// 流程条件类型
        /// </summary>
        public enum ConditionType
        {
            /// <summary>1：组织架构</summary>
            Organization = 1,

            /// <summary>2：岗位</summary>
            Station = 2,

            /// <summary>3：人力资源</summary>
            User = 3,

            /// <summary>4：控件</summary>
            Control = 4
        }

        /// <summary>
        /// 条件操作
        /// </summary>
        public enum ConditionOperate
        {
            /// <summary>1：属于</summary>
            Belong = 1,

            /// <summary>2：不属于</summary>
            NotBelong = 2,

            /// <summary>3：等于</summary>
            Equal = 3,

            /// <summary>4：大于</summary>
            More = 4,

            /// <summary>5：小于</summary>
            Less = 5
        }

        /// <summary>
        /// 节点流程状态
        /// </summary>
        public enum LinkStatus
        {
            /// <summary>0：不通过</summary>
            Deny = 0,

            /// <summary>1：通过</summary>
            Pass = 1,
        }

        /// <summary>
        /// 表单操作类型
        /// </summary>
        public enum FormOperateType
        {
            /// <summary> 1：提交</summary>
            Submit = 1,

            /// <summary>2：通过</summary>
            Pass = 2,

            /// <summary>3：退回</summary>
            Return = 3,

            /// <summary>4：撤销 </summary>
            Revoke = 4,

            /// <summary>5：下载</summary>
            Download = 5,

            /// <summary>6：查看</summary>
            Read = 6,

            /// <summary>7：归档</summary>
            Archive = 7
        }

        /// <summary>
        /// 表单流程状态
        /// </summary>
        public enum FormFlowStatusId
        {
            /// <summary>1、待提交</summary>
            unSubmit = 1,

            /// <summary>2、已提交</summary>
            hasSubmited = 2,

            /// <summary>3、待处理</summary>
            unCheck = 3,

            /// <summary>4、已处理</summary>
            hasChecked = 4,

            /// <summary>5、已办结</summary>
            hasCompleted = 5,

            /// <summary>6、待审核</summary>
            unExamine = 6,

            /// <summary>7、待查阅</summary>
            unRead = 7,

            /// <summary>8、已审核</summary>
            hasExamine = 8,

            /// <summary>9、已查阅</summary>
            hasRead = 9,

            /// <summary>10、节点类型:提交</summary>
            submitMsg = 10,

            /// <summary>11、管理员：流程中</summary>
            flowing = 11,

            /// <summary>12、委托 </summary>
            FlowEntrust = 12,

            /// <summary>13、已提交：从没审批或被他人提交过</summary>
            neverChecked = 13
        }

        /// <summary>
        /// 表单抄送状态
        /// </summary>
        public enum FormDuplicateStatus
        {
            /// <summary>0：查阅</summary>
            Ready = 0,

            /// <summary>1：提交</summary>
            Submit = 1,

            /// <summary>2：审批</summary>
            Approval = 2,

            /// <summary>3：会签</summary>
            countersign = 3,

            /// <summary>4：归档</summary>
            Archive = 4,
        }

        /// <summary>
        /// 上传类型
        /// </summary>
        public enum UploadType
        {
            /// <summary>1：新闻图片</summary>
            NewsImage = 1,

            /// <summary>2：公司文档</summary>
            CompanyDocument = 2,

            /// <summary>3：计划附件</summary>
            PlanAttachment = 3,

            /// <summary>4：个人文档</summary>
            UserDocument = 4,

            /// <summary>5：用户头像</summary>
            HeadImage = 5,

            /// <summary>6：IM文件</summary>
            ImFile = 6,

            /// <summary>7：目标文件</summary>
            ObjectiveFile = 7,

            /// <summary>8:流程首页文档</summary>
            FlowIndexFile = 8,

            /// <summary>9:会议附件</summary>
            MeetingFile = 9
        }

        /// <summary>
        /// 在职状态
        /// </summary>
        public enum workStatus
        {
            /// <summary>0：离职</summary>
            Quit = 0,

            /// <summary>1：在职</summary>
            OnWork = 1,

            /// <summary>2：退休</summary>
            Retired = 2
        }

        /// <summary>
        /// 表单状态
        /// </summary>
        public enum userFormStatus
        {
            /// <summary>1、待提交</summary>
            unSubmit = 1,

            /// <summary>2、流程中</summary>
            flowing = 2,

            /// <summary>50、已办结</summary>
            hasCompleted = 50
        }

        /// <summary>
        /// 控件权限
        /// </summary>
        public enum nodeControlStatus
        {
            /// <summary>0、隐藏 </summary>
            hide = 0,

            /// <summary>1、只读</summary>
            readOnly = 1,

            /// <summary>2、编辑</summary>
            edit = 2
        }

        /// <summary>
        /// 目标首页状态
        /// </summary>
        public enum ObjectIndexStatus
        {
            /// <summary>1：待提交</summary>
            unSubmit = 1,

            /// <summary>2:待审核</summary>
            unChecked = 2,

            /// <summary>3：审核通过</summary>
            hasChecked = 3,

            /// <summary>4：待确认</summary>
            unConfirm = 4,

            /// <summary>5：已完成</summary>
            hasCompleted = 5,

            /// <summary>6、超时</summary>
            overTime = 6,
        }

        /// <summary>
        /// 目标日志操作类型
        /// </summary>
        public enum ObjectiveOperaResult
        {
            /// <summary>1：创建</summary>
            create = 1,

            /// <summary>2：删除</summary>
            delete = 2,

            /// <summary>3：授权</summary>
            authorize = 3,

            /// <summary>4：提交</summary>
            submit = 4,

            /// <summary>5：撤销</summary>
            revoke = 5,

            /// <summary>6：审核通过</summary>
            checkPass = 6,

            /// <summary>7：审核不通过</summary>
            checkNoPass = 7,

            /// <summary> 8：修改</summary>
            update = 8,

            /// <summary>9：分解目标</summary>
            resolve = 9,

            /// <summary>10：更新进度</summary>
            updateProcess = 10,

            /// <summary>11：确认通过</summary>
            confirmPass = 11,

            /// <summary>12：确认不通过 </summary>
            confirmNoPass = 12,

            /// <summary>13：下载</summary>
            down = 13,

            /// <summary>14：查看</summary>
            read = 14,

            /// <summary>15：上传</summary>
            upload = 15,

            /// <summary>16：删除文档</summary>
            deleteDoc = 16,

            /// <summary>17:提交确认</summary>
            submitConfirm = 17,

            /// <summary>取消授权</summary>
            cancelAuthorize = 18
        }

        /// <summary>
        /// 目标考核类型
        /// </summary>
        public enum ObjectiveCheckType
        {
            ///<summary> 1: 金额</summary>
            Amount = 1,

            /// <summary>2：时间</summary>
            Time = 2,

            /// <summary>3：数字</summary>
            Number = 3
        }

        //目标公式类型
        public enum ObjectiveFormulaType
        {
            /// <summary>0：无公式</summary>
            noFormula = 0,

            /// <summary>1:默认公式</summary>
            defaultFormula = 1,

            /// <summary>2:自定义</summary>
            userDefined = 2
        }

        //目标公式字段类型
        public enum ObjectiveFormulaField
        {
            /// <summary>1：实际值</summary>
            actualValue = 1,

            /// <summary>2：指标值</summary>
            objectiveValue = 2,

            /// <summary>3：理想值</summary>
            expectedValue = 3,

            /// <summary>4：开始时间</summary>
            startTime = 4,

            /// <summary>5：结束时间</summary>
            endTime = 5,

            /// <summary>6：:警戒时间</summary>
            alarmTime = 6,

            /// <summary>7：权重</summary>
            weight = 7,

            /// <summary>8：奖励基数</summary>
            bonus = 8,

            /// <summary>9：数字</summary>
            num = 9
        }

        public enum MeetingStatus
        {
            /// <summary>0：全部 </summary>
            all = 0,

            /// <summary>1：未进行</summary>
            unDone = 1,

            /// <summary>2:已完成</summary>
            hasComplete = 2
        }

        public enum MeetingMember
        {
            /// <summary>主讲人 </summary>
            speaker = 0,

            /// <summary>参与人</summary>
            joining = 1
        }

        public enum LoopPlanType
        {
            /// <summary>1:日循环 </summary>
            day = 1,

            /// <summary>2:周循环 </summary>
            week = 2,

            /// <summary>3:月循环 </summary>
            month = 3,

            /// <summary>4:年循环 </summary>
            year = 4
        }

        public enum LoopPlanStatus
        {
            /// <summary>0:做成中</summary>
            unSubmit = 0,

            /// <summary>10:审核中</summary>
            unCheck = 10,

            /// <summary>15:审核未通过 </summary>
            checkNoPass = 15,

            /// <summary>20:审核通过</summary>
            checkPass = 20,

            /// <summary>25:申请修改</summary>
            update = 25
        }

        //循环计划操作日志类型
        public enum LoopPlanOperateStatus
        {
            /// <summary>1、提交确认</summary>
            submit = 1,

            /// <summary>2、审核通过</summary>
            checkPass = 2,

            /// <summary>3、审核不通过</summary>
            checkNoPass = 3,

            /// <summary>4、取消提交 </summary>
            cancelSubmit = 4,

            /// <summary>5、取消审核</summary>
            cancelCheck = 5,

            /// <summary>6、确认通过</summary>
            confirmPass = 6,

            /// <summary>7、确认不通过</summary>
            confirmNoPass = 7,

            /// <summary>8、中止</summary>
            stop = 8,

            /// <summary>9、申请修改</summary>
            update = 9,

            /// <summary>10、提交循环计划</summary>
            submitPlan = 10,

            /// <summary>11、下载附件</summary>
            downLoad = 11,

            /// <summary>12、新建计划</summary>
            create = 12
        }

        public enum PlanOperateStatus
        {
            /// <summary>1、提交</summary>
            submit = 1,

            /// <summary>2、审核通过</summary>
            checkPass = 2,

            /// <summary>3、审核不通过</summary>
            checkNoPass = 3,

            /// <summary>4、取消提交</summary>
            cancelSubmit = 4,

            /// <summary>5、取消审核</summary>
            cancelCheck = 5,

            /// <summary>6、评论</summary>
            discuss = 6,

            /// <summary>7\下载</summary>
            downLoad = 7,

            /// <summary>8、查看</summary>
            read = 8,

            /// <summary>9、转办</summary>
            change = 9,

            /// <summary>10、申请修改</summary>
            update = 10,

            /// <summary>11、申请中止</summary>
            stop = 11,

            /// <summary>12、重新开始</summary>
            reStart = 12,

            /// <summary>13、删除 </summary>
            delete = 13,

            /// <summary>14、确认通过</summary>
            confirmPass = 14,

            /// <summary>15、确认未通过</summary>
            confirmNoPass = 15,

            /// <summary>16、更新进度</summary>
            updateProcess = 16,

            /// <summary>17、分解</summary>
            resolve = 17,

            /// <summary>18、新建计划</summary>
            createPlan = 18,

            /// <summary>19、新建循环计划</summary>
            createLoopPlan = 19,

            /// <summary>20、修改保存</summary>
            updateSave = 20
        }

        public enum ReturnField
        {
            /// <summary>0:成功</summary>
            success = 0,

            /// <summary>-1:失败</summary>
            error = -1
        }
    }
}