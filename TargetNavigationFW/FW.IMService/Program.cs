using System.ServiceProcess;

namespace FW.IMService
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new IMService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}