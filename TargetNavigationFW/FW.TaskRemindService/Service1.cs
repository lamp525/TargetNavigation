using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FW.TaskRemindService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        private ServiceHost _Host = null;
        protected override void OnStart(string[] args)
        {
            if (_Host != null) return;
            _Host = new ServiceHost(typeof(TaskRemindService));
            _Host.Open();
            //开启socket服务
            new TaskRemindService().CreateSocketService();
        }

        protected override void OnStop()
        {
            if (_Host.State != CommunicationState.Closed)
                _Host.Close();
        }
    }
}
