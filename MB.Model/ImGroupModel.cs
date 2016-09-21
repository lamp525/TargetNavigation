namespace MB.Model
{
    public class ImGroupModel
    {
        /// <summary>群组ID</summary>
        public int? groupId { get; set; }

        /// <summary>群组名</summary>
        public string groupName { get; set; }

        /// <summary>头像</summary>
        public string headImage { get; set; }

        /// <summary>群组类型</summary>
        public int type { get; set; }

        /// <summary>创建用户</summary>
        public int createUser { get; set; }

        /// <summary>群组成员ID</summary>
        public int[] groupUserId { get; set; }
    }
}