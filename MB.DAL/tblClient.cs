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
    
    public partial class tblClient
    {
        public int clientId { get; set; }
        public string ip { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<bool> onLine { get; set; }
        public Nullable<int> remind { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public Nullable<System.DateTime> updateTime { get; set; }
    }
}