namespace MB.Model
{
    public class ImContactModel
    {
        /// <summary>用户/群组ID</summary>
        public int id { get; set; }

        /// <summary>联系人ID</summary>
        public int contactId { get; set; }

        /// <summary>联系人名</summary>
        public string contactName { get; set; }

        /// <summary>头像</summary>
        public string headImage { get; set; }

        /// <summary>权限</summary>
        public int? power { get; set; }

        /// <summary>在线</summary>
        public bool onLine { get; set; }

        /// <summary>岗位</summary>
        public string station { get; set; }
    }
}