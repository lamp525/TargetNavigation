namespace MB.Web.Common
{
    public class ConfigHelper
    {
        public static int InputErrorValidate
        {
            get
            {
                int inputErrorValidate = 0;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["InputErrorValidate"].ToString(), out inputErrorValidate);
                return inputErrorValidate;
            }
        }
    }
}