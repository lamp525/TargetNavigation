namespace MB.New.Common
{
    public static class EnumDefine
    {
        #region 共通

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
        /// 布尔结果值
        /// </summary>
        public enum BoolValue
        {
            False = 0,
            True = 1
        }

        /// <summary>
        /// 升序降序
        /// </summary>
        public enum OrderType
        {
            /// <summary>升序</summary>
            Asc = 1,

            /// <summary>降序</summary>
            Desc = 2
        }

        /// <summary>
        /// 滚动条滚动模式
        /// </summary>
        public enum rollMode
        {
            /// <summary>0：默认无滚动 </summary>
            None = 0,

            /// <summary>1：向上滚动</summary>
            Up = 1,

            /// <summary>2：向下滚动</summary>
            Down = 2
        }

        #endregion 共通

        #region 用户相关

        /// <summary>
        /// 常用联系人类型
        /// </summary>
        public enum TopContactsType
        {
            /// <summary>1：上级</summary>
            Superior = 1,

            /// <summary>2：下属</summary>
            Subordinate = 2,

            /// <summary>99：全部</summary>
            Both = 99
        }

        /// <summary>
        /// 在职状态
        /// </summary>
        public enum WorkStatus
        {
            /// <summary>0：离职</summary>
            Quit = 0,

            /// <summary>1：在职</summary>
            OnWork = 1,

            /// <summary>2：退休</summary>
            Retired = 2
        }

        /// <summary>
        /// 密码修改处理结果
        /// </summary>
        public enum ChangePasswordResult
        {
            /// <summary>0：成功</summary>
            Succeed = 0,

            /// <summary>2：密码错误</summary>
            PwdError = 1,

            /// <summary>2：失败</summary>
            Failed = 2
        }

        /// <summary>
        /// 用户登录处理结果
        /// </summary>
        public enum UserLoginResult
        {
            /// <summary>0：登录成功</summary>
            Succeed = 0,

            /// <summary>1：用户名为空</summary>
            BlankUserName = 1,

            ///<summary>2：用户不存在</summary>
            UserNotExist = 2,

            /// <summary>3：密码为空</summary>
            BlankPwd = 3,

            /// <summary>4：密码错误</summary>
            PwdError = 4,

            /// <summary>5：存在重名用户 </summary>
            DuplicationUserName = 5,

            /// <summary>6：验证码为空</summary>
            BlankVCode = 6,

            /// <summary>7：验证码错误</summary>
            VCodeError = 7,

            /// <summary>8：密码错误次数超出设定值</summary>
            WrongPwdOverTime = 8
        }

        #endregion 用户相关

        #region 计划相关

        /// <summary>
        /// 计划快捷筛选
        /// </summary>
        public enum PlanFastFilter
        {
            /// <summary> 0：不筛选 </summary>
            None = 0,

            /// <summary> 1：今日未完成 </summary>
            UnDone = 1,

            /// <summary> 2：超时计划 </summary>
            OverTime = 2
        }

        /// <summary>
        /// 计划列表类型
        /// </summary>
        public enum PlanListType
        {
            /// <summary>我的计划</summary>
            Mine = 0,

            /// <summary> 下属计划 </summary>
            Subordinate = 1,

            /// <summary> 协作计划 </summary>
            Cooperation = 2
        }

        /// <summary>
        /// 画面计划状态
        /// </summary>
        public enum PlanPageStatus
        {
            /// <summary> 1：待提交  </summary>
            UnSubmit = 1,

            /// <summary>  2：待审核  </summary>
            Checking = 2,

            /// <summary> 3：进行中 </summary>
            Running = 3,

            /// <summary>4：待确认  </summary>
            Confirming = 4,

            /// <summary> 5：已完成   </summary>
            Complete = 5,

            /// <summary> 6：已中止 </summary>
            Stop = 6
        }

        /// <summary>
        /// 计划状态
        /// </summary>
        public enum PlanStatus
        {
            /// <summary> 0：未提交  </summary>
            UnSubmit = 0,

            /// <summary>  10：审核中  </summary>
            Checking = 10,

            /// <summary> 15：审核未通过 </summary>
            CheckDenied = 15,

            /// <summary>20：审核通过  </summary>
            CheckPassed = 20,

            /// <summary> 25：申请修改   </summary>
            RequestEdit = 25,

            /// <summary> 30：等待确认 </summary>
            Confirming = 30,

            /// <summary> 40：确认未通过 </summary>
            ConfirmDenied = 40,

            /// <summary> 90：已完成 </summary>
            Complete = 90
        }

        /// <summary>
        /// 循环计划状态
        /// </summary>
        public enum LoopPlanStatus
        {
            /// <summary> 0：未提交  </summary>
            UnSubmit = 0,

            /// <summary>  10：审核中  </summary>
            Checking = 10,

            /// <summary> 15：审核未通过 </summary>
            CheckDenied = 15,

            /// <summary>20：审核通过  </summary>
            CheckPassed = 20,

            /// <summary> 25：申请修改   </summary>
            RequestEdit = 25
        }

        /// <summary>
        /// 循环计划提交状态
        /// </summary>
        public enum LoopPlanSubmitStatus
        {
            /// <summary> 0：未提交  </summary>
            UnSubmit = 0,

            /// <summary> 30：等待确认 </summary>
            Confirming = 30,

            /// <summary> 90：已完成 </summary>
            Complete = 90
        }

        /// <summary>
        /// 循环计划循环类型
        /// </summary>
        public enum LoopPlanLoopType
        {
            /// <summary>
            /// 1：日
            /// </summary>
            Day = 1,

            /// <summary>
            /// 2：周
            /// </summary>
            Week = 2,

            /// <summary>
            /// 3：月
            /// </summary>
            Month = 3,

            /// <summary>
            /// 4：年
            /// </summary>
            Year = 4
        }

        /// <summary>
        /// 计划操作类型
        /// </summary>
        public enum PlanOperateStatus
        {
            /// <summary>1、提交</summary>
            Submit = 1,

            /// <summary>2、审核通过</summary>
            CheckPass = 2,

            /// <summary>3、审核不通过</summary>
            CheckNoPass = 3,

            /// <summary>4、取消提交</summary>
            CancelSubmit = 4,

            /// <summary>5、取消审核</summary>
            CancelCheck = 5,

            /// <summary>6、评论</summary>
            Discuss = 6,

            /// <summary>7、下载</summary>
            DownLoad = 7,

            /// <summary>8、查看</summary>
            Read = 8,

            /// <summary>9、转办</summary>
            Change = 9,

            /// <summary>10、申请修改</summary>
            Update = 10,

            /// <summary>11、申请中止</summary>
            Stop = 11,

            /// <summary>12、重新开始</summary>
            ReStart = 12,

            /// <summary>13、删除 </summary>
            Delete = 13,

            /// <summary>14、确认通过</summary>
            ConfirmPass = 14,

            /// <summary>15、确认未通过</summary>
            ConfirmNoPass = 15,

            /// <summary>16、更新进度</summary>
            UpdateProcess = 16,

            /// <summary>17、分解</summary>
            Resolve = 17,

            /// <summary>18、新建计划</summary>
            CreatePlan = 18,

            /// <summary>19、新建循环计划</summary>
            CreateLoopPlan = 19,

            /// <summary>20、修改保存</summary>
            UpdateSave = 20,

            /// <summary>21、提交循环计划</summary>
            SubmitLoopPlan = 21,

            /// <summary>22、提交循环计划</summary>
            SubmitConfirm = 22,

            /// <summary>23、申请修改审核通过</summary>
            UpdateCheckPass = 23,

            /// <summary>24、申请修改审核不通过</summary>
            UpdateCheckNoPass = 24,

            /// <summary>25、申请中止审核通过</summary>
            StopCheckPass = 25,

            /// <summary>26、申请中止审核不通过</summary>
            StopCheckNoPass = 26
        }

        /// <summary>
        /// 计划中止状态
        /// </summary>
        public enum PlanStopStatus
        {
            /// <summary> 0：运行中 </summary>
            Running = 0,

            /// <summary> 10：审核中 </summary>
            Checking = 10,

            /// <summary> 90：已中止 </summary>
            Stopped = 90
        }

        /// <summary>
        /// 循环计划中止状态
        /// </summary>
        public enum LoopPlanStopStatus
        {
            /// <summary> 0：运行中 </summary>
            Running = 0,

            /// <summary> 10：审核中 </summary>
            Checking = 10,

            /// <summary> 90：已中止 </summary>
            Stopped = 90
        }

        /// <summary>
        /// 计划排序
        /// </summary>
        public enum PlanSortType
        {
            /// <summary> 1：默认 </summary>
            Default = 1,

            /// <summary> 2：重要度 </summary>
            Importance = 2,

            /// <summary> 3：紧急度 </summary>
            Urgency = 3,

            /// <summary> 4：时间 </summary>
            Time = 4,

            /// <summary> 5：责任人 </summary>
            ResponsibleUser = 5,

            /// <summary> 6：确认人 </summary>
            ConfirmUser = 6,

            /// <summary> 7：计划状态 </summary>
            Status = 7,

            /// <summary> 8：组织架构 </summary>
            Organization = 8,
        }

        /// <summary>
        /// 计划分组
        /// </summary>
        public enum PlanGroupType
        {
            /// <summary>-1：不分组</summary>
            None = -1,

            /// <summary>1：时间</summary>
            Time = 1,

            /// <summary>2：部门</summary>
            Organization = 2,

            /// <summary>3：人员</summary>
            User = 3,
        }

        /// <summary>
        /// 时间分组
        /// </summary>
        public enum TimeGroup
        {
            /// <summary>更早</summary>
            Earlier = 0,

            /// <summary>昨天</summary>
            Yesterday = 1,

            /// <summary>今天</summary>
            Today = 2,

            /// <summary>明天</summary>
            Tomorrow = 3,

            /// <summary>更晚</summary>
            Later = 4
        }

        #endregion 计划相关

        #region 目标相关

        /// <summary>
        /// 目标状态
        /// </summary>
        public enum ObjectIndexStatus
        {
            /// <summary>1：待提交</summary>
            UnSubmit = 1,

            /// <summary>2:待审核</summary>
            UnChecked = 2,

            /// <summary>3：审核通过</summary>
            HasChecked = 3,

            /// <summary>4：待确认</summary>
            UnConfirm = 4,

            /// <summary>5：已完成</summary>
            HasCompleted = 5,

            /// <summary>6、超时</summary>
            OverTime = 6,
        }

        #endregion 目标相关

        #region 标签相关

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

        #endregion 标签相关

        #region 文档相关

        /// <summary>
        /// 文档权限类型
        /// </summary>
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
        /// 公司文档权限对象类型
        /// </summary>
        public enum FolderAuthType
        {
            ///<summary>1：组织架构</summary>
            Organization = 1,

            ///<summary>2：岗位</summary>
            Station = 2,

            ///<summary>3：用户</summary>
            User = 3
        }

        /// <summary>
        /// 文件目录类型
        /// </summary>
        public enum FileFolderType
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
            IMFile = 6,

            /// <summary>7：目标文件</summary>
            ObjectiveFile = 7,

            /// <summary>8:流程文档</summary>
            FlowFile = 8,

            /// <summary>9:会议附件</summary>
            MeetingFile = 9,

            /// <summary>预览文件</summary>
            PreviewFile = 10
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public enum FileType
        {
            /// <summary>1：计划附件</summary>
            Plan = 1,

            /// <summary>2：循环计划附件</summary>
            LoopPlan = 2,

            /// <summary>3：我的文档</summary>
            MyDocument = 3,

            /// <summary>4：公司文档</summary>
            CompanyDocument = 4,

            /// <summary>5：用户头像</summary>
            HeadImage = 5
        }

        #endregion 文档相关

        #region 流程相关
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
        /// 控件权限
        /// </summary>
        public enum NodeControlStatus
        {
            /// <summary>0、隐藏 </summary>
            Hide = 0,

            /// <summary>1、只读</summary>
            ReadOnly = 1,

            /// <summary>2、编辑</summary>
            Edit = 2
        }

        #endregion 流程相关

        #region 表单相关

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
            Countersign = 3,

            /// <summary>4：归档</summary>
            Archive = 4,
        }

        /// <summary>
        /// 表单流程状态
        /// </summary>
        public enum FormFlowStatus
        {
            /// <summary>1、待提交</summary>
            UnSubmit = 1,

            /// <summary>2、已提交</summary>
            HasSubmited = 2,

            /// <summary>3、待处理</summary>
            UnCheck = 3,

            /// <summary>4、已处理</summary>
            HasChecked = 4,

            /// <summary>5、已办结</summary>
            HasCompleted = 5,

            /// <summary>6、待审核</summary>
            UnExamine = 6,

            /// <summary>7、待查阅</summary>
            UnRead = 7,

            /// <summary>8、已审核</summary>
            HasExamine = 8,

            /// <summary>9、已查阅</summary>
            HasRead = 9,

            /// <summary>10、节点类型:提交</summary>
            SubmitMsg = 10,

            /// <summary>11、管理员：流程中</summary>
            Flowing = 11,

            /// <summary>12、委托 </summary>
            FlowEntrust = 12,

            /// <summary>13、已提交：从没审批或被他人提交过</summary>
            NeverChecked = 13
        }

        /// <summary>
        /// 表单状态
        /// </summary>
        public enum UserFormStatus
        {
            /// <summary>1、待提交</summary>
            UnSubmit = 1,

            /// <summary>2、流程中</summary>
            Flowing = 2,

            /// <summary>50、已办结</summary>
            HasCompleted = 50
        }

        #endregion 表单相关

        #region 统计相关

        /// <summary>
        /// 统计时间周期类型
        /// </summary>
        public enum StatisticsType
        {
            /// <summary>0：日统计</summary>
            Day = 0,

            /// <summary>1：周统计</summary>
            Week = 1,

            /// <summary>2：月统计</summary>
            Month = 2,

            /// <summary>3：年统计</summary>
            Year = 3
        }

        public enum DateType
        {
            /// <summary>0：不确定</summary>
            None = 0,

            /// <summary>1：工作日</summary>
            Workday = 1,

            /// <summary>2：节假日</summary>
            Holiday = 2,
        }

        #endregion 统计相关
    }
}