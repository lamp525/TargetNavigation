namespace MB.Model
{
    public class StationModel
    {
        //组织Id
        public int organizationId { get; set; }

        //组织架构名
        public string organizationName { get; set; }

        //职位Id
        public int? stationId { get; set; }

        //职位名
        public string stationName { get; set; }

        //上级岗位Id
        public int? parentStation { get; set; }

        //上级岗位名
        public string parentStationName { get; set; }

        public string comment { get; set; }

        //流程审批人
        public bool? approval { get; set; }

        //越级审批
        public bool? skipApproval { get; set; }

        //岗位人员
        public string[] userName { get; set; }

        //岗位人员数
        public int? userNum { get; set; }

        public int id { get; set; }
        public string name { get; set; }
        public string affiliationName { get; set; }
    }
}