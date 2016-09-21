using System.ComponentModel;

namespace FW.MBService
{
    [RunInstaller(true)]
    public partial class MBServiceInstaller : System.Configuration.Install.Installer
    {
        public MBServiceInstaller()
        {
            InitializeComponent();
        }
    }
}