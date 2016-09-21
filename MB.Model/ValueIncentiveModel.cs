namespace MB.Model
{
    public class ValueIncentiveModel
    {
        public int? standardTime { get; set; }
        public int? incentiveType { get; set; }
        public int? maxValueType { get; set; }
        public decimal? maxValue { get; set; }
        public decimal? maxAverage { get; set; }
        public int? minValueType { get; set; }
        public decimal? minValue { get; set; }
        public decimal? minAverage { get; set; }
    }
}