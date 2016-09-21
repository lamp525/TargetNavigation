using System.Collections.Generic;

namespace MB.Model
{
    public class AddUserInfo
    {
        //组织名
        public List<OrganizationInfo> orgNameList { get; set; }

        //项目名
        public List<ProjectInfo> projectList { get; set; }

        //确认人
        public UserInfo UpUser { get; set; }

        // 责任人
        public UserInfo DownUser { get; set; }
    }
}