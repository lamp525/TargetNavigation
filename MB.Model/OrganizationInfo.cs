using System;

namespace MB.Model
{
    public class OrganizationInfo
    {
        public int? organizationId { get; set; }
        public Nullable<int> parentOrganization { get; set; }
        public string schemaName { get; set; }
        public string organizationName { get; set; }
        public Nullable<bool> withSub { get; set; }
        public string orderNumber { get; set; }
    }
}