namespace MB.Model
{
    public class OrgModel
    {
        //组织Id
        public int? organizationId { get; set; }

        //父组织Id
        public int? parentOrganization { get; set; }

        //父组织名
        public string parentName { get; set; }

        //组织类型
        public int? schemaName { get; set; }

        //组织名
        public string organizationName { get; set; }

        //描述
        public string description { get; set; }

        //有无子组织
        public bool? withSub { get; set; }

        //排序
        public int? orderNumber { get; set; }

        public int id { get; set; }
        public string name { get; set; }
        public bool isParent { get; set; }
    }
}