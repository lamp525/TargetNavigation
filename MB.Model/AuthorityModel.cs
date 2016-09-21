using System.Collections.Generic;

namespace MB.Model
{
    //权限信息
    public class AuthorityInfoModel
    {
        //文档Id
        public int documentId { get; set; }

        //文件夹名称
        public string displayName { get; set; }

        //文件夹描述
        public string description { get; set; }

        //权限集合
        public List<AuthorityModel> AuthorityList { get; set; }
    }

    //权限模型
    public class AuthorityModel
    {
        //权限Id
        public int authId { get; set; }

        //权限类型 1：组织架构 2：岗位 3：人
        public int type { get; set; }

        //权限  1：禁止访问 2：仅下载 3：下载和上传 4：完全控制
        public int power { get; set; }

        //权限名称
        public string powerName { get; set; }

        //权限结果Id
        public int[] resultId { get; set; }

        //目标Id
        public int?[] targetId { get; set; }

        //目标名称
        public string[] targetName { get; set; }
    }
}