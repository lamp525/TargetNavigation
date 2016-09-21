using System;

namespace MB.Model
{
    public class PersonalSetting
    {
        public Nullable<int> settingId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<int> refreshTime { get; set; }
        public Nullable<int> pageSize { get; set; }
        public Nullable<int> createUser { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public Nullable<int> updateUser { get; set; }
        public Nullable<System.DateTime> updateTime { get; set; }
        public string bigImage { get; set; }
        public string midImage { get; set; }
        public string smallImage { get; set; }
    }
}