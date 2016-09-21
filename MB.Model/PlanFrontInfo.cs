using System;

namespace MB.Model
{
    public class PlanFrontInfo
    {
        #region Model

        private int _planid;
        private int _planfront;
        private int _createuser;
        private DateTime _createtime;
        private int _updateuser;
        private DateTime _updatetime;

        /// <summary>
        ///
        /// </summary>
        public int planId
        {
            set { _planid = value; }
            get { return _planid; }
        }

        /// <summary>
        ///
        /// </summary>
        public int planFront
        {
            set { _planfront = value; }
            get { return _planfront; }
        }

        /// <summary>
        ///
        /// </summary>
        public int createUser
        {
            set { _createuser = value; }
            get { return _createuser; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime createTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }

        /// <summary>
        ///
        /// </summary>
        public int updateUser
        {
            set { _updateuser = value; }
            get { return _updateuser; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime updateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }

        #endregion Model
    }
}