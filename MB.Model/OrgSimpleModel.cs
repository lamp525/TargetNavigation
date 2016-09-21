namespace MB.Model
{
    public class OrgSimpleModel
    {
        //组织Id
        public int id { get; set; }

        public int[] parentIds { get; set; }

        //组织名称
        public string name { get; set; }

        //是否含下级
        public bool isParent { get; set; }
    }
}