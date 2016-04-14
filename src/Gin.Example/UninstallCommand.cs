using TheGin;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.Example
{
    [Command("Deregistrate the commander as a service (administrator only)")]
    public class UnistallCommand: CommandBase
    {
        public override void Run()
        {
            ManagedInstallerClass.InstallHelper(
                new[] { "/u", Assembly.GetExecutingAssembly().Location });
        }
    }
}
