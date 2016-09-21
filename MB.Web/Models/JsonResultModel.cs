namespace MB.Web.Models
{
    public enum JsonResultType
    {
        success = 1,
        error = 0
    }

    public class JsonResultModel
    {
        public JsonResultModel(JsonResultType resultType, object data, string message = "OK", bool login = true, bool access = true)
        {
            if (resultType == JsonResultType.success)
            {
                this.success = true;
            }
            else
            {
                this.success = false;
            }
            this.data = data;
            this.login = login;
            this.access = access;
            this.message = message;
        }

        public bool success { get; set; }
        public bool login { get; set; }
        public bool access { get; set; }
        public object data { get; set; }
        public string message { get; set; }
    }
}