using System;

namespace MB.Model
{
    public class DocumentInfo
    {
        #region Model：DocumentInfo

        private int _documentid;
        private int? _directoryid;
        private string _displayname;
        private string _savename;
        private string _extension;
        private bool _archive;
        private DateTime? _archivetime;
        private int _createuser;
        private DateTime _createtime;
        private int _updateuser;
        private DateTime _updatetime;
        private bool _deleteflag;

        /// <summary>
        /// 文档ID
        /// </summary>
        public int documentId
        {
            set { _documentid = value; }
            get { return _documentid; }
        }

        /// <summary>
        /// 目录ID
        /// </summary>
        public int? directoryId
        {
            set { _directoryid = value; }
            get { return _directoryid; }
        }

        /// <summary>
        /// 表示名
        /// </summary>
        public string displayName
        {
            set { _displayname = value; }
            get { return _displayname; }
        }

        /// <summary>
        /// 存储名
        /// </summary>
        public string saveName
        {
            set { _savename = value; }
            get { return _savename; }
        }

        /// <summary>
        /// 后缀名
        /// </summary>
        public string extension
        {
            set { _extension = value; }
            get { return _extension; }
        }

        /// <summary>
        /// 归档标志
        /// </summary>
        public bool archive
        {
            set { _archive = value; }
            get { return _archive; }
        }

        /// <summary>
        /// 归档时间
        /// </summary>
        public DateTime? archiveTime
        {
            set { _archivetime = value; }
            get { return _archivetime; }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int createUser
        {
            set { _createuser = value; }
            get { return _createuser; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public int updateUser
        {
            set { _updateuser = value; }
            get { return _updateuser; }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }

        /// <summary>
        /// 删除标志
        /// </summary>
        public bool deleteFlag
        {
            set { _deleteflag = value; }
            get { return _deleteflag; }
        }

        #endregion Model：DocumentInfo

        #region 功能添加字段，将页面上的时间绑定为短日期格式。      --谢小鹏

        /// <summary>
        /// 媒介字段，将View页面的时间转换短日期(2015-1-1)格式；
        /// </summary>
        public string dtShort { get; set; }

        #endregion 功能添加字段，将页面上的时间绑定为短日期格式。      --谢小鹏
    }
}