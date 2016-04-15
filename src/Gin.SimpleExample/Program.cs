using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin.SimpleExample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var gin = new Gin();
            /* Confugure it for your needs
            var gin = new Gin(
                addExitCommand:  true, 
                addHelpCommand:  true, 
                searchBehaviour: SearchCommandBehaviour.ScanAllSolutionAssemblies,
                logFileName:     null
                );
             */
            //Launches help command
            gin.Execute("help");
            //Launches console input loop
            gin.RunInputLoop();
            
            Console.WriteLine("Press any key to continue...");
        }
    }
}
