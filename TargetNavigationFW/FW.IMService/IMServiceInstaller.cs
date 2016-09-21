using System.ComponentModel;

namespace FW.IMService
{
    [RunInstaller(true)]
    public partial class IMServiceInstaller : System.Configuration.Install.Installer
    {
        public IMServiceInstaller()
        {
            InitializeComponent();
        }
    }
}