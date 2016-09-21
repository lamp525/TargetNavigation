using System;

namespace MB.Model
{
    public class IndexImageInfo
    {
        public int imageId { get; set; }
        public string imageName { get; set; }
        public Nullable<bool> display { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public string imgUrl { get; set; }
    }
}