using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class CommandFactory
    {
        public readonly TypeScanner Scanner;
        public CommandFactory(TypeScanner scanner) {
            this.Scanner = scanner;
        }

        public ICommand CreateAndConfigure(string commandName, List<string> args) {
            var cmdDescription = this.Scanner.GetOrNull(commandName);
            if (cmdDescription == null)
                throw new UnknownCommandNameException(commandName);
            var command = cmdDescription.exemplarFactory.GetExemplar();
            ReflectionTools.ExtractAnsSetToProperties(args, cmdDescription.arguments, command);
            if(args.Count!=0)
                    throw new UnknownArgumentsException(args.ToArray());
            return command;
        }
    }
}
