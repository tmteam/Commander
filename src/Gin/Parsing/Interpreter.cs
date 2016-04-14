using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGin
{
    public class Interpreter
    {
        public readonly ICommandLibrary library;
        public Interpreter(ICommandLibrary library) {
            this.library = library;
        }
        public Instruction Create(List<string> args)
        {
            string cmdName = ParseTools.ExtractCommandName(args);

            var description = library.GetOrNull(cmdName);
            if (description == null)
                throw new UnknownCommandNameException(cmdName);

            var intervalArgumentsDescription
                 = ReflectionTools.GetArgumentsDescription(typeof(CommandRunProperties));
            
            var intervalSettings = new CommandRunProperties();

            ReflectionTools.ExtractAnsSetToProperties(args, intervalArgumentsDescription, intervalSettings);
            
            ReflectionTools.ExtractAnsSetToProperties(args, description.Arguments, description.Exemplar);

            if (args.Count != 0)
                throw new UnknownArgumentsException(args.ToArray());
            
            return new Instruction
            {
                ConfiguredCommand  = description.Exemplar,
                ScheduleProperties = intervalSettings,
            };
        }
    }
}
