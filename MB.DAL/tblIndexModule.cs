//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MB.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblIndexModule
    {
        public int moduleId { get; set; }
        public string title { get; set; }
        public Nullable<bool> displayTitle { get; set; }
        public Nullable<bool> linkTarget { get; set; }
        public Nullable<int> maxRow { get; set; }
        public Nullable<int> width { get; set; }
        public Nullable<int> height { get; set; }
        public Nullable<int> type { get; set; }
        public string position { get; set; }
        public Nullable<int> defaultEfficiency { get; set; }
        public Nullable<int> topDisplay { get; set; }
        public Nullable<int> topDisplayLine { get; set; }
        public Nullable<int> defaultLine { get; set; }
        public Nullable<bool> efficiencyValue { get; set; }
        public Nullable<bool> executiveForce { get; set; }
        public Nullable<bool> objective { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
    }
}
