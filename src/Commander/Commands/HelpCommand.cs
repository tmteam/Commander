using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    [Command("Show allowed commands help")]
    public class HelpCommand : CommandBase
    {
        public readonly IEnumerable<CommandDescription> Commands;

        public HelpCommand(IEnumerable<CommandDescription> commands)
        {
            this.Commands = commands;
        }

        private void fillArgDescription(StringBuilder msg, ArgumentDescription arg)
        {
            msg.AppendLine("    -" + arg.Description.ShortAlias + ",  (" + arg.Property.PropertyType.Name + ")   " + arg.Description.Description);
        }

        public override void Run()
        {
            StringBuilder msg = new StringBuilder("Threre are " + this.Commands.Count<CommandDescription>() + " commands: \r\n");
            foreach (CommandDescription description in this.Commands)
            {
                msg.AppendLine("[" + Tools.NormilizeCommandTypeName(description.type.Name) + "] - " + description.attribute.Description);
                ArgumentDescription[] descriptionArray = (from a in description.arguments
                                                          where !a.Description.Optional
                                                          select a).ToArray<ArgumentDescription>();
                if (descriptionArray.Length > 0)
                {
                    msg.AppendLine("neccessary: ");
                    foreach (ArgumentDescription description2 in descriptionArray)
                    {
                        this.fillArgDescription(msg, description2);
                    }
                }
                ArgumentDescription[] descriptionArray2 = (from a in description.arguments
                                                           where a.Description.Optional
                                                           select a).ToArray<ArgumentDescription>();
                if (descriptionArray2.Length > 0)
                {
                    msg.AppendLine("optional: ");
                    foreach (ArgumentDescription description2 in descriptionArray2)
                    {
                        this.fillArgDescription(msg, description2);
                    }
                }
            }
            this.Log.WriteMessage(msg.ToString());
        }
    }


}
