using System;

namespace MB.Model
{
    public class UserInfo
    {
        #region Model

        private int _userid;
        private string _usernumber;
        private string _username;
        private string _randomKey;
        private string _imagePosition;

        public string ImagePosition
        {
            get { return _imagePosition; }
            set { _imagePosition = value; }
        }

        private string _originalImage;
        private string _bigimage;
        private string _midimage;
        private string _smallimage;
        private string _password;
        private bool _sex;
        private string _nation;
        private string _political;
        private bool _marriage;
        private string _identitycard;
        private string _mobile1;
        private string _mobile2;
        private string _address;
        private string _workplace;
        private DateTime? _entrytime;
        private int? _probationaryperiod;
        private DateTime? _positivedate;
        private int? _term;
        private DateTime? _expireddate;
        private string _comment;
        private DateTime? _birthday;
        private int? _age;
        private string _nature;
        private int? _province;
        private int? _city;
        private int? _district;
        private string _township;
        private string _school;
        private string _professional;
        private int? _education;
        private DateTime? _firstwork;
        private string _qualification;
        private string _cornet;
        private string _emergencynumber;
        private int? _bankcard;
        private int? _workstatus;
        private decimal? _salary;
        private int? _security;
        private bool _admin;
        private bool _execution;
        private int _createuser;
        private DateTime _createtime;
        private int _updateuser;
        private DateTime _updatetime;
        private bool _deleteflag;

        /// <summary>
        ///
        /// </summary>
        public int userId
        {
            set { _userid = value; }
            get { return _userid; }
        }

        /// <summary>
        ///
        /// </summary>
        public string userNumber
        {
            set { _usernumber = value; }
            get { return _usernumber; }
        }

        /// <summary>
        ///
        /// </summary>
        public string userName
        {
            set { _username = value; }
            get { return _username; }
        }

        /// <summary>
        ///
        /// </summary>
        public string randomKey
        {
            set { _randomKey = value; }
            get { return _randomKey; }
        }

        /// <summary>
        ///
        /// </summary>
        public string OriginalImage
        {
            get { return _originalImage; }
            set { _originalImage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public string bigImage
        {
            set { _bigimage = value; }
            get { return _bigimage; }
        }

        /// <summary>
        ///
        /// </summary>
        public string midImage
        {
            set { _midimage = value; }
            get { return _midimage; }
        }

        /// <summary>
        ///
        /// </summary>
        public string smallImage
        {
            set { _smallimage = value; }
            get { return _smallimage; }
        }

        /// <summary>
        ///
        /// </summary>
        public string password
        {
            set { _password = value; }
            get { return _password; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool sex
        {
            set { _sex = value; }
            get { return _sex; }
        }

        /// <summary>
        ///
        /// </summary>
        public string nation
        {
            set { _nation = value; }
            get { return _nation; }
        }

        /// <summary>
        ///
        /// </summary>
        public string political
        {
            set { _political = value; }
            get { return _political; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool marriage
        {
            set { _marriage = value; }
            get { return _marriage; }
        }

        /// <summary>
        ///
        /// </summary>
        public string identityCard
        {
            set { _identitycard = value; }
            get { return _identitycard; }
        }

        /// <summary>
        ///
        /// </summary>
        public string mobile1
        {
            set { _mobile1 = value; }
            get { return _mobile1; }
        }

        /// <summary>
        ///
        /// </summary>
        public string mobile2
        {
            set { _mobile2 = value; }
            get { return _mobile2; }
        }

        /// <summary>
        ///
        /// </summary>
        public string address
        {
            set { _address = value; }
            get { return _address; }
        }

        /// <summary>
        ///
        /// </summary>
        public string workPlace
        {
            set { _workplace = value; }
            get { return _workplace; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime? entryTime
        {
            set { _entrytime = value; }
            get { return _entrytime; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? probationaryPeriod
        {
            set { _probationaryperiod = value; }
            get { return _probationaryperiod; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime? positiveDate
        {
            set { _positivedate = value; }
            get { return _positivedate; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? term
        {
            set { _term = value; }
            get { return _term; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime? expiredDate
        {
            set { _expireddate = value; }
            get { return _expireddate; }
        }

        /// <summary>
        ///
        /// </summary>
        public string comment
        {
            set { _comment = value; }
            get { return _comment; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime? birthday
        {
            set { _birthday = value; }
            get { return _birthday; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? age
        {
            set { _age = value; }
            get { return _age; }
        }

        /// <summary>
        ///
        /// </summary>
        public string nature
        {
            set { _nature = value; }
            get { return _nature; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? province
        {
            set { _province = value; }
            get { return _province; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? city
        {
            set { _city = value; }
            get { return _city; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? district
        {
            set { _district = value; }
            get { return _district; }
        }

        /// <summary>
        ///
        /// </summary>
        public string township
        {
            set { _township = value; }
            get { return _township; }
        }

        /// <summary>
        ///
        /// </summary>
        public string school
        {
            set { _school = value; }
            get { return _school; }
        }

        /// <summary>
        ///
        /// </summary>
        public string professional
        {
            set { _professional = value; }
            get { return _professional; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? education
        {
            set { _education = value; }
            get { return _education; }
        }

        /// <summary>
        ///
        /// </summary>
        public DateTime? firstWork
        {
            set { _firstwork = value; }
            get { return _firstwork; }
        }

        /// <summary>
        ///
        /// </summary>
        public string qualification
        {
            set { _qualification = value; }
            get { return _qualification; }
        }

        /// <summary>
        ///
        /// </summary>
        public string cornet
        {
            set { _cornet = value; }
            get { return _cornet; }
        }

        /// <summary>
        ///
        /// </summary>
        public string emergencyNumber
        {
            set { _emergencynumber = value; }
            get { return _emergencynumber; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? bankCard
        {
            set { _bankcard = value; }
            get { return _bankcard; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? workStatus
        {
            set { _workstatus = value; }
            get { return _workstatus; }
        }

        /// <summary>
        ///
        /// </summary>
        public decimal? salary
        {
            set { _salary = value; }
            get { return _salary; }
        }

        /// <summary>
        ///
        /// </summary>
        public int? security
        {
            set { _security = value; }
            get { return _security; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool admin
        {
            set { _admin = value; }
            get { return _admin; }
        }

        /// <summary>
        ///
        /// </summary>
        public bool execution
        {
            set { _execution = value; }
            get { return _execution; }
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

        /// <summary>
        ///
        /// </summary>
        public bool deleteFlag
        {
            set { _deleteflag = value; }
            get { return _deleteflag; }
        }

        #endregion Model

        public int? stationId { get; set; }
        public string stationName { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public int id { get; set; }
        public int? errorPass { get; set; }
        public int? organizationId { get; set; }
        public string organizationName { get; set; }

        //从属父级名称
        public string affiliationName { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoSimpleModel
    {
        //用户ID
        public int userId { get; set; }

        //用户名
        public string userName { get; set; }

        //用户头像
        public string smallImage { get; set; }

        //岗位ID
        public int stationId { get; set; }

        //岗位名称
        public string stationName { get; set; }

        //组织ID
        public int? organizationId { get; set; }

        //组织名称
        public string organizationName { get; set; }
    }
}