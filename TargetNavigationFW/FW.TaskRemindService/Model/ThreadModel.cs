using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FW.TaskRemindService.Model
{
    public class ThreadModel
    {
        public int threadId { get; set; }

        public Socket socketClient { get; set; }

        public ManualResetEvent reviceManager { get; set; }
    }
}
