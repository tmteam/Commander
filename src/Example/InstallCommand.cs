using Commander;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    [Command("Registrate the commander as a service")]
    public class InstallCommand: CommandBase
    {
        public override void Run()
        {
             ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
        }
    }
}
