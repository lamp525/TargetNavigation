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
    
    public partial class NewDocumentModel
    {
        public int documentId { get; set; }
        public string displayName { get; set; }
        public string description { get; set; }
        public Nullable<bool> isFolder { get; set; }
        public int createUser { get; set; }
        public string createUserName { get; set; }
        public System.DateTime createTime { get; set; }
        public string saveName { get; set; }
        public Nullable<int> power { get; set; }
        public Nullable<bool> withShared { get; set; }
        public Nullable<int> folder { get; set; }
        public string extension { get; set; }
    }
}
