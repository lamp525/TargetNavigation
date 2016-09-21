namespace MB.Model
{
    public class CityManagement
    {
        //城市Id
        public int cityId { get; set; }

        public string cityName { get; set; }

        //邮编
        public string zipCode { get; set; }

        //省份Id
        public int? provinceId { get; set; }
    }
}