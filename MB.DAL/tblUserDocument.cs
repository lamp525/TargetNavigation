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
    
    public partial class tblUserDocument
    {
        public int documentId { get; set; }
        public Nullable<int> folder { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public string saveName { get; set; }
        public string extension { get; set; }
        public Nullable<bool> withShared { get; set; }
        public Nullable<bool> isFolder { get; set; }
        public string keyword { get; set; }
        public int createUser { get; set; }
        public System.DateTime createTime { get; set; }
        public int updateUser { get; set; }
        public System.DateTime updateTime { get; set; }
        public bool deleteFlag { get; set; }
    }
}
