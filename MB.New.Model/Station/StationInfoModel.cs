namespace MB.New.Model
{
    public class StationInfoModel
    {
        /// <summary>
        /// 职位
        /// </summary>
        public int? stationId { get; set; }

        public string stationName { get; set; }

        public int? parentStation { get; set; }

        /// <summary>
        /// 组织Id
        /// </summary>
        public int? orgId { get; set; }


        public int? level { get; set; }
    }
}