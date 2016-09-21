namespace MB.New.Model
{
    public class OrganizationInfoModel
    {
        public int? organizationId { get; set; }
        public int? parentOrganization { get; set; }
        public string schemaName { get; set; }
        public string organizationName { get; set; }
        public bool? withSub { get; set; }
        public string orderNumber { get; set; }

        public int? level { get; set; }
    }
}